using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace Kaushik.Spot.Library
{
    /// <summary>
    /// This class provides foundation of SPOT Services and provides all necessary functions to deal with SPOT Services.
    /// </summary>
    public static class SpotServices
    {
        #region Member Variables
        private static readonly short maxServices;
        private static object[] services;
        private static short currentServices;
        private static short port;
        private static bool serverRunning;
        private static ICryptographicServiceProvider cipher = null;
        #endregion Member Variables

        #region SpotServices
        /// <summary>
        /// Constructor
        /// </summary>
        static SpotServices()
        {
            SpotServices.maxServices = 5;
            SpotServices.services = new object[SpotServices.maxServices];
            SpotServices.currentServices = 0;
            SpotServices.serverRunning = false;
        }
        #endregion SpotServices

        #region RegisterService
        /// <summary>
        /// This function is used to register a SPOT Service
        /// </summary>
        /// <param name="service">Object of SPOT Service Class</param>
        public static void RegisterService(object service)
        {
            if (service != null)
            {
                if (SpotServices.currentServices < SpotServices.maxServices)
                {
                    lock (SpotServices.services)
                    {
                        SpotServices.services[SpotServices.currentServices] = service;
                    }
                    SpotServices.currentServices++;
                }
                else
                {
                    throw new Exception("Maximum number (which is " + SpotServices.maxServices.ToString() + ") of services already registered!");
                }
            }
            else
            {
                throw new NullReferenceException();
            }
        }
        #endregion RegisterService

        #region ExecuteCommand
        /// <summary>
        /// This is function is used to invoke a SPOT Service Method.
        /// </summary>
        /// <param name="command">Object of ServiceCommand</param>
        /// <returns>Object of ServiceResult that has return value and any exception details</returns>
        public static ServiceResult ExecuteCommand(ServiceCommand command)
        {
            ServiceResult result = new ServiceResult();
            bool serviceFound = false;

            try
            {
                if (command != null)
                {
                    for (int counter = 0; counter < SpotServices.currentServices; counter++)
                    {
                        if (Array.IndexOf(SpotServices.services[counter].GetType().GetInterfaces(), Type.GetType(command.ServiceName)) != -1)
                        {
                            serviceFound = true;

                            Type serviceType = SpotServices.services[counter].GetType();
                            MethodInfo method = serviceType.GetMethod(command.Method);
                            if (method != null)
                            {
                                result.Result = method.Invoke(SpotServices.services[counter], command.Parameters);
                            }
                            else
                            {
                                throw new NotSupportedException("Requested method not supported.");
                            }
                            break;
                        }
                    }

                    if (!serviceFound)
                    {
                        throw new NotSupportedException("Requested service not supported.");
                    }
                }
                else
                {
                    throw new Exception("Service call does not have Command information.");
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message + "\n" + "Error Details:" + ex.ToString();
            }

            return result;
        }
        /// <summary>
        /// This is function is used to invoke a SPOT Service Method using Serialized ServiceCommand object.
        /// This function also calls Encrypt and Decrypt if required.
        /// </summary>
        /// <param name="commandText">Serialized ServiceCommand object</param>
        /// <returns>Serialized ServiceResult object</returns>
        public static string ExecuteCommand(string commandText)
        {
            ServiceCommand command = new ServiceCommand();
            string resultText = null;

            if (cipher == null)
            {
                ServiceResult result = null;
                try
                {
                    command.Deserialize(commandText);
                    result = ExecuteCommand(command);
                    resultText = result.Serialize();
                }
                catch (Exception error)
                {
                    result = new ServiceResult();
                    result.Error = error.Message + "\n" + error.ToString();
                    resultText = result.Serialize();
                }
            }
            else
            {
                ServiceResult result = null;
                try
                {
                    command.Deserialize(cipher.Decrypt(commandText));
                    result = ExecuteCommand(command);
                    resultText = cipher.Encrypt(result.Serialize());
                }
                catch (Exception error)
                {
                    result = new ServiceResult();
                    result.Error = error.Message + "\n" + error.ToString();
                    resultText = cipher.Encrypt(result.Serialize());
                }
            }


            return resultText;
        }
        #endregion ExecuteCommand

        #region StartServer
        /// <summary>
        /// This function is used to start SPOT Services on a new thread. It will make sure if the Server is already running or not.
        /// </summary>
        /// <param name="port">Port on which services should be avalable, default 8080</param>
        /// <param name="cipher">Cipher program that can be used to protect calls over network.</param>
        public static void StartServer(short port = 8080, ICryptographicServiceProvider cipher = null)
        {
            if (!SpotServices.serverRunning)
            {
                SpotServices.port = port;
                cipher = cipher;
                Thread serverThread = new Thread(startServer);
                serverThread.Start();
                SpotServices.serverRunning = true;
            }
        }

        /// <summary>
        /// This function is used to start SPOT Services
        /// </summary>
        private static void startServer()
        {
            try
            {
                ISpotSocketServer server = SpotSocketFactory.GetServerProvider(SpotSocketProvider.Tcp, SpotServices.port, 5120, 50000000, 0, 500);

                string data;

                using (Socket listner = server.CreatetListner())
                {
                    while (true)
                    {
                        using (Socket socket = server.Accept())
                        {
                            try
                            {
                                server.Receive(out data);

                                string result = SpotServices.ExecuteCommand(data);

                                server.Send(result);

                                socket.Close();
                            }
                            catch (Exception ex)
                            {
                                string message = ex.Message;
                                Debug.Print(ex.Message);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                Debug.Print(ex.Message);
            }
        }
        #endregion StartServer
    }
}
