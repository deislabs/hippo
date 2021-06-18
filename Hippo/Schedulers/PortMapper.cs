using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Hippo.Schedulers
{
    public class PortMapper
    {
        public const int EphemeralPortStartRange = 32768;
        public const int MaxPortNumber = 65535;
        private readonly int _start;
        private readonly int _end;
        public List<int> ReservedPorts { get; set; }

        public PortMapper(int start, int end)
        {
            if (end <= start)
            {
                throw new ArgumentException("ending range must be larger than starting range");
            }
            this._start = start;
            this._end = end;
        }
        public bool IsPortReserved(int port)
        {
            // NOTE(bacongobbler): quick shortcut: check if the port has already been allocated
            // before doing a network lookup (computationally cheaper)
            foreach (var reservedPort in ReservedPorts)
            {
                if (port == reservedPort)
                {
                    return true;
                }
            }
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnectionInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            var tcpListenersInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            return tcpConnectionInfoArray.Any(connection => connection.LocalEndPoint.Port == port) ||
                tcpListenersInfoArray.Any(connection => connection.Port == port);
        }

        public int ReservePort()
        {
            // NOTE(bacongobbler): the base assumption here is that we start from the ephemeral
            // port range and increment from there. If a port has already been allocated, we can
            // assume that it started from the base case.
            //
            // We are also assuming that there are not many ports reserved in the ephemeral port
            // range. This is an O(n+1) operation, where n is the number of ports reserved by
            // other programs.
            foreach (int port in Enumerable.Range(_start + ReservedPorts.Count(), _end))
            {
                if (!IsPortReserved(port))
                {
                    ReservedPorts.Append(port);
                    return port;
                }
            }
            throw new Exception("out of available ports");
        }

        public void FreePort(int port)
        {
            ReservedPorts.Remove(port);
        }
    }
}