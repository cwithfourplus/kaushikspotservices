using System;
using System.Net.Sockets;

namespace Kaushik.Spot.Library
{
    #region Interface ISpotSocket
    /// <summary>
    /// This is a base inteface that has to be implemented for Network communication
    /// </summary>
    public interface ISpotSocket
    {
        /// <summary>
        /// This function sends string data over network
        /// </summary>
        /// <param name="data">String data</param>
        void Send(string data);

        /// <summary>
        /// This function sends string data over network starting from offset till the size given
        /// </summary>
        /// <param name="data">String data</param>
        /// <param name="offset">Start position in string</param>
        /// <param name="size">Total size to be sent</param>
        void Send(string data, int offset, int size);

        /// <summary>
        /// This function is used to recive data from network
        /// </summary>
        /// <param name="data">String data to be set</param>
        /// <returns>Length of bytes received</returns>
        int Receive(out string data);

        /// <summary>
        /// This function is used to recive data from network starting from offset till end of string
        /// </summary>
        /// <param name="data">String data to be set</param>
        /// <param name="offset"></param>
        /// <returns>Length of bytes received</returns>
        int Receive(out string data, int offset);

        /// <summary>
        /// This function is used to recive data from network starting from offset till the size given
        /// </summary>
        /// <param name="data">String data to be set</param>
        /// <param name="offset">Start position in string</param>
        /// <param name="size">Total size in bytes to be received</param>
        /// <returns>Length of bytes received</returns>
        int Receive(out string data, int offset, int size);
    }
    #endregion Interface ISpotSocket

    #region Interface ISpotSocketServer
    /// <summary>
    /// The class that impliments this interface is to be used by Server program for Network communication
    /// </summary>
    public interface ISpotSocketServer : ISpotSocket
    {
        /// <summary>
        /// This function is used to create a Listner socket
        /// </summary>
        /// <returns>Listner socket</returns>
        Socket CreatetListner();

        /// <summary>
        /// This function waits for incomming socket call and once there is one it returns that socket.
        /// </summary>
        /// <returns>Communicating Socket</returns>
        Socket Accept();
    }
    #endregion Interface ISpotSocketServer

    #region Interface ISpotSocketClient
    /// <summary>
    /// The class that impliments this interface is to be used by Server program for Network communication
    /// </summary>
    public interface ISpotSocketClient : ISpotSocket, IDisposable
    {
        /// <summary>
        /// This function creates a client socket.
        /// </summary>
        /// <returns>Client Socket</returns>
        Socket CreateClient();
    }
    #endregion Interface ISpotSocketClient
}
