using Xunit;
using Hippo.Infrastructure.Services;
using System.Collections.Generic;

namespace Hippo.Infrastructure.UnitTests.Services;

public class BindleServiceTests
{
    [Fact]
    public void SpinTomlShouldBeParsable()
    {
        var parcels = new List<string>{
            "[trigger]\ntype = 'http'\nbase = '/'\n\n[config.object]\ndefault = 'teapot'\nrequired = false\nsecret = false\n\n[[component]]\nsource = 'c3b7bfec9f12cf0e9e763fcaaa8140ef099577cf7c84130c9c9d66e85cf6d416'\nid = 'spin_config_tinygo'\n\n[component.trigger]\nroute = '/...'\n\n[component.config]\nmessage = '''I'm a {{object}}'''\n",
            "[trigger]\ntype = 'http'\nbase = '/'\n\n[[component]]\nsource = 'f2a469f6dddea6e53ac554d41eecad98358db280cfde5e44ea89f770682e9b4f'\nid = 'rust-outbound-http'\nallowedHttpHosts = ['https://some-random-api.ml']\n\n[component.trigger]\nroute = '/outbound'\n\n[[component]]\nsource = 'f2a469f6dddea6e53ac554d41eecad98358db280cfde5e44ea89f770682e9b4f'\nid = 'rust-outbound-http-wildcard'\nallowedHttpHosts = ['insecure:allow-all']\n\n[component.trigger]\nroute = '/wildcard'\n"
        };

        foreach (var parcel in parcels)
        {
            BindleService.ParseSpinToml(parcel);
        }
    }
}
