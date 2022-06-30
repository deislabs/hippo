using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using Hippo.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Queries;

public class GetJobStatusesQuery : SearchFilter, IRequest<Page<ChannelJobStatusItem>>
{
}

public class GetChannelStatusesQueryHandler : IRequestHandler<GetJobStatusesQuery, Page<ChannelJobStatusItem>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJobService _jobService;

    public GetChannelStatusesQueryHandler(IApplicationDbContext context,
        IJobService jobService)
    {
        _context = context;
        _jobService = jobService;
    }

    public async Task<Page<ChannelJobStatusItem>> Handle(GetJobStatusesQuery request, CancellationToken cancellationToken)
    {
        var jobs = _jobService.GetJobs()?.ToList();
        var entities = (await _context.Channels
            .Select(c => new ChannelJobStatusItem
            {
                ChannelId = c.Id,
                Status = GetJobStatus(jobs, c.Id),
            })
            .ToListAsync(cancellationToken))
            .Skip(request.Offset)
            .Take(request.PageSize)
            .ToList();

        return new Page<ChannelJobStatusItem>
        {
            Items = entities,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalItems = jobs is null ? 0 : jobs.Count
        };
    }

    private static JobStatus GetJobStatus(List<Job>? jobs, Guid jobId)
    {
        if (jobs is null)
        {
            return JobStatus.Unknown;
        }

        var job = jobs.Where(job => job.Id == jobId).FirstOrDefault();

        return job != null ? job.Status : JobStatus.Dead;
    }
}
