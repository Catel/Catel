// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Windows;

    /// <summary>
    /// Helper class for silverlight hosts.
    /// </summary>
    public static class HostHelper
    {
        /// <summary>
        /// Gets the full url of a relative url to the current host.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <returns>The host url.</returns>
        /// <exception cref="ArgumentException">The <paramref name="relativeUrl"/> is <c>null</c> or whitespace.</exception>
        public static string GetHostUrl(string relativeUrl)
        {
            Argument.IsNotNullOrWhitespace("relativeUrl", relativeUrl);

            if (!relativeUrl.StartsWith("/"))
            {
                relativeUrl = "/" + relativeUrl;
            }

            var source = Application.Current.Host.Source;

            string hostXapSource = source.OriginalString;
            string hostXapFile = source.AbsolutePath;

            string host = hostXapSource;
            int index = hostXapSource.LastIndexOf(hostXapFile);
            if (index > 0)
            {
                host = hostXapSource.Substring(0, index);
            }

            return string.Format("{0}{1}", host, relativeUrl);
        }
    }
}