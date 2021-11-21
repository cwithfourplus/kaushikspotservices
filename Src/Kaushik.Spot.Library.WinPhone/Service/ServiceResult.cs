using System;

namespace Kaushik.Spot.Library
{
    /// <summary>
    /// Object of this class is used for generating response of SPOT Service request.
    /// </summary>
    public class ServiceResult : ServiceDataContract
    {
        /// <summary>
        /// Return value of SPOT Method
        /// </summary>
        public object Result;

        /// <summary>
        /// Exception details if any
        /// </summary>
        public string Error;
    }
}
