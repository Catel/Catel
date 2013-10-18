// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.methodinfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Catel.Reflection
{
    using System;
    using System.Reflection;

#if NETFX_CORE
    using System.Linq;
#endif

    /// <summary>
    /// Reflection extension class.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        public static Attribute GetCustomAttributeEx(this MethodInfo methodInfo, Type attributeType, bool inherit)
        {
            var attributes = GetCustomAttributesEx(methodInfo, attributeType, inherit);
            return (attributes.Length > 0) ? attributes[0] : null;
        }

        public static Attribute[] GetCustomAttributesEx(this MethodInfo methodInfo, bool inherit)
        {
            Argument.IsNotNull("methodInfo", methodInfo);

#if NETFX_CORE
            return methodInfo.GetCustomAttributes(inherit).ToArray();
#else
            return methodInfo.GetCustomAttributes(inherit).ToAttributeArray();
#endif
        }

        public static Attribute[] GetCustomAttributesEx(this MethodInfo methodInfo, Type attributeType, bool inherit)
        {
            Argument.IsNotNull("methodInfo", methodInfo);
            Argument.IsNotNull("attributeType", attributeType);

#if NETFX_CORE
            return methodInfo.GetCustomAttributes(attributeType, inherit).ToArray();
#else
            return methodInfo.GetCustomAttributes(attributeType, inherit).ToAttributeArray();
#endif
        }
    }
}
