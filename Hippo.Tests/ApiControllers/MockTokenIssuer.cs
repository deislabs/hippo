using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Hippo.Tests.ApiControllers
{
    public class MockTokenIssuer
    {
        public readonly string Issuer;
        public readonly string Audience;
        public readonly SecurityKey SecurityKey;
        private readonly Claim[] _claims;

        public MockTokenIssuer()
        {
            Issuer = Guid.NewGuid().ToString();
            Audience = Guid.NewGuid().ToString();
            var key = new byte[32];
            RandomNumberGenerator.Create().GetBytes(key);
            SecurityKey = new SymmetricSecurityKey(key)
            {
                KeyId = Guid.NewGuid().ToString()
            };
            _claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "user@test.com"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, "user")
            };
        }

        public string GetToken(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(
              new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials:
                  new SigningCredentials(
                    key: SecurityKey,
                    algorithm: SecurityAlgorithms.HmacSha256)));
        }

        public string GetToken()
        {
            return GetToken(_claims);
        }
    }
}
