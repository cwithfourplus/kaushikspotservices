using System;

namespace Kaushik.Spot.Library
{
    /// <summary>
    /// List of Network Protocols that can be used
    /// </summary>
    public enum SpotSocketProvider
    {
        Tcp,
        Udp,
        IP
    }

    /// <summary>
    /// This factory class is used to get appropriate Network communication provider.
    /// </summary>
    public static class SpotSocketFactory
    {
        /// <summary>
        /// This factory method is used to get appropriate Network communication provider for server.
        /// </summary>
        /// <param name="provider">Network Protocol</param>
        /// <param name="port">Port on which socket will work, default 8080</param>
        /// <param name="bufferSize">Max buffer size, default 5120  (5 KB)</param>
        /// <param name="socketTimeout">Socket Timeout, default 50000000 (5 Sec)</param>
        /// <param name="backlog">Backlog, default 0</param>
        /// <param name="chunkSize">Size of chunk, default 500 bytes</param>
        /// <returns></returns>
        public static ISpotSocketServer GetServerProvider(SpotSocketProvider provider = SpotSocketProvider.Tcp, short port = 8080, short bufferSize = 5120, int socketTimeout = 50000000, short backlog = 0, short chunkSize = 500)
        {
            ISpotSocketServer spotSocketServer = null;

            switch (provider)
            {
                case SpotSocketProvider.Tcp:
                    spotSocketServer = new SpotTcpSocketServer(port, bufferSize, socketTimeout, backlog, chunkSize);
                    break;
                default:
                    throw new NotImplementedException("Requested Provider is not implememnted.");
            }

            return spotSocketServer;
        }

        /// <summary>
        /// This factory method is used to get appropriate Network communication provider for clent.
        /// </summary>
        /// <param name="provider">Network Protocol</param>
        /// <param name="hostNameOrAddress">IP Address or Name of host</param>
        /// <param name="port">Port on which socket will work, default 8080</param>
        /// <param name="bufferSize">Max buffer size, default 5120  (5 KB)</param>
        /// <param name="socketTimeout">Socket Timeout, default 50000000 (5 Sec)</param>
        /// <param name="backlog">Backlog, default 0</param>
        /// <param name="chunkSize">Size of chunk, default 500 bytes</param>
        /// <returns></returns>
        public static ISpotSocketClient GetClientProvider(string hostNameOrAddress, SpotSocketProvider provider = SpotSocketProvider.Tcp, short port = 8080, short bufferSize = 5120, int socketTimeout = 50000000, short chunkSize = 500)
        {
            ISpotSocketClient spotSocketClient = null;

            switch (provider)
            {
                case SpotSocketProvider.Tcp:
                    spotSocketClient = new SpotTcpSocketClient(hostNameOrAddress, port, bufferSize, socketTimeout, chunkSize);
                    break;
                default:
                    throw new NotImplementedException("Requested Provider is not implememnted.");
            }

            return spotSocketClient;
        }
    }
}