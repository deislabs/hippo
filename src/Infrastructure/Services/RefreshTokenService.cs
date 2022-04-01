using System.Security.Claims;
using Hippo.Application.Common.Configuration;
using Hippo.Application.Identity;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly JwtConfiguration jwtConfiguration;

    private readonly ITokenGenerator tokenGenerator;

    public RefreshTokenService(JwtConfiguration configuration, ITokenGenerator generator)
    {
        jwtConfiguration = configuration;
        tokenGenerator = generator;
    }

    public string Generate(string id)
    {
        return tokenGenerator.Generate
        (
            jwtConfiguration.RefreshTokenSecret,
            jwtConfiguration.Issuer,
            jwtConfiguration.Audience,
            jwtConfiguration.RefreshTokenExpirationMinutes,
            Array.Empty<Claim>()
        );
    }
}
