using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using Hippo.Core.Models;
using MediatR;

namespace Hippo.Application.ChannelStatuses.Queries;

public class GetChannelStatusesQuery : SearchFilter, IRequest<Page<ChannelJobStatusItem>>
{
    public Guid? ChannelId { get; set; }
}

public class GetChannelStatusesQueryHandler : IRequestHandler<GetChannelStatusesQuery, Page<ChannelJobStatusItem>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJobService _jobService;

    public GetChannelStatusesQueryHandler(IApplicationDbContext context,
        IJobService jobService)
    {
        _context = context;
        _jobService = jobService;
    }

    public async Task<Page<ChannelJobStatusItem>> Handle(GetChannelStatusesQuery request, CancellationToken cancellationToken)
    {
        List<ChannelJobStatusItem> entities;
        int totalItems;

        if (!request.ChannelId.HasValue)
        {
            (totalItems, entities) = GetPaginatedChannelsStatuses(request.Offset, request.PageSize);
        }
        else
        {
            entities = new List<ChannelJobStatusItem>();
            entities.Add(GetChannelStatus(request.ChannelId.Value));
            totalItems = 1;
        }

        return await Task.FromResult(new Page<ChannelJobStatusItem>
        {
            Items = entities,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalItems = totalItems
        });
    }

    private static JobStatus FindJobAndGetStatus(List<Job>? jobs, Guid jobId)
    {
        if (jobs is null)
        {
            return JobStatus.Unknown;
        }

        var job = jobs.FirstOrDefault(job => job.Id == jobId);

        return GetJobStatus(job);
    }

    private (int, List<ChannelJobStatusItem>) GetPaginatedChannelsStatuses(int offset, int pageSize)
    {
        var jobs = _jobService.GetJobs()?.ToList();
        var totalItems = jobs?.Count ?? 0;
        var paginatedChannelsStatuses = _context.Channels
            .Select(c => new ChannelJobStatusItem
            {
                ChannelId = c.Id,
                Status = FindJobAndGetStatus(jobs, c.Id),
            })
        .Skip(offset)
        .Take(pageSize)
        .ToList();

        return (totalItems, paginatedChannelsStatuses);
    }

    private ChannelJobStatusItem GetChannelStatus(Guid channelId)
    {
        var job = _jobService.GetJob(channelId.ToString());

        return new ChannelJobStatusItem
        {
            ChannelId = channelId,
            Status = GetJobStatus(job),
        };
    }

    private static JobStatus GetJobStatus(Job? job)
    {
        return job?.Status ?? JobStatus.Dead;
    }
}
