namespace Hippo.Application.Common.Interfaces;

public interface ITokenService
{
    TokenInfo CreateSecurityToken(string id);
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
