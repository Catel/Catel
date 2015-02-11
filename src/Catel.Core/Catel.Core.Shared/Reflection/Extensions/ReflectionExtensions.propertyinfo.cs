// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.propertyinfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Catel.Reflection
{
    using System;
    using System.Reflection;

#if NETFX_CORE || PCL
    using System.Linq;
#endif

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

        public static Attribute[] GetCustomAttributesEx(this PropertyInfo propertyInfo, bool inherit)
        {
            Argument.IsNotNull("propertyInfo", propertyInfo);

#if NETFX_CORE || PCL
            return propertyInfo.GetCustomAttributes(inherit).ToArray();
#else
            return propertyInfo.GetCustomAttributes(inherit).ToAttributeArray();
#endif
        }

        public static Attribute[] GetCustomAttributesEx(this PropertyInfo propertyInfo, Type attributeType, bool inherit)
        {
            Argument.IsNotNull("propertyInfo", propertyInfo);
            Argument.IsNotNull("attributeType", attributeType);

#if NETFX_CORE || PCL
            return propertyInfo.GetCustomAttributes(attributeType, inherit).ToArray();
#else
            return propertyInfo.GetCustomAttributes(attributeType, inherit).ToAttributeArray();
#endif
        }
    }
}
