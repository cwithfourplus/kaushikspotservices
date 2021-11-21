using System;
using Kaushik.Spot.Library;

namespace Kaushik.Spot.MicroServer
{
    /// <summary>
    /// Service contract of SpotServiceTest
    /// </summary>
    [SpotService]
    public interface ISpotServiceTest
    {
        /// <summary>
        /// This function adds two int values and returns result
        /// </summary>
        /// <param name="a">First Int</param>
        /// <param name="b">Second Int</param>
        /// <returns>Sum</returns>
        [SpotMethod]
        int Add(int a, int b);
        
        /// <summary>
        /// This function is used to return Hello message for given Name.
        /// </summary>
        /// <param name="name">Name of person</param>
        /// <returns>Hello Message</returns>
        [SpotMethod]
        string SayHello(string name);
    }
}
