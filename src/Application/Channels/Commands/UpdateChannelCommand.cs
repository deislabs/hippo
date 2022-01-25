using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using MediatR;

namespace Hippo.Application.Channels.Commands;

public class UpdateChannelCommand : IRequest
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Domain { get; set; }

    public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

    public string? RangeRule { get; set; }

    public Guid? ActiveRevisionId { get; set; }

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
        var entity = await _context.Channels
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Channel), request.Id);
        }

        entity.Name = request.Name;
        entity.Domain = request.Domain;
        entity.RevisionSelectionStrategy = request.RevisionSelectionStrategy;
        entity.RangeRule = request.RangeRule;
        entity.ActiveRevisionId = request.ActiveRevisionId;
        entity.CertificateId = request.CertificateId;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
