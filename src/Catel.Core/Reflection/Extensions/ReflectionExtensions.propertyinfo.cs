namespace Catel.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Reflection extension class.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        public static Attribute GetCustomAttributeEx(this PropertyInfo propertyInfo, Type attributeType, bool inherit)
        {
            var attributes = GetCustomAttributesEx(propertyInfo, attributeType, inherit);
            return (attributes.Length > 0) ? attributes[0] : null;
        }

        public static TAttribute GetCustomAttributeEx<TAttribute>(this PropertyInfo propertyInfo, bool inherit)
            where TAttribute : Attribute
        {
            var attributes = GetCustomAttributesEx(propertyInfo, typeof(TAttribute), inherit);
            return (attributes.Length > 0) ? (TAttribute)attributes[0] : default;
        }

        public static Attribute[] GetCustomAttributesEx(this PropertyInfo propertyInfo, bool inherit)
        {
            return propertyInfo.GetCustomAttributes(inherit).ToAttributeArray();
        }

        public static Attribute[] GetCustomAttributesEx(this PropertyInfo propertyInfo, Type attributeType, bool inherit)
        {
            return propertyInfo.GetCustomAttributes(attributeType, inherit).ToAttributeArray();
        }
    }
}
