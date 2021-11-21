using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System;
using Microsoft.SPOT;

namespace Kaushik.Spot.MicroServer
{
    /// <summary>
    /// This is a service that controls Switch points on Micro Controller
    /// </summary>
    public class SwitchService : ISwitchService
    {
        private static object switch1;
        private static object switch2;
        private static object switch3;
        private static Thread switchThread;

        /// <summary>
        /// Constructor
        /// </summary>
        static SwitchService()
        {
            SwitchService.switchThread = new Thread(controlSwitch);
            SwitchService.switch1 = false;
            SwitchService.switch2 = false;
            SwitchService.switch3 = false;

            if (!SwitchService.switchThread.IsAlive)
            {
                SwitchService.switchThread.Start();
            }
        }

        /// <summary>
        /// This function is used to Switch on and off given switch number.
        /// </summary>
        /// <param name="on">True for On, False for Off</param>
        /// <param name="switchNumber">Switch Number</param>
        /// <returns>True when done</returns>
        public bool ApplySwitchState(bool on, int switchNumber)
        {
            try
            {
                switch (switchNumber)
                {
                    case 1:
                        lock (SwitchService.switch1)
                        {
                            SwitchService.switch1 = on;
                        }
                        break;
                    case 2:
                        lock (SwitchService.switch2)
                        {
                            SwitchService.switch2 = on;
                        }
                        break;
                    case 3:
                        lock (SwitchService.switch3)
                        {
                            SwitchService.switch3 = on;
                        }
                        break;
                    default:
                        throw new NotSupportedException("Switch #" + switchNumber.ToString() + " is not supported.");
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
        /// This function runs in a loop on a thread and is used to control switchs
        /// </summary>
        private static void controlSwitch()
        {
            OutputPort switchPin1 = new OutputPort(Pins.GPIO_PIN_D0, false);
            OutputPort switchPin2 = new OutputPort(Pins.GPIO_PIN_D1, false);
            OutputPort switchPin3 = new OutputPort(Pins.GPIO_PIN_D2, false);
            try
            {
                while (true)
                {
                    lock (SwitchService.switch1)
                    {
                        switchPin1.Write((bool)SwitchService.switch1);
                    }
                    lock (SwitchService.switch2)
                    {
                        switchPin2.Write((bool)SwitchService.switch2);
                    }
                    lock (SwitchService.switch3)
                    {
                        switchPin3.Write((bool)SwitchService.switch3);
                    }

                    Thread.Sleep(1000);      // wait 1000 ms
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
