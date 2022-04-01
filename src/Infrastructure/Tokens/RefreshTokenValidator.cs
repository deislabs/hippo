using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Hippo.Application.Common.Configuration;
using Hippo.Application.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Hippo.Infrastructure.Tokens;

public class RefreshTokenValidator : IRefreshTokenValidator
{
    private readonly JwtConfiguration jwtConfiguration;

    public RefreshTokenValidator(JwtConfiguration configuration) => jwtConfiguration = configuration;

    public bool Validate(string refreshToken)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfiguration.Issuer,
            ValidAudience = jwtConfiguration.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.RefreshTokenSecret)),
            ClockSkew = TimeSpan.Zero
        };

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        try
        {
            jwtSecurityTokenHandler.ValidateToken(refreshToken, validationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
