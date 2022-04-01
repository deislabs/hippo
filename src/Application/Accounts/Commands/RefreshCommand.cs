using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Accounts.Commands;

public class RefreshCommand : IRequest<ApiToken>
{
    [Required]
    public string RefreshToken { get; set; } = "";
}

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, ApiToken>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IAuthenticateService _authenticateService;
    private readonly IIdentityService _identityService;
    private readonly IRefreshTokenValidator _refreshTokenValidator;

    public RefreshCommandHandler(IAuthenticateService authenticateService, IRefreshTokenValidator refreshTokenValidator, IIdentityService identityService, IApplicationDbContext applicationDbContext)
    {
        _authenticateService = authenticateService;
        _refreshTokenValidator = refreshTokenValidator;
        _identityService = identityService;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<ApiToken> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var isValidRefreshToken = _refreshTokenValidator.Validate(request.RefreshToken);

        if (!isValidRefreshToken)
        {
            throw new InvalidRefreshTokenException();
        }

        var refreshToken = await _applicationDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (refreshToken is null)
        {
            throw new InvalidRefreshTokenException();
        }

        _applicationDbContext.RefreshTokens.Remove(refreshToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        var user = await _identityService.FindByIdAsync(refreshToken.UserId);

        if (user is null)
        {
            throw new NotFoundException(nameof(Account), refreshToken.UserId);
        }

        return await _authenticateService.Authenticate(user, cancellationToken);
    }
}
