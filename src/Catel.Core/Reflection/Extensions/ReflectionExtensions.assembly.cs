// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.assembly.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Catel.Reflection
{
    using System;
    using System.Reflection;

#if NETFX_CORE || NET || NETSTANDARD || PCL
    using System.Linq;
#endif

    /// <summary>
    /// Reflection extension class.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        public static Type[] GetExportedTypesEx(this Assembly assembly)
        {
            Argument.IsNotNull("assembly", assembly);

            Type[] results = null;

#if NETFX_CORE || PCL
            results = assembly.ExportedTypes.ToArray();
#else
            results = assembly.GetExportedTypes();
#endif

            return results;
        }

        public static Type[] GetTypesEx(this Assembly assembly)
        {
            Argument.IsNotNull("assembly", assembly);

            Type[] results = null;

#if NETFX_CORE || PCL
            results = (from type in assembly.DefinedTypes
                       select type.AsType()).ToArray();
#else
            results = assembly.GetTypes();
#endif

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

#if NETFX_CORE || PCL
            return assembly.GetCustomAttributes(attributeType).ToArray();
#else
            return assembly.GetCustomAttributes(attributeType, true).ToAttributeArray();
#endif
        }
    }
}
