using Kaushik.Spot.Library;

namespace Kaushik.Spot.MicroServer
{
    /// <summary>
    /// This is a Service Contract of Blink LED Service
    /// </summary>
    [SpotService]
    public interface IBlinkLedService
    {
        /// <summary>
        /// This method is used to change LED blink pattern
        /// </summary>
        /// <param name="pattern">Pattern Number</param>
        /// <returns>True once done</returns>
        [SpotMethod]
        bool ApplyPattern(int pattern);
    }
}
