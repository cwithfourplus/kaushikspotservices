using System;
using System.Security.Cryptography;

namespace Kaushik.Spot.ConsoleClient
{
    class ClientProgram
    {

        static void Main(string[] args)
        {

            TestSpotService(1000000);
            //Crypto();

            Console.ReadLine();
        }

        public static void TestSpotService(int loopLimit)
        {
            SpotServiceTest spotServiceProxy = new SpotServiceTest(null, "localhost", 100, Library.SpotSocketProvider.Tcp);
            SwitchService switchServiceProxy = new SwitchService(null, "localhost", 100, Library.SpotSocketProvider.Tcp);
            BlinkLedService blinkLedServiceProxy = new BlinkLedService(null, "localhost", 100, Library.SpotSocketProvider.Tcp);

            for (int counter = 0; counter < loopLimit; counter++)
            {
                Console.WriteLine("**************************************************************************");
                Console.WriteLine(counter);
                Console.WriteLine("**************************************************************************");

                TestSpotService(spotServiceProxy, switchServiceProxy, blinkLedServiceProxy);



                Console.WriteLine("**************************************************************************");
            }
        }

        public static void TestSpotService(SpotServiceTest spotServiceProxy, SwitchService switchServiceProxy, BlinkLedService blinkLedServiceProxy)
        {
            try
            {

                int sum = spotServiceProxy.Add(10, 20);

                Console.WriteLine("Result of Add is : {0}", sum);

                string message = spotServiceProxy.SayHello("Vishal Kaushik");

                Console.WriteLine("Result of Hello is : {0}", message);

                bool result = switchServiceProxy.ApplySwitchState(true, 1);

                result = blinkLedServiceProxy.ApplyPattern(1);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Crypto()
        {
            try
            {
                //string data = "This is a test data";

                //System.Security.Cryptography. crypto = new Key_TinyEncryptionAlgorithm(Encoding.UTF8.GetBytes("Matrix"));

                //byte[] result = crypto.Encrypt(Encoding.UTF8.GetBytes(data), 0, data.Length, null);
                //result = new byte[] { 195, 156, 8, 249, 52, 26, 146, 224, 54, 15, 0, 112, 156, 14, 74, 100, 111, 149, 99 };
                //data = new string(Encoding.UTF8.GetChars(crypto.Decrypt(result, 0, data.Length, null)));

            }
            catch (CryptographicException e)
            {
                //Catch this exception in case the encryption did
                //not succeed.
                Console.WriteLine(e.Message);

            }

        }
    }
}
