namespace Kaushik.Spot.Library
{
    /// <summary>
    /// Class that implements this Interface can be used to encrypt data of SPOT Services over network. 
    /// Same implementation of this class should be used by server and client application. 
    /// Although chipper can be symmetric as well as asymmetric key, and the class can get the key using constructor. 
    /// </summary>
    public interface ICryptographicServiceProvider
    {
        /// <summary>
        /// Used to encrypt the message, Use UTF8 encoding as .NET Micro framework does not support any other encoding.
        /// </summary>
        /// <param name="plainText">Plain Text</param>
        /// <returns>Encrypted text</returns>
        string Encrypt(string plainText);

        /// <summary>
        /// Used to dencrypt the message, Use UTF8 encoding as .NET Micro framework does not support any other encoding.
        /// </summary>
        /// <param name="cipherText">Encrypted Text</param>
        /// <returns>Decrypted text</returns>
        string Decrypt(string cipherText);
    }
}
