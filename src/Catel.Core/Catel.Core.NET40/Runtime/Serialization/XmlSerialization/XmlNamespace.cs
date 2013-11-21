// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlNamespace.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Xml
{
    using System;

    /// <summary>
    /// Contains information about an xml namespace.
    /// </summary>
    public class XmlNamespace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlNamespace"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="uri">The URI.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="prefix"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c> or whitespace.</exception>
        public XmlNamespace(string prefix, string uri)
        {
            Argument.IsNotNullOrWhitespace("prefix", prefix);
            Argument.IsNotNullOrWhitespace("uri", uri);

            Prefix = prefix;
            Uri = uri;
        }

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        public string Prefix { get; private set; }

        /// <summary>
        /// Gets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", Prefix, Uri);
        }
    }
}