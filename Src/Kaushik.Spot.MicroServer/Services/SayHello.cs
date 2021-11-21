using System;
using Microsoft.SPOT;

namespace Kaushik.Spot.MicroServer
{
    public class SayHello : ISayHello
    {

        public string HelloMessage(string name)
        {
            return "Hello " + name + "!!!";
        }
    }
}
