using System;
using Microsoft.SPOT;
using Kaushik.Spot.Library;

namespace Kaushik.Spot.MicroServer
{
    [SpotService]
    public interface ISayHello
    {
        [SpotMethod]
        string HelloMessage(string name);
    }
}
