using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Queries;

public class GetJobStatusesQuery : IRequest<List<ChannelJobStatus>>
{
}

public class GetChannelStatusesQueryHandler : IRequestHandler<GetJobStatusesQuery, List<ChannelJobStatus>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJobService _jobService;

    public GetChannelStatusesQueryHandler(IApplicationDbContext context,
        IJobService jobService)
    {
        _context = context;
        _jobService = jobService;
    }

    public async Task<List<ChannelJobStatus>> Handle(GetJobStatusesQuery request, CancellationToken cancellationToken)
    {
        var jobs = _jobService.GetJobs()?.ToList();
        var entities = await _context.Channels
            .Select(c => new ChannelJobStatus
            {
                ChannelId = c.Id,
                Status = GetJobStatus(jobs, c.Id),
            })
            .ToListAsync(cancellationToken);

        return entities;
    }

    private static JobStatus GetJobStatus(List<Job>? jobs, Guid jobId)
    {
        if (jobs == null)
        {
            return JobStatus.Unknown;
        }

        var job = jobs.Where(job => job.Id == jobId).FirstOrDefault();

        return job != null ? job.Status : JobStatus.Dead;
    }
}
