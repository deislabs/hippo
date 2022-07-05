using System;
using Hippo.Core.Enums;
using Xunit;

namespace Hippo.Core.UnitTests.Enums;

public class RegistrationModeTests
{
    /// <summary>
    /// The underlying values here are contractual with the database.
    /// **DO NOT** change the underlying numeric value of any case.
    /// </summary>
    [Fact]
    public void ShouldReturnCorrectRegistrationMode()
    {
        Assert.Equal(0, (Convert.ToInt32(RegistrationMode.AdministratorOnly)));
        Assert.Equal(1, (Convert.ToInt32(RegistrationMode.Closed)));
        Assert.Equal(2, (Convert.ToInt32(RegistrationMode.Open)));

        Assert.Equal("AdministratorOnly", RegistrationMode.AdministratorOnly.ToString());
        Assert.Equal("Closed", RegistrationMode.Closed.ToString());
        Assert.Equal("Open", RegistrationMode.Open.ToString());
    }
}
