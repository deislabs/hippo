using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Commands;

public class UpdateChannelCommand : IRequest
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string? Domain { get; set; }

    [Required]
    public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

    public string? RangeRule { get; set; }

    public Guid? ActiveRevisionId { get; set; }

    public DateTime LastPublishDate { get; set; }

    public Guid? CertificateId { get; set; }
}

public class UpdateChannelCommandHandler : IRequestHandler<UpdateChannelCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateChannelCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.Channels
            .Include(c => c.ActiveRevision)
            .Include(c => c.App)
            .First(c => c.Id == request.Id);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Channel), request.Id);
        }

        entity.Name = request.Name;
        entity.Domain = request.Domain;
        entity.RevisionSelectionStrategy = request.RevisionSelectionStrategy;
        entity.RangeRule = request.RangeRule;
        entity.ActiveRevisionId = request.ActiveRevisionId;
        entity.CertificateId = request.CertificateId;

        if (entity.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseRangeRule && entity.RangeRule is null)
        {
            entity.RangeRule = "*";
        }

        entity.AddDomainEvent(new ModifiedEvent<Channel>(entity));

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
