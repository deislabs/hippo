using Deislabs.Bindle;
using Hippo.Application.Common.Interfaces;

namespace Hippo.Infrastructure.Services;

public class BindleService : IBindleService
{
    private BindleClient _client { get; set; }

    public BindleService()
    {
        _client = new BindleClient(configuration.GetValue<string>("Bindle:Url", "http://127.0.0.1:8080/v1"));
    }
}
