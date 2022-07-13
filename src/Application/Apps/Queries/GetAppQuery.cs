using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Hippo.Application.Apps.Extensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Queries;

public class GetAppQuery : IRequest<AppItem>
{
    [Required]
    public Guid Id { get; set; }
}

public class GetAppQueryHandler : IRequestHandler<GetAppQuery, AppItem>
{
    private readonly IApplicationDbContext _context;

    private readonly ICurrentUserService _currentUserService;

    private readonly IMapper _mapper;

    public GetAppQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<AppItem> Handle(GetAppQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Apps
            .Where(a => a.Id == request.Id)
            .Where(a => a.CreatedBy == _currentUserService.UserId)
            .Include(a => a.Channels)
            .Include(a => a.Revisions)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(App), request.Id);
        }

        return entity.ToAppItem();
    }
}
