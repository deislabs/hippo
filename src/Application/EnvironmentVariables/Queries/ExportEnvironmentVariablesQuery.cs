using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.EnvironmentVariables.Queries;

public class ExportEnvironmentVariablesQuery : IRequest<ExportEnvironmentVariablesVm>
{
}

public class ExportEnvironmentVariablesQueryHandler : IRequestHandler<ExportEnvironmentVariablesQuery, ExportEnvironmentVariablesVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IJsonFileBuilder _fileBuilder;

    public ExportEnvironmentVariablesQueryHandler(IApplicationDbContext context, IMapper mapper, IJsonFileBuilder fileBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileBuilder = fileBuilder;
    }

    public async Task<ExportEnvironmentVariablesVm> Handle(ExportEnvironmentVariablesQuery request, CancellationToken cancellationToken)
    {
        var records = await _context.EnvironmentVariables
            .ProjectTo<EnvironmentVariableRecord>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var vm = new ExportEnvironmentVariablesVm(
                "environment-variables.json",
                "application/json",
                _fileBuilder.BuildEnvironmentVariablesFile(records));

        return vm;
    }
}
