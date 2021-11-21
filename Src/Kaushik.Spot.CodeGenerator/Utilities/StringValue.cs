using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kaushik.Spot.CodeGenerator.Utilities
{
    public class StringValue : System.Attribute
    {
        private string value;

        public StringValue(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return this.value; }
        }
    }

    public static class EnumExtended
    {
        public static string ToDescription(Enum value)
        {
            string stringValue = null;

            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(value.ToString());

            StringValue[] attributes = fieldInfo.GetCustomAttributes(typeof(StringValue), false) as StringValue[];

            if (attributes.Length > 0)
            {
                stringValue = attributes[0].Value;
            }

            return stringValue;
        }
    }
}
