namespace Hippo.Application.Identity;

public interface ICurrentUserService
{
    string? UserId { get; }
}
