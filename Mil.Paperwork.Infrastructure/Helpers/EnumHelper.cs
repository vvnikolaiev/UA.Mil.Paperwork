using System.ComponentModel;
using System.Reflection;

namespace Mil.Paperwork.Infrastructure.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// Gets the description of an enum value, or its name if no description is set.
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attr = field?.GetCustomAttribute<DescriptionAttribute>();
            var result = attr?.Description ?? value.ToString();

            return result;
        }

        public static IEnumerable<T> GetValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Returns all values of the enum with their descriptions.
        /// </summary>
        public static IEnumerable<(T Value, string Description)> GetValuesWithDescriptions<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                       .Select(v => (v, GetDescription(v)));
        }

        /// <summary>
        /// Returns a dictionary: enum value → description.
        /// </summary>
        public static Dictionary<T, string> GetDescriptionDictionary<T>() where T : Enum
        {
            return GetValuesWithDescriptions<T>().ToDictionary(t => t.Value, t => t.Description);
        }
    }
}
