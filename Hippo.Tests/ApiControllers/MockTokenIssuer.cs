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
    }
}