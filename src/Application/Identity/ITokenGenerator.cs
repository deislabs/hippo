using System.Security.Claims;

namespace Hippo.Application.Identity;

public interface ITokenGenerator
{
    string Generate(string secretKey, string issuer, string audience, double expires, IEnumerable<Claim> claims);
}
