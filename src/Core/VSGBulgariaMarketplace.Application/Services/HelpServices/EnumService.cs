namespace VSGBulgariaMarketplace.Application.Services.HelpServices
{
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    using VSGBulgariaMarketplace.Application.Constants;

    public static class EnumService
    {
        public static List<string> GetAll<T>() where T : Enum
        {
            List<string> enumStrings = new List<string>();

            var enumConstants = Enum.GetValues(typeof(T));

            foreach (T enumConstant in enumConstants)
            {
                string locationString = GetEnumDisplayName(enumConstant);
                enumStrings.Add(locationString);
            }

            return enumStrings;
        }

        public static string GetEnumDisplayName(Enum enumValue)
        {
            var enumMember = enumValue.GetType().GetMember(enumValue.ToString())[0];

            var displayAttribute = enumMember.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute != null ? displayAttribute.Name : enumValue.ToString();
        }

        public static T GetEnumValueFromDisplayName<T>(string displayName) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DisplayAttribute)) is DisplayAttribute attribute)
                {
                    if (attribute.Name == displayName)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == displayName)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException(string.Format(ServiceConstant.NO_ENUM_WITH_DISPLAY_NAME_FOUND_ERROR_MESSAGE, typeof(T).Name, displayName));
        }
    }
}
