using AutoMapper;
using Hippo.Application.Common.Interfaces;
using MediatR;

namespace Hippo.Application.Channels.Queries;

public class GetChannelLogsQuery : IRequest<GetChannelLogsVm>
{
    public GetChannelLogsQuery(Guid channelId)
    {
        ChannelId = channelId;
    }

    public Guid ChannelId { get; set; }
}

public class GetChannelLogsQueryHandler : IRequestHandler<GetChannelLogsQuery, GetChannelLogsVm>
{
    private readonly IJobService _jobService;

    public GetChannelLogsQueryHandler(IJobService nomadService)
    {
        _jobService = nomadService;
    }

    public async Task<GetChannelLogsVm> Handle(GetChannelLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = _jobService.GetJobLogs(request.ChannelId.ToString());
        var vm = new GetChannelLogsVm(logs.ToList());
        return await Task.FromResult(vm);
    }
}
