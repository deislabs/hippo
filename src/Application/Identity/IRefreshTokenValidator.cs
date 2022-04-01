namespace Hippo.Application.Identity;

public interface IRefreshTokenValidator
{
    bool Validate(string refreshToken);
}
