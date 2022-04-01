using Hippo.Application.Common.Interfaces;
using Hippo.Application.Identity;
using Hippo.Core.Entities;

namespace Hippo.Infrastructure.Identity;

public class AuthenticateService : IAuthenticateService
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public AuthenticateService(IAccessTokenService accessTokenService, IRefreshTokenService refreshTokenService, IIdentityService identityService, IApplicationDbContext context)
    {
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _identityService = identityService;
        _context = context;
    }


    public async Task<ApiToken> Authenticate(Account user, CancellationToken cancellationToken)
    {
        var refreshToken = _refreshTokenService.Generate(user.Id);
        await _context.RefreshTokens.AddAsync(new RefreshToken(user.Id, refreshToken), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new ApiToken(_accessTokenService.Generate(user.Id), refreshToken);
    }
}
