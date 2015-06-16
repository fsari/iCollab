using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Core.Extensions
{
    public static class EnumExtensions
    {
        public static string DisplayName(this Enum value)
        {
            Type enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            MemberInfo member = enumType.GetMember(enumValue)[0];

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            var outString = ((DisplayAttribute)attrs[0]).Name;

            if (((DisplayAttribute)attrs[0]).ResourceType != null)
            {
                outString = ((DisplayAttribute)attrs[0]).GetName();
            }

            return outString;
        }

        public static T ParseEnum<T>(string value)
        {
            //return (T)Enum.Parse(typeof(T), value, true);

            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Generic Type 'T' must be an Enum");
            }
            if (!string.IsNullOrEmpty(value))
            {
                if (Enum.GetNames(typeof(T)).Any(
                      e => e.Trim().ToUpperInvariant() == value.Trim().ToUpperInvariant()))
                {
                    return (T)Enum.Parse(typeof(T), value, true);
                }
            }
            return default(T);
        }
    }
}