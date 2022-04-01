using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Hippo.Application.Common.Configuration;
using Hippo.Application.Identity;

namespace Hippo.Infrastructure.Services;

public class AccessTokenService : IAccessTokenService
{
    private readonly JwtConfiguration jwtConfiguration;

    private readonly ITokenGenerator tokenGenerator;

    public AccessTokenService(JwtConfiguration configuration, ITokenGenerator generator)
    {
        jwtConfiguration = configuration;
        tokenGenerator = generator;
    }

    public string Generate(string id)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, id),
            new Claim(JwtRegisteredClaimNames.UniqueName, id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, id)
        };

        return tokenGenerator.Generate
        (
            jwtConfiguration.AccessTokenSecret,
            jwtConfiguration.Issuer,
            jwtConfiguration.Audience,
            jwtConfiguration.AccessTokenExpirationMinutes,
            claims
        );
    }
}
