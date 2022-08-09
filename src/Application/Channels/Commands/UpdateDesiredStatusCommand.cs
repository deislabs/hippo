using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Commands;

public class UpdateDesiredStatusCommand : IRequest
{
    [Required]
    public Guid ChannelId { get; set; }

    [Required]
    public DesiredStatus DesiredStatus { get; set; }
}

public class UpdateDesiredStatusCommandHandler : IRequestHandler<UpdateDesiredStatusCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateDesiredStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateDesiredStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.Channels
            .Include(c => c.ActiveRevision)
            .Include(c => c.EnvironmentVariables)
            .Include(c => c.App)
            .FirstOrDefault(c => c.Id == request.ChannelId);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Channel), request.ChannelId);
        }

        entity.DesiredStatus = request.DesiredStatus;

        entity.AddDomainEvent(new ModifiedEvent<Channel>(entity));

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
