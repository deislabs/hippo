using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Queries;

public class ExportChannelsQuery : IRequest<ExportChannelsVm>
{
}

public class ExportChannelsQueryHandler : IRequestHandler<ExportChannelsQuery, ExportChannelsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IJsonFileBuilder _fileBuilder;

    public ExportChannelsQueryHandler(IApplicationDbContext context, IMapper mapper, IJsonFileBuilder fileBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileBuilder = fileBuilder;
    }

    public async Task<ExportChannelsVm> Handle(ExportChannelsQuery request, CancellationToken cancellationToken)
    {
        var records = await _context.Channels
            .ProjectTo<ChannelRecord>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var vm = new ExportChannelsVm(
                "channels.json",
                "application/json",
                _fileBuilder.BuildChannelsFile(records));
        return vm;
    }
}
