﻿/**********************************************************************************
 This class is generated using Kaushik.Spot.CodeGenerator, 
 any change to this file may lead to malfunction. 
 **********************************************************************************/
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kaushik.Spot.ControllerApp
{
    using System;
    using System.Net.Sockets;
    using Kaushik.Spot.Library;


    public sealed class SayHello
    {

        private string server;

        private short port;

        private short bufferSize;

        private int socketClientTimeout;

        private short chunkSize;

        private string serviceName;

        private ICryptographicServiceProvider cipher;

        private ISpotSocketClient socketClient;

        public SayHello() :
            this(null, "192.167.1.107", 8080, SpotSocketProvider.Tcp)
        {
        }

        public SayHello(ICryptographicServiceProvider cipher, string server, short port, SpotSocketProvider provider)
        {
            this.server = server;
            this.port = port;
            this.bufferSize = 5120;
            this.socketClientTimeout = 50000000;
            this.chunkSize = 500;
            this.serviceName = "Kaushik.Spot.MicroServer.ISayHello";
            this.cipher = cipher;
            this.socketClient = SpotSocketFactory.GetClientProvider(server, provider, port, bufferSize, socketClientTimeout, chunkSize);
        }

        private string encrypt(string text)
        {
            if ((this.cipher == null))
            {
                return text;
            }
            else
            {
                return this.cipher.Encrypt(text);
            }
        }

        private string decrypt(string text)
        {
            if ((this.cipher == null))
            {
                return text;
            }
            else
            {
                return this.cipher.Decrypt(text);
            }
        }

        public string HelloMessage(string name)
        {
            string request;
            Kaushik.Spot.Library.ServiceCommand command;
            Kaushik.Spot.Library.ServiceResult result;
            string response;
            string finalResult;
            System.Net.Sockets.Socket socket;
            request = null;
            command = new Kaushik.Spot.Library.ServiceCommand();
            result = new Kaushik.Spot.Library.ServiceResult();
            response = null;
            finalResult = default(string);
            socket = this.socketClient.CreateClient();
            try
            {
                command.ServiceName = this.serviceName;
                command.Method = "HelloMessage";
                command.Parameters = new object[1];
                command.Parameters[0] = name;
                request = this.encrypt(command.Serialize());
                this.socketClient.Send(request);
                this.socketClient.Receive(out response);
                result.Deserialize(this.decrypt(response));
                if ((string.IsNullOrEmpty(result.Error) != true))
                {
                    throw new System.Exception(result.Error);
                }
                if ((result.Result != null))
                {
                    finalResult = result.Result.ToString();
                }
            }
            finally
            {
                if ((socket != null))
                {
                    socket.Dispose();
                    socket = null;
                }
            }
            return finalResult;
        }
    }
}
