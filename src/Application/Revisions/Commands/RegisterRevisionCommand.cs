using AutoMapper;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Revisions.Queries;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Revisions.Commands;

public class RegisterRevisionCommand : IRequest<RevisionsVm>
{
    // NOTE: We should rename this to AppName, but that will require an update to
    // the Hippo API client libraries (and I'm lazy so...)
    public string? AppStorageId { get; set; }

    public string? RevisionNumber { get; set; }
}

public class RegisterRevisionCommandHandler : IRequestHandler<RegisterRevisionCommand, RevisionsVm>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public RegisterRevisionCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<RevisionsVm> Handle(RegisterRevisionCommand request, CancellationToken cancellationToken)
    {
        RevisionsVm viewModel = new RevisionsVm();

        foreach (App app in _context.Apps.Where(a => a.Name == request.AppStorageId))
        {
            var entity = new Revision
            {
                AppId = app.Id,
                RevisionNumber = request.RevisionNumber
            };

            entity.DomainEvents.Add(new RevisionCreatedEvent(entity));

            _context.Revisions.Add(entity);

            viewModel.Revisions.Add(_mapper.Map<RevisionDto>(entity));
        }

        await _context.SaveChangesAsync(cancellationToken);

        return viewModel;
    }
}
