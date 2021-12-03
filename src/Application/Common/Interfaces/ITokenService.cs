namespace Hippo.Application.Common.Interfaces;

public interface ITokenService
{
    TokenInfo CreateSecurityToken(string userId);
}

public class TokenInfo
{
    public string Token { get; }

    public DateTime Expiration { get; }

    public TokenInfo(string token, DateTime expiresIn)
    {
        Token = token;
        Expiration = expiresIn;
    }
}
