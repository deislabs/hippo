using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Hippo.Schedulers
{
    public class PortSniffer : IPortIsInUseChecker
    {
        public bool CheckPortIsInUse(int port)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnectionInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            var tcpListenersInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            return tcpConnectionInfoArray.Any(connection => connection.LocalEndPoint.Port == port) ||
                tcpListenersInfoArray.Any(connection => connection.Port == port);
        }
    }

    public class PortAllocator
    {
        public const int EphemeralPortStartRange = 32768;
        public const int MaxPortNumber = 65535;
        private readonly int _start;
        private readonly int _end;
        private readonly List<int> _reservedPorts;

        private IPortIsInUseChecker _portIsInUseChecker;

        public PortAllocator(int start, int end) : this(start, end, new PortSniffer()) {}

        public PortAllocator(int start, int end, IPortIsInUseChecker checker)
        {
            if (start <= 0)
            {
                throw new ArgumentException("start of range must be a positive integer");
            }
            if (start >= MaxPortNumber || end >= MaxPortNumber)
            {
                throw new ArgumentException("start and end of range must be below " + MaxPortNumber);
            }
            if (end <= start)
            {
                throw new ArgumentException("end of range must be larger than start of range");
            }
            this._start = start;
            this._end = end;
            this._portIsInUseChecker = checker;
            this._reservedPorts = new();
        }

        public bool IsPortReserved(int port)
        {
            // NOTE(bacongobbler): quick shortcut: check if the port has already been allocated
            // before doing a network lookup (computationally cheaper)
            if (_reservedPorts.Contains(port))
            {
                return true;
            }
            return _portIsInUseChecker.CheckPortIsInUse(port);
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
            foreach (int port in Enumerable.Range(_start + _reservedPorts.Count(), _end - _start))
            {
                if (!IsPortReserved(port))
                {
                    _reservedPorts.Add(port);
                    return port;
                }
            }
            throw new Exception("out of available ports");
        }

        public void FreePort(int port)
        {
            _reservedPorts.Remove(port);
        }
    }
}
