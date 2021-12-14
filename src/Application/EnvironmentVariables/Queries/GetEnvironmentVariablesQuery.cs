using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.EnvironmentVariables.Queries;

public class GetEnvironmentVariablesQuery : IRequest<EnvironmentVariablesVm>
{
}

public class GetEnvironmentVariablesQueryHandler : IRequestHandler<GetEnvironmentVariablesQuery, EnvironmentVariablesVm>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetEnvironmentVariablesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EnvironmentVariablesVm> Handle(GetEnvironmentVariablesQuery request, CancellationToken cancellationToken)
    {
        return new EnvironmentVariablesVm
        {
            EnvironmentVariables = await _context.EnvironmentVariables
                .ProjectTo<EnvironmentVariableDto>(_mapper.ConfigurationProvider)
                .OrderBy(e => e.Key)
                .ToListAsync(cancellationToken)
        };
    }
}
