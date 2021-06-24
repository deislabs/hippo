using System;
using Microsoft.Extensions.Hosting;

namespace Hippo.Services
{
    public interface ITraefikService
    {
        void StartProxy(string name, Uri hostname, Uri proxyUrl);
        void StopProxy(string name);
    }
}
