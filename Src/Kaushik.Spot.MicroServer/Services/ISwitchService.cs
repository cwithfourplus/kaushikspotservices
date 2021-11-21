using Kaushik.Spot.Library;

namespace Kaushik.Spot.MicroServer
{
    /// <summary>
    /// Service Contract for SwitchService
    /// </summary>
    [SpotService]
    public interface ISwitchService
    {
        /// <summary>
        /// This function is used to Switch on and off given switch number.
        /// </summary>
        /// <param name="on">True for On, False for Off</param>
        /// <param name="switchNumber">Switch Number</param>
        /// <returns>True when done</returns>
        [SpotMethod]
        bool ApplySwitchState(bool on, int switchNumber);
    }
}
