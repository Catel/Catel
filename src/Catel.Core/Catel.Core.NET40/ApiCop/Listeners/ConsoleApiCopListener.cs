// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleApiCopListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop.Listeners
{
    using System;

    /// <summary>
    /// <see cref="IApiCopListener"/> implementation which writes all results to the console.
    /// </summary>
    public class ConsoleApiCopListener : TextApiCopListenerBase
    {
        /// <summary>
        /// Writes the line the to final output.
        /// </summary>
        /// <param name="line">The line.</param>
        protected override void WriteLine(string line)
        {
#if NET
            System.Diagnostics.Trace.WriteLine(line);
#elif SILVERLIGHT
            Console.WriteLine(line);
#else
            System.Diagnostics.Debug.WriteLine(line);
#endif
        }
    }
}