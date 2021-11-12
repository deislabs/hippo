using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;

namespace Hippo.Proxies
{
    public interface IReverseProxy
    {
        void StartProxy(Channel channel, string address);
        void StopProxy(Channel channel);
    }
}
