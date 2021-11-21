using System;
using System.Reflection;
using System.Text;
using Microsoft.SPOT;

namespace Kaushik.Spot.Library
{
    public static class Utility
    {

        #region SizeBytesCount
        public const ushort SizeBytesCount = 4;
        #endregion SizeBytesCount

        #region ConvertToType
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
                case "System.Object": convertedValue = value;
                    break;
                default:
                    throw new NotSupportedException("Data type conversion not supported!");
            }

            return convertedValue;
        }
        #endregion ConvertToType

        #region GetFixedLengthString
        public static string GetFixedLengthString(string value, ushort length = SizeBytesCount)
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
        public static byte[] ConvertIntToByteArray(int size)
        {
            byte[] sizeInBytes = Encoding.UTF8.GetBytes(size.ToString());

            if (sizeInBytes.Length > SizeBytesCount)
            {
                throw new NotSupportedException("The size cannot be more than 9999.");
            }
            else if (sizeInBytes.Length < SizeBytesCount)
            {
                byte[] formatedSizeInBytes = new byte[] { 48, 48, 48, 48 }; // 48 is Zero in UTF8
                sizeInBytes.CopyTo(formatedSizeInBytes, SizeBytesCount - sizeInBytes.Length);
                sizeInBytes = formatedSizeInBytes;
            }

            return sizeInBytes;
        }
        #endregion ConvertIntToByteArray

        #region Serialize
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
                                    throw new NotSupportedException("Data Type '" + value.GetType().ToString() + "' is not supported.");
                                }
                            }
                        }
                        else
                        {
                            throw new NotSupportedException("Data Type '" + field.GetType().ToString() + "' is not supported.");
                        }
                    }

                    //Debug.GC(true);
                }
            }

            //Debug.GC(true);

            return stringNotation;
        }
        #endregion Serialize

        #region Deserialize
        public static void Deserialize(ServiceDataContract currentContract, string stringNotation)
        {
            ushort currentPosition = 0, infoLength;
            short arrayItemCount;

            string name, type, value;
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

                            //Debug.GC(true);
                        }

                        //Set the value
                        field.SetValue(currentContract, array);
                    }

                    //Debug.GC(true);
                } while (currentPosition < stringNotation.Length);
            }
            catch (Exception error)
            {
                throw new NotSupportedException("Operation not supported.", error);
            }

            name = null;
            type = null;
            value = null;
            serviceDataContractType = null;
            field = null;

            //Debug.GC(true);
        }
        #endregion Deserialize

        #region GetArrayString
        private static string getArrayString(FieldInfo field, object current)
        {
            int totalElements = 0;

            if (field.GetValue(current) != null)
            {
                totalElements = ((object[])field.GetValue(current)).Length;
            }

            return Utility.GetFixedLengthString(field.Name.Length.ToString()) + field.Name
                + Utility.GetFixedLengthString(field.FieldType.ToString().Length.ToString()) + field.FieldType.ToString()
                + Utility.GetFixedLengthString(totalElements.ToString().Length.ToString()) + totalElements.ToString();
        }
        #endregion GetArrayString

        #region GetFieldString
        private static string getFieldString(FieldInfo field, object current)
        {
            if (field.GetValue(current) != null)
            {
                return Utility.GetFixedLengthString(field.Name.Length.ToString()) + field.Name
                    + Utility.GetFixedLengthString(field.FieldType.ToString().Length.ToString()) + field.FieldType.ToString()
                    + Utility.GetFixedLengthString(field.GetValue(current).ToString().Length.ToString()) + field.GetValue(current).ToString();
            }
            else
            {
                return Utility.GetFixedLengthString(field.Name.Length.ToString()) + field.Name
                    + Utility.GetFixedLengthString(field.FieldType.ToString().Length.ToString()) + field.FieldType.ToString()
                    + Utility.GetFixedLengthString("0");
            }
        }
        #endregion GetFieldString

        #region GetArrayItemString
        private static string getArrayItemString(string itemName, object value)
        {
            if (value != null)
            {
                return Utility.GetFixedLengthString(itemName.Length.ToString()) + itemName
                    + Utility.GetFixedLengthString(value.GetType().ToString().Length.ToString()) + value.GetType().ToString()
                    + Utility.GetFixedLengthString(value.ToString().Length.ToString()) + value.ToString();
            }
            else
            {
                return Utility.GetFixedLengthString(itemName.Length.ToString()) + itemName
                    + Utility.GetFixedLengthString(value.GetType().ToString().Length.ToString()) + value.GetType().ToString()
                    + Utility.GetFixedLengthString("0");
            }
        }
        #endregion GetArrayItemString

        #region GetInfoLenght
        private static ushort getInfoLenght(string stringNotation, ref ushort currentPosition)
        {
            return Convert.ToUInt16(getInfo(stringNotation, ref currentPosition, Utility.SizeBytesCount).Trim());
        }
        #endregion GetInfoLenght

        #region GetInfo
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
