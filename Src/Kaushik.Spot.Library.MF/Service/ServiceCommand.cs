namespace Kaushik.Spot.Library
{
    /// <summary>
    /// Object of this class is used for generating SPOT Service request.
    /// </summary>
    public class ServiceCommand : ServiceDataContract
    {
        /// <summary>
        /// Full name of SPOT Service
        /// </summary>
        public string ServiceName;

        /// <summary>
        /// Name of SPOT Service Method to be called.
        /// </summary>
        public string Method;

        /// <summary>
        /// Array of parameters of SPOT Service Method in proper order
        /// </summary>
        public object[] Parameters;
    }
}
