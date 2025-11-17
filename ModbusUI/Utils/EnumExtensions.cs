using System.ComponentModel;
using System.Reflection;

namespace ModbusUI.Utils
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }

        public static List<object> ToList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new
                {
                    Value = e,
                    Text = (e as Enum).GetDescription()
                })
                .Cast<object>()
                .ToList();
        }
    }
}
