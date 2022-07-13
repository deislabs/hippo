using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using MediatR;

namespace Hippo.Application.Accounts.Commands;

public class AddPasswordCommand : IRequest
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AddPasswordCommandHandler : IRequestHandler<AddPasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public AddPasswordCommandHandler(IApplicationDbContext context, IIdentityService identityService, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<Unit> Handle(AddPasswordCommand request, CancellationToken cancellationToken)
    {
        var id = await _identityService.GetUserIdAsync(request.UserName, cancellationToken);

        // NOTE: This prevents malicious users from adding password auth to another user's account.
        if (string.IsNullOrEmpty(_currentUserService.UserId))
            throw new UnauthorizedAccessException();
        if (_currentUserService.UserId != await _identityService.GetUserIdAsync(request.UserName, cancellationToken)
            && !await _identityService.IsInRoleAsync(_currentUserService.UserId, UserRole.Administrator, cancellationToken))
            throw new ForbiddenAccessException();

        var result = await _identityService.AddPasswordAsync(id, request.Password, cancellationToken);
        if (!result.Succeeded)
            throw new AccountException(result.Errors);
        return Unit.Value;
    }
}
