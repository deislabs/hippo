namespace Hippo.Application.Identity;

public interface IAuthenticateService
{
    Task<ApiToken> Authenticate(Account account, CancellationToken cancellationToken);
}
