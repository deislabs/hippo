namespace Hippo.Application.Identity;

public interface ITokenService
{
    string Generate(string id);
}

public interface IAccessTokenService : ITokenService { }
public interface IRefreshTokenService : ITokenService { }
