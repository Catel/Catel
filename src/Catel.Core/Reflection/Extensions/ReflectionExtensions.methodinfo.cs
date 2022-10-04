namespace Catel.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Reflection extension class.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        public static Attribute? GetCustomAttributeEx(this MethodInfo methodInfo, Type attributeType, bool inherit)
        {
            var attributes = GetCustomAttributesEx(methodInfo, attributeType, inherit);
            return (attributes.Length > 0) ? attributes[0] : null;
        }

        public static Attribute[] GetCustomAttributesEx(this MethodInfo methodInfo, bool inherit)
        {
            return methodInfo.GetCustomAttributes(inherit).ToAttributeArray();
        }

        public static Attribute[] GetCustomAttributesEx(this MethodInfo methodInfo, Type attributeType, bool inherit)
        {
            return methodInfo.GetCustomAttributes(attributeType, inherit).ToAttributeArray();
        }
    }
}
