using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;

namespace Hippo.Application.Apps.Commands;

public class UpdateAppCommand : IRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string StorageId { get; set; } = "";
}

public class UpdateAppCommandHandler : IRequestHandler<UpdateAppCommand>
{
    private readonly IApplicationDbContext _context;

    private readonly IJobScheduler _jobScheduler;

    public UpdateAppCommandHandler(IApplicationDbContext context, IJobScheduler jobScheduler)
    {
        _context = context;
        _jobScheduler = jobScheduler;
    }

    public async Task<Unit> Handle(UpdateAppCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Apps
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(App), request.Id);
        }

        // if the user changes the bindle storage id, ALL channels will stop working until the user registers new revisions that satisfy the channels' rules.
        //
        // TODO: how do we want to handle channels that requested ChannelRevisionSelectionStrategy.UseSpecifiedRevision?
        if (entity.StorageId != request.StorageId)
        {
            foreach (var channel in entity.Channels)
            {
                _jobScheduler.Stop(channel);
            }
        }

        entity.Name = request.Name;
        entity.StorageId = request.StorageId;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
