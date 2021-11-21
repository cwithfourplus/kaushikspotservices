using System;
using System.Text;

namespace Kaushik.Spot.Library
{
    /// <summary>
    /// This class provides RC4 Cipher.
    /// </summary>
    public class RC4Cipher : ICryptographicServiceProvider
    {
        #region Costructor

        public RC4Cipher(string key)
        {
            boxLength = 255;
            encryptionKey = key;
        }

        #endregion

        #region Private Fields
        /// <summary>
        /// RC4 Box used for cipher algorithm
        /// </summary>
        protected byte[] box;
        /// <summary>
        /// Length of Box, 127 was the max length that could work with UTF8.
        /// </summary>
        private readonly uint boxLength;
        /// <summary>
        /// Encryption key
        /// </summary>
        private string encryptionKey;
        #endregion

        #region Public Method

        /// <summary>
        /// Used to encrypt the message using RC4, Use UTF8 encoding as .NET Micro framework does not support any other encoding.
        /// </summary>
        /// <param name="plainText">Plain Text</param>
        /// <returns>Encrypted text</returns>
        public string Encrypt(string plainText)
        {
            string cipherText;
            uint counterI = 0;
            uint counterJ = 0;
            byte first, second, temp;

            byte[] input = Encoding.UTF8.GetBytes(plainText);
            byte[] output = new byte[input.Length];

            uint chipherLength = (uint)(input.Length + 1);

            setBox();

            // Run Alghoritm
            for (uint offset = 0; offset < input.Length; offset++)
            {
                counterI = (counterI + 1) % boxLength;
                counterJ = (counterJ + box[counterI]) % boxLength;
                temp = box[counterI];
                box[counterI] = box[counterJ];
                box[counterJ] = temp;
                first = input[offset];
                second = box[(box[counterI] + box[counterJ]) % boxLength];
                output[offset] = (byte)((int)first ^ (int)second);
            }

            cipherText = new String(Encoding.UTF8.GetChars(output));

            return cipherText;
        }

        /// <summary>
        /// Used to dencrypt the message using RC4, Use UTF8 encoding as .NET Micro framework does not support any other encoding.
        /// </summary>
        /// <param name="cipherText">Encrypted Text</param>
        /// <returns>Decrypted text</returns>
        public string Decrypt(string cipherText)
        {
            return Encrypt(cipherText);
        }

        #endregion

        #region Helper Functions

        private void setBox()
        {
            box = new byte[boxLength];

            uint index = 0;

            byte[] encodedKey = Encoding.UTF8.GetBytes(encryptionKey);

            uint keyLength = (uint)encryptionKey.Length;

            for (uint counter = 0; counter < boxLength; counter++)
            {
                //126 is to do adjustments for UTF8
                box[counter] = (byte)((counter + 1) % 126);
            }

            for (uint counter = 0; counter < boxLength; counter++)
            {
                index = (index + box[counter] + encodedKey[counter % keyLength]) % boxLength;
                byte temp = box[counter];
                box[counter] = box[index];
                box[index] = temp;
            }
        }
        #endregion

    }
}
