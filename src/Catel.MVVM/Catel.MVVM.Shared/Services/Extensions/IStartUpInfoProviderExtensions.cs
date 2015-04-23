// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStartUpInfoProviderExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



#if NET

namespace Catel.Services
{
    using System;
    using System.Linq;

    /// <summary>
    /// Extension methods for the startup info provider.
    /// </summary>
    public static class IStartUpInfoProviderExtensions
    {
        #region Methods
        /// <summary>
        /// Gets the command line as a string and quotes the values with a space.
        /// </summary>
        /// <param name="startUpInfoProvider">The start up information provider.</param>
        /// <returns>The command line as string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="startUpInfoProvider"/> is <c>null</c>.</exception>
        public static string GetCommandLine(this IStartUpInfoProvider startUpInfoProvider)
        {
            Argument.IsNotNull(() => startUpInfoProvider);

            var commandLine = string.Join(" ", startUpInfoProvider.Arguments.Select(x => x.Contains(" ") ? string.Format("\"{0}\"", x) : x));
            return commandLine;
        }
        #endregion
    }
}

#endif