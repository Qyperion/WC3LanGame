using System.ComponentModel;

using WC3LanGame.Core.Warcraft3.Types;

namespace WC3LanGame.Core.Extensions;

public static class EnumExtensions
{
    public static T GetAttribute<T>(this Enum value) where T : Attribute
    {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);

        return attributes.Length > 0
            ? (T)attributes[0]
            : null;
    }

    public static string Version(this WarcraftVersion value)
    {
        return value.Description();
    }

    public static string Description(this Enum value)
    {
        var attribute = value.GetAttribute<DescriptionAttribute>();
        return attribute == null ? value.ToString() : attribute.Description;
    }

    public static byte Id(this WarcraftVersion value)
    {
        return (byte)value;
    }
}
