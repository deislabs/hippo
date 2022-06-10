using Hippo.Core.Enums;

namespace Hippo.Application.Common.Config;

public class HippoConfig
{
    public string PlatformDomain { get; set; } = "hippofactory.io";

    public RegistrationMode RegistrationMode { get; set; } = RegistrationMode.Open;

    public List<UserCredentials> Administrators { get; set; } = new List<UserCredentials>();
}

public class UserCredentials
{
    public string Username { get; set; } = String.Empty;

    public string Password { get; set; } = String.Empty;
}
