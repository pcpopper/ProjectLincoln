using System;
using System.ComponentModel;
using System.Reflection;

namespace ProjectLincoln.Helpers {
    class EnumHelper {

        /// <summary>
        /// Gets the description string from the given enum value
        /// </summary>
        /// <see cref="http://blog.spontaneouspublicity.com/associating-strings-with-enums-in-c"/>
        /// <example>GetEnumDescription(States.NewMexico);</example>
        /// <param name="value">The enum value to get the description for</param>
        /// <returns>The description</returns>
        public static string GetEnumDescription (Enum value) {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        /// <summary>
        /// Gets the enum's value from the given description
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/4367723/get-enum-from-description-attribute"/>
        /// <example>GetValueFromDescription<Animal>("Giant Panda");</example>
        /// <typeparam name="T">The enum</typeparam>
        /// <param name="description">The description to get the value for</param>
        /// <returns>The value of the provided string</returns>
        public static T GetValueFromDescription<T>(string description) {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields()) {
                var attribute = Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null) {
                    if (attribute.Description == description)
                        return (T) field.GetValue(null);
                } else {
                    if (field.Name == description)
                        return (T) field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
            // or return default(T);
        }
    }
}
