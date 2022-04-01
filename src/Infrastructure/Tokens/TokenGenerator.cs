using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hippo.Application.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Hippo.Infrastructure.Tokens;

public class TokenGenerator : ITokenGenerator
{
    public string Generate(string secretKey, string issuer, string audience, double expires, IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken securityToken = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(expires),
            credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}
