using System;
using Microsoft.SPOT;
using Kaushik.Spot.Library;

namespace Kaushik.Spot.MicroServer
{
    /// <summary>
    /// This is a simple test service for SPOT Services
    /// </summary>
    public class SpotServiceTest : ISpotServiceTest
    {
        /// <summary>
        /// This function adds two int values and returns result
        /// </summary>
        /// <param name="a">First Int</param>
        /// <param name="b">Second Int</param>
        /// <returns>Sum</returns>
        public int Add(int a, int b)
        {
            try
            {
                return a + b;
            }
            catch (Exception error)
            {
                Debug.Print(error.Message);
                throw;
            }
        }

        /// <summary>
        /// This function is used to return Hello message for given Name.
        /// </summary>
        /// <param name="name">Name of person</param>
        /// <returns>Hello Message</returns>
        public string SayHello(string name)
        {
            try
            {
                return "Hello " + name + "!";
            }
            catch (Exception error)
            {
                Debug.Print(error.Message);
                throw;
            }
        }

        /// <summary>
        /// This method returns same string which is passed. This is just for test of Proxy Code Generator
        /// </summary>
        /// <param name="test">string</param>
        /// <returns>string</returns>
        public int NonSpotMethod(int test)
        {
            return test;
        }
    }
}
