// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Catel.Reflection
{
    using System;
    using System.Reflection;

#if NETFX_CORE || WP8 || NET45
    using System.Linq;
#endif

    /// <summary>
    /// Reflection extension class.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        public static Type[] GetTypesEx(this Assembly assembly)
        {
            Argument.IsNotNull("assembly", assembly);

#if NETFX_CORE || WP8
            return (from type in assembly.DefinedTypes
                    select type.AsType()).ToArray();
#else
            return assembly.GetTypes();
#endif
        }

        public static object GetCustomAttributeEx(this Assembly assembly, Type attributeType, bool inherit)
        {
            var attributes = GetCustomAttributesEx(assembly, attributeType, inherit);
            return (attributes.Length > 0) ? attributes[0] : null;
        }

        public static object[] GetCustomAttributesEx(this Assembly assembly, Type attributeType, bool inherit)
        {
            Argument.IsNotNull("assembly", assembly);
            Argument.IsNotNull("attributeType", attributeType);

#if NETFX_CORE || WP8
            return assembly.GetCustomAttributes(attributeType).ToArray<object>();
#else
            return assembly.GetCustomAttributes(attributeType, inherit);
#endif
        }
    }
}
