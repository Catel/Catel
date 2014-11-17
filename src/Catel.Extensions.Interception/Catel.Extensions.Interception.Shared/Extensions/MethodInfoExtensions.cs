// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodInfoExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Interception;

    /// <summary>
    /// The <see cref="MethodInfo"/> extensions.
    /// </summary>
    public static class MethodInfoExtensions
    {
        #region Methods
        /// <summary>
        /// Selects the overridable methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <returns></returns>
        public static MethodInfo[] SelectOverridableMethods(this IEnumerable<MethodInfo> methods)
        {
            Argument.IsNotNull(() => methods);

            return methods.Where(method => method.IsVirtual && !method.IsFinal).ToArray();
        }

        /// <summary>
        /// Selects the getters.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> SelectGetters(this IEnumerable<MethodInfo> methods)
        {
            Argument.IsNotNull(() => methods);

            return methods.Where(method => method.Name.StartsWith("get_", StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        /// <summary>
        /// Selects the setters.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> SelectSetters(this IEnumerable<MethodInfo> methods)
        {
            Argument.IsNotNull(() => methods);

            return methods.Where(method => method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        /// <summary>
        /// Gets the method parameter types.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static Type[] GetMethodParameterTypes(this MethodInfo method)
        {
            Argument.IsNotNull(() => method);

            return method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
        }

        /// <summary>
        /// Extracts the definition.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        public static IMemberDefinition ExtractDefinition(this MethodInfo info)
        {
            Argument.IsNotNull(() => info);

            return new MemberDefinition(info.Name, info.GetMethodParameterTypes());
        }

        /// <summary>
        /// Gets the methods to intercept.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetMethodsToIntercept(this Type type)
        {
            Argument.IsNotNull(() => type);

            return type.IsInterface ?
                       type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                       : type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                             .SelectOverridableMethods();
        }
        #endregion
    }
}