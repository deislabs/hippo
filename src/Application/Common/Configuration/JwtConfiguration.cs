namespace Hippo.Application.Common.Configuration;

public class JwtConfiguration
{
    public string AccessTokenSecret { get; set; } = "ceci n'est pas une jeton";

    public string RefreshTokenSecret { get; set; } = "ceci n'est pas une jeton";

    public double AccessTokenExpirationMinutes { get; set; } = 0.3;

    public double RefreshTokenExpirationMinutes { get; set; } = 60;

    public string Issuer { get; set; } = "localhost";

    public string Audience { get; set; } = "localhost";
}
