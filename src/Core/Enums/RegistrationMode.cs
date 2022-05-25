namespace Hippo.Core.Enums;

/// <summary>
/// The mode to use to when registering new accounts
/// </summary>
public enum RegistrationMode
{
    /// <summary>
    /// Allows account registration IFF you are logged in as an administrator.
    /// </summary>
    AdministratorOnly,
    /// <summary>
    /// Disables account registration.
    /// </summary>
    Closed,
    /// <summary>
    /// Enables open account registration (anyone with the URL can register an account). This is the default.
    /// </summary>
    Open
}
