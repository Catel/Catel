namespace Catel.Reflection
{
    using MethodTimer;
    using System;
    using System.Reflection;

    /// <summary>
    /// Reflection extension class.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        public static Type[] GetExportedTypesEx(this Assembly assembly)
        {
            Argument.IsNotNull("assembly", assembly);

            var results = assembly.GetExportedTypes();
            return results;
        }

#if DEBUG
        [Time("{assembly}")]
#endif
        public static Type[] GetTypesEx(this Assembly assembly)
        {
            Argument.IsNotNull("assembly", assembly);

            var results = assembly.GetTypes();
            return results;
        }

        public static Attribute GetCustomAttributeEx(this Assembly assembly, Type attributeType)
        {
            var attributes = GetCustomAttributesEx(assembly, attributeType);
            return (attributes.Length > 0) ? attributes[0] : null;
        }

        public static Attribute[] GetCustomAttributesEx(this Assembly assembly, Type attributeType)
        {
            Argument.IsNotNull("assembly", assembly);
            Argument.IsNotNull("attributeType", attributeType);

            return assembly.GetCustomAttributes(attributeType, true).ToAttributeArray();
        }
    }
}
