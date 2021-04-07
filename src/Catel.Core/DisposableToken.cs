// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableToken.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;

    /// <summary>
    /// A reusable disposable token that accepts initialization and uninitialization code.
    /// </summary>
    public class DisposableToken : DisposableToken<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableToken" /> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="initialize">The initialize action.</param>
        /// <param name="dispose">The dispose action.</param>
        /// <param name="tag">The tag.</param>
        public DisposableToken(object instance, Action<IDisposableToken<object>> initialize, Action<IDisposableToken<object>> dispose, object tag = null) 
            : base(instance, initialize, dispose, tag)
        {
        }
    }

    /// <summary>
    /// A reusable disposable token that accepts initialization and uninitialization code.
    /// </summary>
    public class DisposableToken<T> : Disposable, IDisposableToken<T>
    {
        #region Fields
        private T _instance;
        private Action<IDisposableToken<T>> _dispose;
        private object _tag;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableToken{T}" /> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="initialize">The initialize action that will be called with (token).</param>
        /// <param name="dispose">The dispose action that will be called with (instance, tag).</param>
        /// <param name="tag">The tag.</param>
        public DisposableToken(T instance, Action<IDisposableToken<T>> initialize, Action<IDisposableToken<T>> dispose, object tag = null)
        {
            _instance = instance;
            _dispose = dispose;
            _tag = tag;

            if (initialize is not null)
            {
                initialize(this);
            }
        }
        #endregion

        /// <summary>
        /// Gets the instance attached to this token.
        /// </summary>
        /// <value>The instance.</value>
        public T Instance { get { return _instance; } }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get { return _tag; } }

        #region IDisposable Members
        protected override void DisposeManaged()
        {
            if (_dispose is not null)
            {
                _dispose(this);
            }

            _instance = default(T);
            _dispose = null;
            _tag = null;
        }
        #endregion
    }
}
