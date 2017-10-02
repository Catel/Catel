// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Threading
{
#if NET
    using System;
    using System.Windows.Threading;
#endif

    /// <summary>
    /// Extension methods for the dispatcher.
    /// </summary>
    public static partial class DispatcherExtensions
    {
#if NET
        /// <summary>
        /// Gets the managed thread identifier for the specified dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <returns>The managed thread id.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcher" /> is <c>null</c>.</exception>
        public static int GetThreadId(this Dispatcher dispatcher)
        {
            Argument.IsNotNull("dispatcher", dispatcher);

            return dispatcher.Thread.ManagedThreadId;
        }
#endif
    }
}

#endif