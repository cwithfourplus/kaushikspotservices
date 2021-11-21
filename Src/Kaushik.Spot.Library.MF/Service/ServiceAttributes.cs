using System;

namespace Kaushik.Spot.Library
{
    #region SpotServiceAttribute
    /// <summary>
    /// This attribute is a marking attribute for SPOT Service and is used by proxy generation tool.
    /// </summary>
    public class SpotServiceAttribute : Attribute
    {
    }
    #endregion SpotServiceAttribute

    #region SpotMethodAttribute
    /// <summary>
    /// This attribute is a marking attribute for SPOT Service Method and is used by proxy generation tool.
    /// </summary>
    public class SpotMethodAttribute : Attribute
    {
    }
    #endregion SpotMethodAttribute
}
