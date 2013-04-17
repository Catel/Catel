// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticMemberHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Diagnostics;

#if !NETFX_CORE
    using System.Runtime.CompilerServices;
#endif

    /// <summary>
    /// Helper class for static classes and members.
    /// </summary>
    public static class StaticHelper
    {
        /// <summary>
        /// Gets the type which is calling the current method which might be static. 
        /// </summary>
        /// <returns>The type calling the method.</returns>
#if !NETFX_CORE
        [MethodImpl(MethodImplOptions.NoInlining)]
#endif
        public static Type GetCallingType()
        {
            if (EnvironmentHelper.IsProcessHostedByTool)
            {
                return typeof(object);
            }


//#if NETFX_CORE
//            var type = typeof(object);
//#else
//            var stackTrace = StackTraceHelper.GetStackTrace();
//            var stackFrame = stackTrace.GetFrame(2);
//            var type = stackFrame.GetMethod().DeclaringType;
//#endif

#if NET
            var frame = new StackFrame(2, false);
            var type = frame.GetMethod().DeclaringType;
#elif NETFX_CORE
            var type = typeof(object);
#else
            var frame = new StackTrace().GetFrame(2);
            var type = frame.GetMethod().DeclaringType;
#endif

            return type;
        }
    }
}