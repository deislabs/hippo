using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hippo.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Hippo.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenInfo CreateSecurityToken(string id)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            // jti - unique string that is representative of each token so using a guid
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.Now.AddDays(90),
            claims: claims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new TokenInfo(new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
    }
}
