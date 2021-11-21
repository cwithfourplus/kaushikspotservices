using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System;
using Microsoft.SPOT;

namespace Kaushik.Spot.MicroServer
{
    /// <summary>
    /// Class that implements LED Blinking Service
    /// </summary>
    public class BlinkLedService : IBlinkLedService
    {
        private static object synchronize;
        private static int pattern;
        private static Thread blikLedThread;

        /// <summary>
        /// Constructor
        /// </summary>
        static BlinkLedService()
        {
            BlinkLedService.synchronize = new object();
            BlinkLedService.blikLedThread = new Thread(blikLed);
            BlinkLedService.pattern = 1;
        }

        /// <summary>
        /// This method is used to change LED blink pattern
        /// </summary>
        /// <param name="pattern">Pattern Number</param>
        /// <returns>True once done</returns>
        public bool ApplyPattern(int pattern)
        {
            try
            {
                lock (BlinkLedService.synchronize)
                {
                    BlinkLedService.pattern = pattern;
                }

                if (!BlinkLedService.blikLedThread.IsAlive)
                {
                    BlinkLedService.blikLedThread.Start();
                }
            }
            catch (Exception error)
            {
                Debug.Print(error.Message);
                throw;
            }
            return true;
        }

        /// <summary>
        /// This function runs in a loop on a thread and is used to control pattern of LED blinking
        /// </summary>
        private static void blikLed()
        {
            OutputPort ledPort = new OutputPort(Pins.ONBOARD_LED, false);

            int loopLimit = 0;
            try
            {
                while (true)
                {
                    lock (BlinkLedService.synchronize)
                    {
                        switch (BlinkLedService.pattern)
                        {
                            case 1: //Simple Blink
                                ledPort.Write(true);    // turn on LED
                                Thread.Sleep(200);      // wait 200 ms

                                ledPort.Write(false);   // turn off LED
                                Thread.Sleep(200);      // wait 200 ms
                                break;
                            case 2: //Progressive Blink
                                for (int counter = 0; counter <= loopLimit; counter++)
                                {
                                    ledPort.Write(true);    // turn on LED
                                    Thread.Sleep(200);      // wait 200 ms

                                    ledPort.Write(false);   // turn off LED
                                    Thread.Sleep(200);      // wait 200 ms
                                }

                                loopLimit++;

                                if (loopLimit == 10)
                                {
                                    loopLimit = 0;
                                }
                                break;

                            case 3: //Always On
                                ledPort.Write(true);    // turn on LED
                                break;

                            case 4: //Always Off
                            default:
                                ledPort.Write(false);    // turn on LED
                                break;
                        }
                    }

                    Thread.Sleep(1000);      // wait 10000 ms
                }
            }
            catch (Exception error)
            {
                Debug.Print(error.Message);
                throw;
            }
        }
    }
}
