using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using MediatR;

namespace Hippo.Application.Accounts.Commands;

public class ChangePasswordCommand : IRequest
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public ChangePasswordCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var id = await _identityService.GetUserIdAsync(request.UserName, cancellationToken);

        if (string.IsNullOrEmpty(_currentUserService.UserId))
            throw new UnauthorizedAccessException();
        if (_currentUserService.UserId != await _identityService.GetUserIdAsync(request.UserName, cancellationToken)
            && !await _identityService.IsInRoleAsync(_currentUserService.UserId, UserRole.Administrator, cancellationToken))
            throw new ForbiddenAccessException();

        var result = await _identityService.ChangePasswordAsync(id, request.Password, request.NewPassword, cancellationToken);
        if (!result.Succeeded)
            throw new AccountException(result.Errors);
        return Unit.Value;
    }
}
