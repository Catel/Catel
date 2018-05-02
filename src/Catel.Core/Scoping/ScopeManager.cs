// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Scoping
{
    using System;
    using System.Collections.Generic;
    using IoC;
    using Logging;
    using Reflection;

    /// <summary>
    /// Custom scope manager to define a scope for a type.
    /// </summary>
    /// <typeparam name="T">The type to scope.</typeparam>
    public class ScopeManager<T> : IDisposable
        where T : class
    {
        #region Constants
        private static readonly object _lock = new object();
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly string TypeName;

        private static Dictionary<string, object> _instances = new Dictionary<string, object>();
        #endregion

        #region Fields
        private readonly string _scopeName;
        private T _scopeObject;
        private int _refCount;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="ScopeManager{T}"/> class.
        /// </summary>
        static ScopeManager()
        {
            TypeName = typeof(T).GetSafeFullName(false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeManager{T}" /> class.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        /// <param name="createScopeFunction">The create scope function.</param>
        protected ScopeManager(string scopeName, Func<T> createScopeFunction)
        {
            _scopeName = scopeName;

            if (createScopeFunction != null)
            {
                Log.Debug("Custom function to create the scope is provided, creating custom scope for type '{0}' with name '{1}'", TypeName, _scopeName);

                _scopeObject = createScopeFunction();
            }
            else
            {
                Log.Debug("No custom function to create the scope is provided, creating custom scope for type '{0}' with name '{1}' using TypeFactory", TypeName, _scopeName);

                var typeFactory = this.GetTypeFactory();
                _scopeObject = typeFactory.CreateInstance<T>();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Occurs when the scope reference count reaches zero.
        /// </summary>
        public event EventHandler<ScopeClosedEventArgs> ScopeClosed;

        /// <summary>
        /// Gets the scope object.
        /// </summary>
        public T ScopeObject
        {
            get
            {
                lock (_lock)
                {
                    return _scopeObject;
                }
            }
        }

        /// <summary>
        /// Gets the current reference count for this
        /// object.
        /// </summary>
        public int RefCount
        {
            get
            {
                lock (_lock)
                {
                    return _refCount;
                }
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose object, dereferencing or disposing the context it is managing.
        /// </summary>
        public void Dispose()
        {
            DeRef();
        }
        #endregion

        #region Methods
        private void AddRef()
        {
            lock (_lock)
            {
                _refCount += 1;

                Log.Debug("Referencing type '{0}' with scope name '{1}', new ref count is {2}", TypeName, _scopeName, _refCount);
            }
        }

        private void DeRef()
        {
            lock (_lock)
            {
                _refCount -= 1;

                Log.Debug("Dereferencing type '{0}' with scope name '{1}', new ref count is {2}", TypeName, _scopeName, _refCount);

                if (_refCount == 0)
                {
                    Log.Debug("Type '{0}' with scope name '{1}' has reached a ref count of 0, scope is closed now", TypeName, _scopeName);

                    var scopeObjectAsDisposable = _scopeObject as IDisposable;
                    if (scopeObjectAsDisposable != null)
                    {
                        scopeObjectAsDisposable.Dispose();
                    }

                    _scopeObject = null;

                    _instances.Remove(_scopeName);

                    var scopeClosed = ScopeClosed;
                    if (scopeClosed != null)
                    {
                        scopeClosed.Invoke(this, new ScopeClosedEventArgs(ScopeObject, _scopeName));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the specified scope exists.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        /// <returns><c>true</c> if the scope exists, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentException">The <paramref name="scopeName"/> is <c>null</c>.</exception>
        public static bool ScopeExists(string scopeName = "")
        {
            Argument.IsNotNull("scopeName", scopeName);

            lock (_lock)
            {
                return _instances.ContainsKey(scopeName);
            }
        }

        /// <summary>
        /// Gets the ContextManager object for the specified database.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        /// <param name="createScopeFunction">The create scope function. Can be <c>null</c>.</param>
        /// <returns>The <see cref="ScopeManager{T}" />.</returns>
        /// <exception cref="ArgumentException">The <paramref name="scopeName"/> is <c>null</c>.</exception>
        public static ScopeManager<T> GetScopeManager(string scopeName = "", Func<T> createScopeFunction = null)
        {
            Argument.IsNotNull("scopeName", scopeName);

            lock (_lock)
            {
                ScopeManager<T> scopeManager;

                object scopeManagerStoredInstance;
                if (_instances.TryGetValue(scopeName, out scopeManagerStoredInstance))
                {
                    Log.Debug("Returning existing scope for type '{0}' with name '{1}'", TypeName, scopeName);

                    scopeManager = (ScopeManager<T>) scopeManagerStoredInstance;
                }
                else
                {
                    Log.Debug("Creating new scope for type '{0}' with name '{1}'", TypeName, scopeName);

                    scopeManager = new ScopeManager<T>(scopeName, createScopeFunction);
                    _instances[scopeName] = scopeManager;
                }

                scopeManager.AddRef();
                return scopeManager;
            }
        }
        #endregion
    }
}