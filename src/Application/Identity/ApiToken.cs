namespace Hippo.Application.Identity;

public class ApiToken
{
    public string AccessToken { get; }

    public string RefreshToken { get; }

    public ApiToken(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}
