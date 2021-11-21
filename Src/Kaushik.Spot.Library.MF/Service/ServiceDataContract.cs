
namespace Kaushik.Spot.Library
{
    /// <summary>
    /// This is a base class for ServiceCommand and ServiceResult classes. 
    /// It provides Serialize and Deserialize
    /// </summary>
    public class ServiceDataContract
    {
        #region Serialize
        /// <summary>
        /// Serialize current object
        /// </summary>
        /// <returns>String representing current object</returns>
        public string Serialize()
        {
            return Utility.Serialize(this);
        }
        #endregion Serialize

        #region Deserialize
        /// <summary>
        /// Deserialize and populate field values of current object from string representing current object
        /// </summary>
        /// <param name="stringNotation">String representing current object</param>
        public void Deserialize(string stringNotation)
        {
            Utility.Deserialize(this, stringNotation);
        }
        #endregion Deserialize
    }
}
