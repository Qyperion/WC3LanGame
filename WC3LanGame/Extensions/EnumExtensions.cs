using System.ComponentModel;
using System.Reflection;
using WC3LanGame.Warcraft3.Types;

namespace WC3LanGame.Extensions
{
    internal static class EnumExtensions
    {
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            Type type = value.GetType();
            MemberInfo[] memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            
            return attributes.Length > 0
                ? (T)attributes[0]
                : null;
        }

        public static string Version(this WarcraftVersion value)
        {
            DescriptionAttribute attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static byte Id(this WarcraftVersion value)
        {
            return (byte)value;
        }
    }
}
