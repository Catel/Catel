// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDisposableToken.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;

    /// <summary>
    /// A reusable disposable token that accepts initialization and uninitialization code.
    /// </summary>
    public interface IDisposableToken<T> : IDisposable
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        T Instance { get; }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        object Tag { get; }
    }
}