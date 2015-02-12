// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Extension methods for <see cref="Delegate"/>.
    /// </summary>
    public static class DelegateExtensions
    {
        /// <summary>
        /// Gets the method info of the delegate.
        /// </summary>
        /// <param name="del">The delegate.</param>
        /// <returns>The <see cref="MethodInfo"/> of the delegate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="del"/> is <c>null</c>.</exception>
        public static MethodInfo GetMethodInfoEx(this Delegate del)
        {
            Argument.IsNotNull("del", del);

#if NETFX_CORE || WP80 || NET45 || PCL
            return del.GetMethodInfo();
#else
            return del.Method;
#endif
        }
    }
}