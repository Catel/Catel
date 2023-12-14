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
            return del.Method;
        }
    }
}
