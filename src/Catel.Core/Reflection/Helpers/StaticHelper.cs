namespace Catel.Reflection
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Helper class for static classes and members.
    /// </summary>
    public static class StaticHelper
    {
        /// <summary>
        /// Gets the type which is calling the current method which might be static. 
        /// </summary>
        /// <returns>The type calling the method.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Type GetCallingType()
        {
            var frame = new StackFrame(2, false);
            var type = frame.GetMethod().DeclaringType;

            return type;
        }
    }
}
