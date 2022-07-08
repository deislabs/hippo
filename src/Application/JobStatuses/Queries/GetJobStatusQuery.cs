using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using MediatR;

namespace Hippo.Application.Channels.Queries;

public class GetJobStatusQuery : IRequest<ChannelJobStatusItem>
{
    public Guid ChannelId { get; set; }
}

public class GetJobStatusQueryHandler : IRequestHandler<GetJobStatusQuery, ChannelJobStatusItem>
{
    private readonly IJobService _jobService;

    public GetJobStatusQueryHandler(IJobService jobService)
    {
        _jobService = jobService;
    }

    public Task<ChannelJobStatusItem> Handle(GetJobStatusQuery request, CancellationToken cancellationToken)
    {
        var job = _jobService.GetJob(request.ChannelId.ToString());

        return Task.FromResult(new ChannelJobStatusItem
        {
            ChannelId = request.ChannelId,
            Status = job?.Status ?? JobStatus.Dead,
        });
    }
}
