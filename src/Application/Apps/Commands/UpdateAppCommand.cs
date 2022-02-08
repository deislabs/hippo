using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using Hippo.Core.Entities;
using MediatR;

namespace Hippo.Application.Apps.Commands;

public class UpdateAppCommand : IRequest
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string StorageId { get; set; } = "";
}

public class UpdateAppCommandHandler : IRequestHandler<UpdateAppCommand>
{
    private readonly IApplicationDbContext _context;

    private readonly JobScheduler _jobScheduler = JobScheduler.Current;

    public UpdateAppCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateAppCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Apps
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(App), request.Id);
        }

        // if the user changes the bindle storage id, ALL channels will stop working until the user registers new revisions that satisfy the channels' rules.
        //
        // TODO: how do we want to handle channels that requested ChannelRevisionSelectionStrategy.UseSpecifiedRevision?
        if (entity.StorageId != request.StorageId)
        {
            foreach (Job job in _jobScheduler.GetRunningJobs())
            {
                foreach (var channel in entity.Channels)
                {
                    if (job.Id == channel.Id)
                    {
                        job.Stop();
                    }
                }
            }
        }

        entity.Name = request.Name;
        entity.StorageId = request.StorageId;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
