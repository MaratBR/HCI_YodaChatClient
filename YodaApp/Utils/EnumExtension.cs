using System;
using System.ComponentModel;
using System.Reflection;

namespace YodaApp.Utils
{
    internal static class EnumExtension
    {
        /// <summary>
        /// https://stackoverflow.com/questions/1415140/can-my-enums-have-friendly-names
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>string representation of value</returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return value.ToString();
        }
    }
}