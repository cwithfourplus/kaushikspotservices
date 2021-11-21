using System;
using System.Reflection;
using System.Text;

namespace Kaushik.Spot.Library
{
    /// <summary>
    /// This is a common utility class
    /// </summary>
    public static class Utility
    {
        #region Constants
        /// <summary>
        /// This is the length of bytes used as Size for data that will be preceded in network communication or serialization.
        /// </summary>
        public const ushort SizeBytesCount = 4;
        #endregion Constants

        #region ConvertToType
        /// <summary>
        /// This function is used to convert a string value into the type provided.
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="type">Full name of type</param>
        /// <returns>Converted result as Object</returns>
        public static object ConvertToType(string value, string type)
        {
            object convertedValue = null;

            switch (type)
            {
                case "System.Byte":
                    convertedValue = Convert.ToByte(value);
                    break;
                case "System.SByte":
                    convertedValue = Convert.ToSByte(value);
                    break;
                case "System.Int16":
                    convertedValue = Convert.ToInt16(value);
                    break;
                case "System.Int32":
                    convertedValue = Convert.ToInt32(value);
                    break;
                case "System.Int64":
                    convertedValue = Convert.ToInt64(value);
                    break;
                case "System.UInt16":
                    convertedValue = Convert.ToUInt16(value);
                    break;
                case "System.UInt32":
                    convertedValue = Convert.ToUInt32(value);
                    break;
                case "System.UInt64":
                    convertedValue = Convert.ToUInt64(value);
                    break;
                case "System.Single":
                    convertedValue = Convert.ToDouble(value);
                    break;
                case "System.Double":
                    convertedValue = Convert.ToDouble(value);
                    break;
                case "System.Char":
                    convertedValue = Convert.ToChar(Convert.ToUInt16(value));
                    break;
                case "System.Decimal":
                    convertedValue = Convert.ToDouble(value);
                    break;
                case "System.String":
                    convertedValue = value;
                    break;
                case "System.Boolean":
                    if (value.ToUpper() == "TRUE")
                    {
                        convertedValue = true;
                    }
                    else
                    {
                        convertedValue = false;
                    }
                    break;
                case "System.Object":
                    convertedValue = value;
                    break;
                default:
                    throw new NotSupportedException("Data type conversion not supported!");
            }

            return convertedValue;
        }
        #endregion ConvertToType

        #region GetFixedLengthString
        /// <summary>
        /// This function adds trailing spaces so as to make the length of string as specified in length parameter.
        /// </summary>
        /// <param name="value">String Value</param>
        /// <param name="length">This is the length of bytes used as Size for data that will be preceded in serialization.</param>
        /// <returns>Fixed length string</returns>
        private static string getFixedLengthString(string value, ushort length = SizeBytesCount)
        {
            if (value.Length < length)
            {
                int loopLimit = length - value.Length;

                for (short counter = 0; counter < loopLimit; counter++)
                {
                    value += " ";
                }
            }
            else
            {
                throw new Exception("String length is already more than desired length.");
            }

            return value;
        }
        #endregion GetFixedLengthString

        #region ConvertIntToByteArray
        /// <summary>
        /// This function converts int size into fixed length byte array
        /// </summary>
        /// <param name="size">This Size of data that will be sent in network communication</param>
        /// <param name="length">This is the length of bytes used as Size for data that will be preceded in network communication.</param>
        /// <returns>Byte array of int value</returns>
        public static byte[] ConvertIntToByteArray(int size, ushort length = SizeBytesCount)
        {
            byte[] sizeInBytes = Encoding.UTF8.GetBytes(size.ToString());

            if (sizeInBytes.Length > length)
            {
                throw new Exception("The size cannot be more than 9999.");
            }
            else if (sizeInBytes.Length < length)
            {
                byte[] formatedSizeInBytes = new byte[] { 48, 48, 48, 48 }; // 48 is Zero in UTF8
                sizeInBytes.CopyTo(formatedSizeInBytes, length - sizeInBytes.Length);
                sizeInBytes = formatedSizeInBytes;
            }

            return sizeInBytes;
        }
        #endregion ConvertIntToByteArray

        #region Serialize
        /// <summary>
        /// This function converts current Service Contract into String.
        /// </summary>
        /// <param name="currentContract">Object of Service Data Contract</param>
        /// <returns>String representing current Service Contract</returns>
        public static string Serialize(ServiceDataContract currentContract)
        {
            Type serviceDataContractType = currentContract.GetType();
            FieldInfo[] fields = serviceDataContractType.GetFields();
            string stringNotation = null;

            if (fields != null)
            {
                foreach (FieldInfo field in fields)
                {
                    if (field.GetValue(currentContract) != null)
                    {
                        if (field.GetValue(currentContract) is ValueType
                            || field.GetValue(currentContract) is String)
                        {
                            stringNotation += Utility.getFieldString(field, currentContract);
                        }
                        else if (field.GetValue(currentContract) is System.Array)
                        {
                            stringNotation += Utility.getArrayString(field, currentContract);

                            foreach (object value in (object[])field.GetValue(currentContract))
                            {
                                if (value is ValueType
                                        || value is String)
                                {
                                    stringNotation += Utility.getArrayItemString("Item", value);
                                }
                                else
                                {
                                    throw new Exception("Data Type '" + value.GetType().ToString() + "' is not supported.");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Data Type '" + field.GetType().ToString() + "' is not supported.");
                        }
                    }
                }
            }

            return stringNotation;
        }
        #endregion Serialize

        #region Deserialize
        /// <summary>
        /// This function is used to convert a string into ServiceDataContract i.e. fill all its field values.
        /// </summary>
        /// <param name="currentContract">Object of current Service Contract</param>
        /// <param name="stringNotation">String Value</param>
        public static void Deserialize(ServiceDataContract currentContract, string stringNotation)
        {
            ushort currentPosition = 0, infoLength;
            string name, type, value;
            short arrayItemCount;
            Type serviceDataContractType = currentContract.GetType();
            FieldInfo field;

            try
            {
                do
                {
                    //Get Name Length
                    infoLength = Utility.getInfoLenght(stringNotation, ref currentPosition);
                    //Get Name
                    name = Utility.getInfo(stringNotation, ref currentPosition, infoLength);

                    field = serviceDataContractType.GetField(name);

                    //Get Type Length
                    infoLength = Utility.getInfoLenght(stringNotation, ref currentPosition);
                    //Get Type
                    type = Utility.getInfo(stringNotation, ref currentPosition, infoLength);
                    //Get Value Length
                    infoLength = Utility.getInfoLenght(stringNotation, ref currentPosition);

                    //Get Value
                    value = Utility.getInfo(stringNotation, ref currentPosition, infoLength);

                    if (type != "System.Array" && type != "System.Object[]")
                    {
                        //Set the value
                        field.SetValue(currentContract, Utility.ConvertToType(value, type));
                    }
                    else
                    {
                        arrayItemCount = Convert.ToInt16(value);
                        object[] array = new object[arrayItemCount];

                        for (int counter = 0; counter < arrayItemCount; counter++)
                        {
                            //Get Name Length
                            infoLength = Utility.getInfoLenght(stringNotation, ref currentPosition);
                            //Get Name
                            name = Utility.getInfo(stringNotation, ref currentPosition, infoLength);

                            //Get Type Length
                            infoLength = Utility.getInfoLenght(stringNotation, ref currentPosition);
                            //Get Type
                            type = Utility.getInfo(stringNotation, ref currentPosition, infoLength);
                            //Get Value Length
                            infoLength = Utility.getInfoLenght(stringNotation, ref currentPosition);

                            //Get Value
                            value = Utility.getInfo(stringNotation, ref currentPosition, infoLength);

                            //Set the value
                            array[counter] = Utility.ConvertToType(value, type);
                        }

                        //Set the value
                        field.SetValue(currentContract, array);
                    }
                } while (currentPosition < stringNotation.Length);
            }
            catch (Exception error)
            {
                throw new NotSupportedException("Operation not supported.", error);
            }
        }
        #endregion Deserialize

        #region GetArrayString
        /// <summary>
        /// This function is used to convert Array field into string. It just starts array value.
        /// </summary>
        /// <param name="field">Object Array Field</param>
        /// <param name="current">Object</param>
        /// <returns>String representing Array and its length</returns>
        private static string getArrayString(FieldInfo field, object current)
        {
            int totalElements = 0;

            if (field.GetValue(current) != null)
            {
                totalElements = ((object[])field.GetValue(current)).Length;
            }

            return Utility.getFixedLengthString(field.Name.Length.ToString()) + field.Name
                + Utility.getFixedLengthString(field.FieldType.ToString().Length.ToString()) + field.FieldType.ToString()
                + Utility.getFixedLengthString(totalElements.ToString().Length.ToString()) + totalElements.ToString();
        }
        #endregion GetArrayString

        #region GetFieldString
        /// <summary>
        /// This function converts given field value into string for given object.
        /// </summary>
        /// <param name="field">Object Field</param>
        /// <param name="current">Object</param>
        /// <returns>String representing complete field</returns>
        private static string getFieldString(FieldInfo field, object current)
        {
            if (field.GetValue(current) != null)
            {
                return Utility.getFixedLengthString(field.Name.Length.ToString()) + field.Name
                    + Utility.getFixedLengthString(field.FieldType.ToString().Length.ToString()) + field.FieldType.ToString()
                    + Utility.getFixedLengthString(field.GetValue(current).ToString().Length.ToString()) + field.GetValue(current).ToString();
            }
            else
            {
                return Utility.getFixedLengthString(field.Name.Length.ToString()) + field.Name
                    + Utility.getFixedLengthString(field.FieldType.ToString().Length.ToString()) + field.FieldType.ToString()
                    + Utility.getFixedLengthString("0");
            }
        }
        #endregion GetFieldString

        #region GetArrayItemString
        /// <summary>
        /// This function converts array item into string value
        /// </summary>
        /// <param name="itemName">Name of array item</param>
        /// <param name="value">Value of array item</param>
        /// <returns>String representing complete item</returns>
        private static string getArrayItemString(string itemName, object value)
        {
            if (value != null)
            {
                return Utility.getFixedLengthString(itemName.Length.ToString()) + itemName
                    + Utility.getFixedLengthString(value.GetType().ToString().Length.ToString()) + value.GetType().ToString()
                    + Utility.getFixedLengthString(value.ToString().Length.ToString()) + value.ToString();
            }
            else
            {
                return Utility.getFixedLengthString(itemName.Length.ToString()) + itemName
                    + Utility.getFixedLengthString(value.GetType().ToString().Length.ToString()) + value.GetType().ToString()
                    + Utility.getFixedLengthString("0");
            }
        }
        #endregion GetArrayItemString

        #region GetInfoLenght
        /// <summary>
        /// This function is used to read string as a forward only stream and returns length of information.
        /// It also incriments the currentPosition.
        /// </summary>
        /// <param name="stringNotation">Complete string notation</param>
        /// <param name="currentPosition">Start position of information</param>
        /// <returns>Length of information</returns>
        private static ushort getInfoLenght(string stringNotation, ref ushort currentPosition)
        {
            return Convert.ToUInt16(getInfo(stringNotation, ref currentPosition, Utility.SizeBytesCount).Trim());
        }
        #endregion GetInfoLenght

        #region GetInfo
        /// <summary>
        /// This function is used to read string as a forward only stream and return part of information starting from current position to the length given.
        /// It also incriments the currentPosition.
        /// </summary>
        /// <param name="stringNotation">Complete string notation</param>
        /// <param name="currentPosition">Start position of information</param>
        /// <param name="length">Length of information</param>
        /// <returns>Information</returns>
        private static string getInfo(string stringNotation, ref ushort currentPosition, ushort length)
        {
            string info = String.Empty;

            if (length > 0)
            {
                info = stringNotation.Substring(currentPosition, length);
                currentPosition += length;
            }

            return info;
        }
        #endregion GetInfo
    }
}
