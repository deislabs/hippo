using Hippo.Core.Entities;

namespace Hippo.Application.Common.Interfaces;

public interface IReverseProxy
{
    void Start(Channel c, string address);

    void Stop(Channel c);
}
