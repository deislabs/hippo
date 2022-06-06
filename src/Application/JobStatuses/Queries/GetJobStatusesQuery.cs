using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Queries;

public class GetJobStatusesQuery : IRequest<List<JobStatus>>
{
}

public class GetChannelStatusesQueryHandler : IRequestHandler<GetJobStatusesQuery, List<JobStatus>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJobService _jobService;

    public GetChannelStatusesQueryHandler(IApplicationDbContext context,
        IJobService jobService)
    {
        _context = context;
        _jobService = jobService;
    }

    public async Task<List<JobStatus>> Handle(GetJobStatusesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _context.Channels
            .Select(c => new JobStatus
            {
                ChannelId = c.Id,
                Status = _jobService.GetJobStatus(c.Id.ToString())
            })
            .ToListAsync(cancellationToken);

        return entities;
    }
}
