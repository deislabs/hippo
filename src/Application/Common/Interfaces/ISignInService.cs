using Hippo.Application.Common.Models;
using MediatR;

namespace Hippo.Application.Common.Interfaces;

public interface ISignInService
{
    /// <summary>
    /// Attempts to sign in the current user using the provided password.
    /// </summary>
    Task<Result> PasswordSignInAsync(string userId, string password, bool rememberMe = false);

    /// <summary>
    /// Signs the current user out of the application.
    /// </summary>
    Task<Unit> SignOutAsync();
}
