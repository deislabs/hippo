using Xunit;
using Hippo.Infrastructure.Services;

namespace Infrastructure.UnitTests.Services;

public class BindleServiceTests
{
    [Fact]
    public void SpinTomlShouldBeParsable()
    {
        var parcel = @"
[trigger]
type = 'http'
base = '/'

[config.object]
default = 'teapot'
required = false
secret = false

[[component]]
source = 'c3b7bfec9f12cf0e9e763fcaaa8140ef099577cf7c84130c9c9d66e85cf6d416'
id = 'spin_config_tinygo'

[component.trigger]
route = '/...'

[component.config]
message = '''I'm a {{object}}'''
";

        var spinToml = BindleService.ParseSpinToml(parcel);
    }
}
