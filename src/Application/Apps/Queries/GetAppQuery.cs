using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Hippo.Application.Apps.Extensions;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Queries;

public class GetAppQuery : IRequest<AppDto>
{
    [Required]
    public Guid Id { get; set; }
}

public class GetAppQueryHandler : IRequestHandler<GetAppQuery, AppDto>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetAppQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AppDto> Handle(GetAppQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Apps
            .Where(a => a.Id == request.Id)
            .Include(a => a.Channels)
            .Include(a => a.Revisions)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(App), request.Id);
        }

        return entity.ToAppDto();
    }
}
