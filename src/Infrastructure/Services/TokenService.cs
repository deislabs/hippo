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

    private readonly IIdentityService _identityService;

    public TokenService(IConfiguration configuration, IIdentityService identityService)
    {
        _configuration = configuration;
        _identityService = identityService;
    }

    public TokenInfo CreateSecurityToken(string id)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, id),
            new Claim(JwtRegisteredClaimNames.UniqueName, id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, id)
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.UtcNow.AddMinutes(30),
            claims: claims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new TokenInfo(new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
    }
}
