using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;

namespace Kaushik.Spot.Library
{
    #region Class SpotTcpSocket
    /// <summary>
    /// This is a base class that helps in TCP based Network communication
    /// </summary>
    public class SpotTcpSocket : ISpotSocket, IDisposable
    {
        #region Protected Member Variables
        protected readonly short chunkSize = 500;
        protected readonly short port;
        protected readonly short bufferSize;
        protected readonly short backlog;
        protected readonly int socketTimeout;
        protected Socket socket;
        protected NetworkStream networkStream;
        #endregion Protected Member Variables

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">Port on which socket will work, default 8080</param>
        /// <param name="bufferSize">Max buffer size, default 5120  (5 KB)</param>
        /// <param name="socketTimeout">Socket Timeout, default 50000000 (5 Sec)</param>
        /// <param name="backlog">Backlog, default 0</param>
        /// <param name="chunkSize">Size of chunk, default 500 bytes</param>
        public SpotTcpSocket(short port = 8080, short bufferSize = 5120, int socketTimeout = 50000000, short backlog = 0, short chunkSize = 500)
        {
            this.port = port;
            this.bufferSize = bufferSize;
            this.socketTimeout = socketTimeout;
            this.backlog = backlog;
            this.chunkSize = chunkSize;
        }
        #endregion Constructor

        #region Send Functions
        /// <summary>
        /// This function sends string data over network
        /// </summary>
        /// <param name="data">String data</param>
        public void Send(string data)
        {
            Send(data, 0, data.Length);
        }

        /// <summary>
        /// This function sends string data over network starting from offset till the size given
        /// </summary>
        /// <param name="data">String data</param>
        /// <param name="offset">Start position in string</param>
        /// <param name="size">Total size to be sent</param>
        public void Send(string data, int offset, int size)
        {
            int totalSize = offset + size;//The size of buffer can be more but this is bare minimum
            long startTickCount = DateTime.Now.Ticks;

            //Send Data Size that will be sent
            send(Utility.ConvertIntToByteArray(size), 0, Utility.SizeBytesCount, this.socket, this.networkStream, this.socketTimeout);

            for (int partOffset = offset; partOffset < totalSize; partOffset += this.chunkSize)
            {
                if (DateTime.Now.Ticks > startTickCount + this.socketTimeout
                    && this.socketTimeout != 0)
                {
                    throw new SocketException(SocketError.TimedOut);
                }

                //Send Data in chunks
                send(Encoding.UTF8.GetBytes(data), partOffset, totalSize - partOffset > this.chunkSize ? this.chunkSize : size - partOffset, this.socket, this.networkStream, this.socketTimeout);
            }
        }

        private static void send(byte[] buffer, int offset, int size, Socket socket, NetworkStream networkStream, int socketTimeout)
        {
            long startTickCount = DateTime.Now.Ticks;
            int sent = 0;  // how many bytes is already sent

            networkStream.Flush();

            do
            {
                if (DateTime.Now.Ticks > startTickCount + socketTimeout
                    && socketTimeout != 0)
                {
                    throw new SocketException(SocketError.TimedOut);
                }

                try
                {
                    sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == (int)SocketError.WouldBlock ||
                        ex.ErrorCode == (int)SocketError.InProgress ||
                        ex.ErrorCode == (int)SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably full, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                    {
                        throw ex;  // any serious error occurr
                    }
                }
            } while (sent < size);

            networkStream.Flush();
        }
        #endregion Send Functions

        #region Receive Functions
        /// <summary>
        /// This function is used to recive data from network
        /// </summary>
        /// <param name="data">String data to be set</param>
        /// <returns>Length of bytes received</returns>
        public int Receive(out string data)
        {
            return Receive(out data, 0, 9999);
        }

        /// <summary>
        /// This function is used to recive data from network starting from offset till end of string
        /// </summary>
        /// <param name="data">String data to be set</param>
        /// <param name="offset"></param>
        /// <returns>Length of bytes received</returns>
        public int Receive(out string data, int offset)
        {
            return Receive(out data, offset, 9999);
        }

        /// <summary>
        /// This function is used to recive data from network starting from offset till the size given
        /// </summary>
        /// <param name="data">String data to be set</param>
        /// <param name="offset">Start position in string</param>
        /// <param name="size">Total size in bytes to be received</param>
        /// <returns>Length of bytes received</returns>
        public int Receive(out string data, int offset, int size)
        {
            byte[] buffer = new byte[Utility.SizeBytesCount];

            //get data size that will be comming
            receive(buffer, 0, Utility.SizeBytesCount, this.socket, this.networkStream, this.socketTimeout);
            int incommingSize = Convert.ToInt32(new String(Encoding.UTF8.GetChars(buffer)));


            buffer = new byte[incommingSize];
            int received = receive(buffer, offset, size > incommingSize ? incommingSize : size, this.socket, this.networkStream, this.socketTimeout);
            data = new String(Encoding.UTF8.GetChars(buffer)).Trim('\0');
            return received;

        }

        private static int receive(byte[] buffer, int offset, int size, Socket socket, NetworkStream networkStream, int socketTimeout)
        {
            long startTickCount = DateTime.Now.Ticks;
            int received = 0;  // how many bytes is already received

            networkStream.Flush();

            do
            {
                if (DateTime.Now.Ticks > startTickCount + socketTimeout
                    && socketTimeout != 0)
                {
                    throw new SocketException(SocketError.TimedOut);
                }

                try
                {
                    received += socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == (int)SocketError.WouldBlock ||
                        ex.ErrorCode == (int)SocketError.InProgress ||
                        ex.ErrorCode == (int)SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably empty, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                    {
                        throw ex;  // any serious error occurr
                    }
                }
            } while (received < size);

            networkStream.Flush();

            return received;
        }
        #endregion Receive Functions

        #region Helper Functions
        /// <summary>
        /// This function closes and disposes Network Stream
        /// </summary>
        protected void cleanStream()
        {
            if (this.networkStream != null)
            {
                try
                {
                    this.networkStream.Close();
                    this.networkStream.Dispose();
                }
                catch
                {
                    //Well there are very less options here to check if it was open or not
                }

                this.networkStream = null;
            }
        }

        /// <summary>
        /// This function closes and disposes Socket
        /// </summary>
        /// <param name="socket"></param>
        protected void cleanSocket(Socket socket)
        {
            if (socket != null)
            {
                try
                {
                    socket.Close();
                }
                catch
                {
                    //Well there are very less options here to check if it was open or not
                }
                socket = null;
            }
        }
        #endregion Helper Functions

        #region Dispose
        /// <summary>
        /// Dispose SpotTcpSocket
        /// </summary>
        public void Dispose()
        {
            cleanStream();
            cleanSocket(this.socket);
        }
        #endregion Dispose
    }
    #endregion Class SpotTcpSocket

    #region Class SpotTcpSocketServer
    /// <summary>
    /// This class is to be used by Server program for TCP based Network communication
    /// </summary>
    public class SpotTcpSocketServer : SpotTcpSocket, ISpotSocketServer
    {
        #region Protected Member Variables
        protected Socket listener;
        #endregion Protected Member Variables

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">Port on which socket will work, default 8080</param>
        /// <param name="bufferSize">Max buffer size, default 5120  (5 KB)</param>
        /// <param name="socketTimeout">Socket Timeout, default 50000000 (5 Sec)</param>
        /// <param name="backlog">Backlog, default 0</param>
        /// <param name="chunkSize">Size of chunk, default 500 bytes</param>
        public SpotTcpSocketServer(short port = 8080, short bufferSize = 5120, int socketTimeout = 50000000, short backlog = 0, short chunkSize = 500)
            : base(port, bufferSize, socketTimeout, backlog, chunkSize)
        {

        }
        #endregion Constructor

        #region CreateListner Functions
        /// <summary>
        /// This function is used to create a Listner socket
        /// </summary>
        /// <returns>Listner socket</returns>
        public Socket CreatetListner()
        {
            cleanStream();
            cleanSocket(this.socket);
            cleanSocket(this.listener);

            this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.listener.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

            this.listener.Bind(new IPEndPoint(IPAddress.Any, this.port));
            this.listener.Listen(this.backlog);

            return this.listener;
        }
        #endregion CreateListner Functions

        #region Accept
        /// <summary>
        /// This function waits for incomming socket call and once there is one it returns that socket.
        /// </summary>
        /// <returns>Communicating Socket</returns>
        public Socket Accept()
        {
            cleanStream();
            cleanSocket(this.socket);

            this.socket = this.listener.Accept();
            this.networkStream = new NetworkStream(this.socket);

            return this.socket;
        }
        #endregion Accept

        #region Dispose
        /// <summary>
        /// Dispose SpotTcpSocketServer
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            cleanSocket(this.listener);
        }
        #endregion Dispose
    }
    #endregion Class SpotTcpSocketServer

    #region Class SpotTcpSocketClient
    /// <summary>
    /// This class is to be used by Client program for TCP based Network communication
    /// </summary>
    public class SpotTcpSocketClient : SpotTcpSocket, ISpotSocketClient
    {
        #region Protected Member Variables
        protected readonly string hostNameOrAddress;
        protected IPEndPoint serverEndPoint;
        #endregion Protected Member Variables

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostNameOrAddress">IP Address or Name of host</param>
        /// <param name="port">Port on which socket will work, default 8080</param>
        /// <param name="bufferSize">Max buffer size, default 5120  (5 KB)</param>
        /// <param name="socketTimeout">Socket Timeout, default 50000000 (5 Sec)</param>
        /// <param name="backlog">Backlog, default 0</param>
        /// <param name="chunkSize">Size of chunk, default 500 bytes</param>
        public SpotTcpSocketClient(string hostNameOrAddress, short port = 8080, short bufferSize = 5120, int socketTimeout = 50000000, short chunkSize = 500)
            : base(port, bufferSize, socketTimeout, 0, chunkSize)
        {
            this.hostNameOrAddress = hostNameOrAddress;
        }
        #endregion Constructor

        #region CreateClient Functions
        /// <summary>
        /// This function creates a client socket
        /// </summary>
        /// <returns>Client Socket</returns>
        public Socket CreateClient()
        {
            cleanStream();
            cleanSocket(this.socket);

            if (this.serverEndPoint == null)
            {
                //GetHostEntry does not work as Netduino does not have a name I guess.
                IPHostEntry host = Dns.GetHostEntry(this.hostNameOrAddress);
                IPAddress address = host.AddressList[0];
                this.serverEndPoint = new IPEndPoint(address, this.port);
            }
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this.socket.Connect(this.serverEndPoint);

            this.networkStream = new NetworkStream(this.socket);

            return this.socket;
        }
        #endregion CreateClient Functions
    }
    #endregion Class SpotTcpSocketClient
}
