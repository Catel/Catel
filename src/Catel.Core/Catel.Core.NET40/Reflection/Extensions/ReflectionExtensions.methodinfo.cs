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

#if NETFX_CORE || WP8
    using System.Linq;
#endif

    /// <summary>
    /// Reflection extension class.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        public static object GetCustomAttributeEx(this MethodInfo methodInfo, Type attributeType, bool inherit)
        {
            var attributes = GetCustomAttributesEx(methodInfo, attributeType, inherit);
            return (attributes.Length > 0) ? attributes[0] : null;
        }

        public static object[] GetCustomAttributesEx(this MethodInfo methodInfo, Type attributeType, bool inherit)
        {
            Argument.IsNotNull("methodInfo", methodInfo);
            Argument.IsNotNull("attributeType", attributeType);

#if NETFX_CORE || WP8
            return methodInfo.GetCustomAttributes(attributeType).ToArray<object>();
#else
            return methodInfo.GetCustomAttributes(attributeType, inherit);
#endif
        }
    }
}
