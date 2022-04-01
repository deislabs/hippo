namespace Hippo.Core.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }

    public string UserId { get; set; }

    public string Token { get; set; }

    public RefreshToken(string id, string token)
    {
        UserId = id;
        Token = token;
    }
}
