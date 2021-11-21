using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Kaushik.Spot.Library
{
    public enum SpotSocketOperation
    {
        None,
        SendSize,
        ReceiveSize,
        ReceiveSizeComplete,
        SendChunk,
        ReceiveChunk,
        Close,
        Error
    }
    public class Data
    {
        public byte[] Buffer { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }
        public SpotSocketOperation NextOperation { get; set; }
    }

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
        protected DnsEndPoint serverEndPoint;
        protected ManualResetEvent operationComplete;
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
            this.operationComplete = new ManualResetEvent(false);
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

            send(Encoding.UTF8.GetBytes(data), offset, size);

        }

        private void send(byte[] buffer, int offset, int size)
        {
            this.operationComplete.Reset();

            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(socketEventArgCompleted);
            socketEventArg.RemoteEndPoint = this.serverEndPoint;
            Data data = new Data() { Buffer = buffer, Offset = offset, Size = size, NextOperation = SpotSocketOperation.SendSize };
            socketEventArg.UserToken = data;

            this.socket.ConnectAsync(socketEventArg);

            this.operationComplete.WaitOne();

            if (socketEventArg.SocketError != SocketError.Success)
            {
                throw new SocketException((int)socketEventArg.SocketError);
            }
        }

        private void sendSize(SocketAsyncEventArgs socketEventArg)
        {
            Data data = (Data)socketEventArg.UserToken;

            if (socketEventArg.SocketError == SocketError.Success)
            {
                data.NextOperation = SpotSocketOperation.SendChunk;
                socketEventArg.SetBuffer(Utility.ConvertIntToByteArray(data.Size), 0, Utility.SizeBytesCount);

                this.socket.SendAsync(socketEventArg);
            }
            else
            {
                data.NextOperation = SpotSocketOperation.Error;
                socketEventArgCompleted(null, socketEventArg);
            }
        }

        private void sendChunk(SocketAsyncEventArgs socketEventArg)
        {
            Data data = (Data)socketEventArg.UserToken;

            if (socketEventArg.SocketError == SocketError.Success)
            {
                socketEventArg.SetBuffer(data.Buffer, data.Offset, data.Size < this.chunkSize ? data.Size : this.chunkSize);

                if (data.Size < this.chunkSize)
                {
                    data.NextOperation = SpotSocketOperation.None;
                }
                else
                {
                    data.NextOperation = SpotSocketOperation.SendChunk;
                    data.Size -= this.chunkSize;
                    data.Offset += this.chunkSize;
                }

                this.socket.SendAsync(socketEventArg);
            }
            else
            {
                data.NextOperation = SpotSocketOperation.Error;
                socketEventArgCompleted(null, socketEventArg);
            }

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
            byte[] buffer = null;
            int received = receive(out buffer, offset, size);
            data = new String(Encoding.UTF8.GetChars(buffer)).Trim('\0');
            return received;
        }

        private int receive(out byte[] buffer, int offset, int size)
        {
            this.operationComplete.Reset();

            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(socketEventArgCompleted);
            socketEventArg.RemoteEndPoint = this.serverEndPoint;
            socketEventArg.SetBuffer(new byte[Utility.SizeBytesCount], 0, Utility.SizeBytesCount);
            Data data = new Data() { Buffer = new byte[Utility.SizeBytesCount], Offset = 0, Size = Utility.SizeBytesCount, NextOperation = SpotSocketOperation.ReceiveSize };
            socketEventArg.UserToken = data;

            if (!this.socket.Connected)
            {
                this.socket.ConnectAsync(socketEventArg);
            }
            else
            {
                receiveSize(socketEventArg);
            }

            this.operationComplete.WaitOne();

            if (socketEventArg.SocketError != SocketError.Success)
            {
                throw new SocketException((int)socketEventArg.SocketError);
            }

            buffer = data.Buffer;
            return data.Buffer.Length;
        }

        private void receiveSize(SocketAsyncEventArgs socketEventArg)
        {
            Data data = (Data)socketEventArg.UserToken;

            if (socketEventArg.SocketError == SocketError.Success)
            {
                data.NextOperation = SpotSocketOperation.ReceiveSizeComplete;

                this.socket.ReceiveAsync(socketEventArg);
            }
            else
            {
                data.NextOperation = SpotSocketOperation.Error;
                socketEventArgCompleted(null, socketEventArg);
            }
        }

        private void receiveSizeComplete(SocketAsyncEventArgs socketEventArg)
        {
            Data data = (Data)socketEventArg.UserToken;

            if (socketEventArg.SocketError == SocketError.Success)
            {
                data.NextOperation = SpotSocketOperation.ReceiveChunk;

                data.Size = Convert.ToInt32(new String(Encoding.UTF8.GetChars(socketEventArg.Buffer)));
                data.Buffer = new byte[data.Size];
                data.Offset = 0;
                socketEventArg.SetBuffer(data.Buffer, 0, data.Size);
                this.socket.ReceiveAsync(socketEventArg);
            }
            else
            {
                data.NextOperation = SpotSocketOperation.Error;
                socketEventArgCompleted(null, socketEventArg);
            }
        }

        private void receiveChunk(SocketAsyncEventArgs socketEventArg)
        {
            Data data = (Data)socketEventArg.UserToken;

            if (socketEventArg.SocketError == SocketError.Success)
            {
                socketEventArg.Buffer.CopyTo(data.Buffer, data.Offset);
                data.Offset += socketEventArg.Buffer.Length;

                if (data.Size > data.Offset)
                {
                    data.NextOperation = SpotSocketOperation.ReceiveChunk;

                    this.socket.ReceiveAsync(socketEventArg);
                }
                else
                {
                    data.NextOperation = SpotSocketOperation.None;
                    socketEventArgCompleted(null, socketEventArg);
                }
            }
            else
            {
                data.NextOperation = SpotSocketOperation.Error;
                socketEventArgCompleted(null, socketEventArg);
            }
        }

        #endregion Receive Functions

        #region Helper Functions

        /// <summary>
        /// A single callback is used for all socket operations. 
        /// This method forwards execution on to the correct handler based on the type of completed operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void socketEventArgCompleted(object sender, SocketAsyncEventArgs socketEventArg)
        {
            switch (((Data)socketEventArg.UserToken).NextOperation)
            {
                case SpotSocketOperation.None:
                    this.operationComplete.Set();
                    break;
                case SpotSocketOperation.SendSize:
                    sendSize(socketEventArg);
                    break;
                case SpotSocketOperation.ReceiveSize:
                    receiveSize(socketEventArg);
                    break;
                case SpotSocketOperation.ReceiveSizeComplete:
                    receiveSizeComplete(socketEventArg);
                    break;
                case SpotSocketOperation.SendChunk:
                    sendChunk(socketEventArg);
                    break;
                case SpotSocketOperation.ReceiveChunk:
                    receiveChunk(socketEventArg);
                    break;
                case SpotSocketOperation.Close:
                    close(socketEventArg);
                    break;
                case SpotSocketOperation.Error:
                    this.operationComplete.Set();
                    break;
            }
        }

        private void close(SocketAsyncEventArgs socketEventArg)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// This function closes and disposes Network Stream
        /// </summary>
        protected void cleanStream()
        {
            //Do nothing right now
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
            throw new NotImplementedException("Creating SpotTcpSocketServer is not supported on Windows Phone.");
        }
        #endregion Constructor

        #region CreateListner Functions
        /// <summary>
        /// This function is used to create a Listner socket
        /// </summary>
        /// <returns>Listner socket</returns>
        public Socket CreatetListner()
        {
            return null;
        }
        #endregion CreateListner Functions

        #region Accept
        /// <summary>
        /// This function waits for incomming socket call and once there is one it returns that socket.
        /// </summary>
        /// <returns>Communicating Socket</returns>
        public Socket Accept()
        {
            return null;
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
        /// This function creates a client socket.
        /// </summary>
        /// <returns>Client Socket, not yet connected</returns>
        public Socket CreateClient()
        {
            cleanStream();
            cleanSocket(this.socket);

            if (this.serverEndPoint == null)
            {
                this.serverEndPoint = new DnsEndPoint(hostNameOrAddress, this.port);
            }

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            return this.socket;
        }
        #endregion CreateClient Functions
    }
    #endregion Class SpotTcpSocketClient
}
