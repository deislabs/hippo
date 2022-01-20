using System.Threading.Tasks;
using Hippo.Application.Revisions.Commands;
using Hippo.Application.Common.Exceptions;
using Xunit;
using Hippo.Application.Certificates.Commands;

namespace Hippo.FunctionalTests.Application.Certificates.Commands;

public class CreateCertificateTests : TestBase
{
    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateCertificateCommand()));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateCertificateCommand { Name = "testcert" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateCertificateCommand { PublicKey = "testcert" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateCertificateCommand { PrivateKey = "testcert" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateCertificateCommand { Name = "testcert", PublicKey = "testcert" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateCertificateCommand { Name = "testcert", PrivateKey = "testcert" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateCertificateCommand { PublicKey = "testcert", PrivateKey = "testcert" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateCertificateCommand { Name = "", PublicKey = "testcert", PrivateKey = "testcert" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateCertificateCommand { Name = "testcert", PublicKey = "", PrivateKey = "testcert" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateCertificateCommand { Name = "testcert", PublicKey = "testcert", PrivateKey = "" }));
    }

    [Fact]
    public async Task ShouldRequireUniqueName()
    {
        await SendAsync(new CreateCertificateCommand
        {
            Name = "testuniquename",
            PublicKey = "testuniquename",
            PrivateKey = "testuniquename"
        });

        var command = new CreateCertificateCommand
        {
            Name = "testuniquename",
            PublicKey = "testuniquename",
            PrivateKey = "testuniquename"
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
