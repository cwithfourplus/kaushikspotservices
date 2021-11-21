using System;
using System.Net.Sockets;
using System.Threading;
using Kaushik.Spot.Library;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace Kaushik.Spot.MicroServer
{
    /// <summary>
    /// Main class of this project that provides entry point
    /// </summary>
    public class ServerProgram
    {
        /// <summary>
        /// This is main entry function
        /// </summary>
        public static void Main()
        {
            OutputPort ledPort = new OutputPort(Pins.ONBOARD_LED, false);

            StartServer();

            while (true)
            {
                Thread.Sleep(100000);
            }
        }

        /// <summary>
        /// This function starts the server
        /// </summary>
        public static void StartServer()
        {
            try
            {
                //string encryptionKey = "avbi4s8h4a9l5kfadujskh4i9k50dj39405fk";
                //ICryptographicServiceProvider cipher = new RC4Cipher(encryptionKey);

                //Register SPOT Services
                SpotServices.RegisterService(new SpotServiceTest());
                SpotServices.RegisterService(new BlinkLedService());
                SpotServices.RegisterService(new SwitchService());
                SpotServices.RegisterService(new SayHello());

                //Start SPOT Server
                SpotServices.StartServer(8080);
            }
            catch (Exception error)
            {
                string message = error.Message;
                Trace.Print(error.Message);
            }
        }

    }
}
