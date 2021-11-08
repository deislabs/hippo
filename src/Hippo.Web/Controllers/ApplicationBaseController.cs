using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Core.Interfaces;
using Hippo.Core.Models;
using Hippo.Core.Tasks;
using Hippo.Infrastructure.Data;
using Hippo.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hippo.Web.Controllers
{
    public abstract class ApplicationBaseController : BaseController
    {
        private protected readonly IUnitOfWork _unitOfWork;
        private protected readonly UserManager<Account> _userManager;
        private protected readonly ITaskQueue<ChannelReference> _channelsToReschedule;
        private protected readonly EventOrigin _eventSource;

        protected ApplicationBaseController(IUnitOfWork unitOfWork, UserManager<Account> userManager, ITaskQueue<ChannelReference> channelsToReschedule, ILogger logger, EventOrigin eventSource)
            : base(logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _channelsToReschedule = channelsToReschedule;
            _eventSource = eventSource;
        }

        protected async Task<ActionResult<Application>> CreateApplication(ICreateApplicationParameters request)
        {
            var applicationId = Guid.NewGuid();
            var application = new Application
            {
                Id = applicationId,
                Name = request.ApplicationName,
                StorageId = request.StorageId,
                Owner = await _userManager.FindByNameAsync(User.Identity.Name),
            };

            await _unitOfWork.Applications.AddNew(application);
            await _unitOfWork.SaveChanges();

            return application;
        }

        protected async Task<ActionResult<Channel>> CreateChannel(ICreateChannelParameters request)
        {
            var application = _unitOfWork.Applications.GetApplicationById(request.ApplicationId);
            if (application == null)
            {
                LogIfNotFound(application, request.ApplicationId);
                return NotFound();
            }
            // TODO: tidier
            var revision = request.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseSpecifiedRevision ?
                _unitOfWork.Revisions.GetRevisionByNumber(application, request.RevisionNumber) :
                null;
            if (request.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseSpecifiedRevision && revision == null)
            {
                LogIfNotFound(revision, request.RevisionNumber);
                return BadRequest($"Cannot create a channel at revision {request.RevisionNumber} as bindle {application.StorageId}/{request.RevisionNumber} does not exist or is not registered");
            }

            // Set up ancillary entites
            var domain = new Domain
            {
                Name = request.DomainName
            };
            var configuration = new Configuration
            {
                EnvironmentVariables = request.EnvironmentVariables.Select(kvp => new EnvironmentVariable { Key = kvp.Key, Value = kvp.Value }).ToList()
            };

            // The channel itself
            var channelId = Guid.NewGuid();
            var channel = new Channel
            {
                Id = channelId,
                Application = application,
                Name = request.ChannelName,
                Domain = domain,
                Configuration = configuration,
                RevisionSelectionStrategy = request.RevisionSelectionStrategy,
                RangeRule = request.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseRangeRule ? request.RangeRule : "",
                SpecifiedRevision = request.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseSpecifiedRevision ? revision : null,
            };

            // Wire up backlinks
            // TODO: not sure if this is needed but in-memory database is not great at it
            application.Channels.Add(channel);
            foreach (var ev in configuration.EnvironmentVariables)
            {
                ev.Configuration = configuration;
            }

            // Finalise
            channel.ReevaluateActiveRevision();

            await _unitOfWork.Channels.AddNew(channel);
            await _unitOfWork.EventLog.ChannelCreated(_eventSource, channel);
            await _unitOfWork.EventLog.ChannelRevisionChanged(_eventSource, channel, "(none)", "channel created");
            await _unitOfWork.SaveChanges();

            await _channelsToReschedule.Enqueue(new ChannelReference(channel.Application.Id, channel.Id), CancellationToken.None);

            return channel;
        }
    }
}
