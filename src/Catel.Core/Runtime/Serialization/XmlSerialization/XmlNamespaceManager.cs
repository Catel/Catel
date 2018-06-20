// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlNamespaceManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Catel.Reflection;
    using Catel.Scoping;

    /// <summary>
    /// Manages the namespaces based on the current serialization scope.
    /// </summary>
    /// <remarks>
    /// Note that this class is not thread-safe. Serialization on it's own is not thread-safe because serialization
    /// of a single object should happen on the same thread.
    /// </remarks>
    public class XmlNamespaceManager : IXmlNamespaceManager
    {
        private const string ArraySchemaUrl = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
        private const string ArraySchemaName = "arr";
        private const string NamespaceUriPrefix = "http://schemas.datacontract.org/2004/07/";

        private readonly Dictionary<string, XmlScopeNamespaceInfo> _scopeInfo = new Dictionary<string, XmlScopeNamespaceInfo>();

        /// <summary>
        /// Gets the namespace for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="preferredPrefix">The preferred prefix.</param>
        /// <returns>The xml namespace.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="preferredPrefix"/> is <c>null</c> or whitespace.</exception>
        public XmlNamespace GetNamespace(Type type, string preferredPrefix)
        {
            Argument.IsNotNull("type", type);

            var scopeName = SerializationContextHelper.GetSerializationReferenceManagerScopeName();
            using (var scopeManager = ScopeManager<SerializationContextScope<XmlSerializationContextInfo>>.GetScopeManager(scopeName))
            {
                EnsureSubscribedToScope(scopeManager, scopeName);

                var scopeInfo = _scopeInfo[scopeName];
                return scopeInfo.GetNamespace(type, preferredPrefix);
            }
        }

        private void EnsureSubscribedToScope(ScopeManager<SerializationContextScope<XmlSerializationContextInfo>> scopeManager, string scopeName)
        {
            if (!_scopeInfo.ContainsKey(scopeName))
            {
                _scopeInfo.Add(scopeName, new XmlScopeNamespaceInfo(scopeName));
                scopeManager.ScopeClosed += OnScopeClosed;
            }
        }

        private void OnScopeClosed(object sender, ScopeClosedEventArgs e)
        {
            _scopeInfo.Remove(e.ScopeName);

            var scopeManager = (ScopeManager<SerializationContextScope<XmlSerializationContextInfo>>)sender;
            scopeManager.ScopeClosed -= OnScopeClosed;
        }

        private class XmlScopeNamespaceInfo
        {
            private readonly Dictionary<Type, XmlNamespace> _xmlNamespaces = new Dictionary<Type, XmlNamespace>();
            //private readonly Dictionary<string, XmlNamespace> _xmlNamespacesByDotNetNamespace = new Dictionary<string, XmlNamespace>();
            private readonly Dictionary<string, int> _prefixCounter = new Dictionary<string, int>();

            public XmlScopeNamespaceInfo(string scopeName)
            {
                ScopeName = scopeName;
            }

            public string ScopeName { get; private set; }

            public XmlNamespace GetNamespace(Type type, string preferredPrefix)
            {
                if (!_xmlNamespaces.TryGetValue(type, out var xmlNamespace))
                {
                    xmlNamespace = GetTypeNamespace(type, preferredPrefix);
                    _xmlNamespaces[type] = xmlNamespace;
                }

                return xmlNamespace;
            }

            private XmlNamespace GetTypeNamespace(Type type, string preferredPrefix)
            {
                var typeNamespace = type.Namespace;
                //if (_xmlNamespacesByDotNetNamespace.ContainsKey(typeNamespace))
                //{
                //    return _xmlNamespacesByDotNetNamespace[typeNamespace];
                //}

                var prefix = preferredPrefix;
                var uri = string.Format("{0}{1}", NamespaceUriPrefix, typeNamespace);

                if (type.IsBasicType())
                {
                    return null;
                }

                if (type != typeof(string) && typeof (IEnumerable).IsAssignableFromEx(type))
                {
                    prefix = ArraySchemaName;
                    uri = ArraySchemaUrl;
                }
                else
                {
                    if (!_prefixCounter.ContainsKey(preferredPrefix))
                    {
                        _prefixCounter[preferredPrefix] = 1;
                    }

                    prefix = string.Format("{0}{1}", prefix, _prefixCounter[preferredPrefix]++);
                }

                // TODO: Read xml namespace from attribute

                var xmlNamespace = new XmlNamespace(prefix, uri);

                _xmlNamespaces.Add(type, xmlNamespace);
                //_xmlNamespacesByDotNetNamespace.Add(typeNamespace, xmlNamespace);

                return xmlNamespace;
            }
        }
    }
}
