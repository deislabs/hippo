using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.EnvironmentVariables.Queries;

public class GetEnvironmentVariableQuery : IRequest<EnvironmentVariableDto>
{
    [Required]
    public Guid Id { get; set; }
}

public class GetEnvironmentVariableQueryHandler : IRequestHandler<GetEnvironmentVariableQuery, EnvironmentVariableDto>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetEnvironmentVariableQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EnvironmentVariableDto> Handle(GetEnvironmentVariableQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.EnvironmentVariables
            .Where(a => a.Id == request.Id)
            .ProjectTo<EnvironmentVariableDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(EnvironmentVariable), request.Id);
        }

        return entity;
    }
}
