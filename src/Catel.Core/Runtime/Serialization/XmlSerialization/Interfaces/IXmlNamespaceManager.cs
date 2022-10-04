namespace Catel.Runtime.Serialization.Xml
{
    using System;

    /// <summary>
    /// Manages the xml namespaces for a specific type.
    /// </summary>
    public interface IXmlNamespaceManager
    {
        /// <summary>
        /// Gets the namespace for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="preferredPrefix">The preferred prefix.</param>
        /// <returns>The xml namespace.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="preferredPrefix"/> is <c>null</c> or whitespace.</exception>
        XmlNamespace? GetNamespace(Type type, string preferredPrefix);
    }
}
