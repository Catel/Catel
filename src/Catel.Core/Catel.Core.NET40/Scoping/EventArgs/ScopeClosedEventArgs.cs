// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeClosedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Scoping
{
    using System;

    /// <summary>
    /// EventArgs for the <see cref="ScopeManager{T}.ScopeClosed"/> event.
    /// </summary>
    public class ScopeClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeClosedEventArgs" /> class.
        /// </summary>
        /// <param name="scopeObject">The scope object.</param>
        /// <param name="scopeName">Name of the scope.</param>
        public ScopeClosedEventArgs(object scopeObject, string scopeName)
        {
            ScopeObject = scopeObject;
            ScopeName = scopeName;
        }

        /// <summary>
        /// Gets the scope object.
        /// </summary>
        /// <value>The scope object.</value>
        public object ScopeObject { get; private set; }

        /// <summary>
        /// Gets the name of the scope.
        /// </summary>
        /// <value>The name of the scope.</value>
        public string ScopeName { get; private set; }
    }
}