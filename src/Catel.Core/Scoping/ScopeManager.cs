//#define EXTREME_LOGGING

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
        private static readonly object _lock = new object();
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly string TypeName;

        private static readonly Dictionary<string, object> _instances = new Dictionary<string, object>();

        private readonly string _scopeName;
        private T _scopeObject;
        private int _refCount;

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

            if (createScopeFunction is not null)
            {
                Log.Debug($"Custom function to create the scope is provided, creating custom scope for type '{TypeName}' with name '{_scopeName}'");

                _scopeObject = createScopeFunction();
            }
            else
            {
                Log.Debug($"No custom function to create the scope is provided, creating custom scope for type '{TypeName}' with name '{_scopeName}' using TypeFactory");

                var typeFactory = this.GetTypeFactory();
                _scopeObject = typeFactory.CreateInstance<T>();
            }
        }

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
        /// Gets the current reference count for this object.
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

        /// <summary>
        /// Dispose object, dereferencing or disposing the object it is managing.
        /// </summary>
        public virtual void Dispose()
        {
            DeRef();
        }

        private void AddRef()
        {
            lock (_lock)
            {
                _refCount += 1;

#if EXTREME_LOGGING
                Log.Debug($"Referencing type '{TypeName}' with scope name '{_scopeName}', new ref count is {_refCount}");
#endif
            }
        }

        private void DeRef()
        {
            lock (_lock)
            {
                _refCount -= 1;

#if EXTREME_LOGGING
                Log.Debug($"Dereferencing type '{TypeName}' with scope name '{_scopeName}', new ref count is {_refCount}");
#endif

                if (_refCount == 0)
                {
                    Log.Debug($"Type '{TypeName}' with scope name '{_scopeName}' has reached a ref count of 0, scope is closed now");

                    var scopeObjectAsDisposable = _scopeObject as IDisposable;
                    if (scopeObjectAsDisposable is not null)
                    {
                        scopeObjectAsDisposable.Dispose();
                    }

                    _scopeObject = null;

                    _instances.Remove(_scopeName);

                    var scopeClosed = ScopeClosed;
                    if (scopeClosed is not null)
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
            lock (_lock)
            {
                return _instances.ContainsKey(scopeName);
            }
        }

        /// <summary>
        /// Gets the ScopeManager for the specified scope name.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        /// <param name="createScopeFunction">The create scope function. Can be <c>null</c>.</param>
        /// <returns>The <see cref="ScopeManager{T}" />.</returns>
        /// <exception cref="ArgumentException">The <paramref name="scopeName"/> is <c>null</c>.</exception>
        public static ScopeManager<T> GetScopeManager(string scopeName = "", Func<T> createScopeFunction = null)
        {
            lock (_lock)
            {
                ScopeManager<T> scopeManager;

                if (_instances.TryGetValue(scopeName, out var scopeManagerStoredInstance))
                {
#if EXTREME_LOGGING
                    Log.Debug($"Returning existing scope for type '{TypeName}' with name '{scopeName}'");
#endif

                    scopeManager = (ScopeManager<T>)scopeManagerStoredInstance;
                }
                else
                {
                    Log.Debug($"Creating new scope for type '{TypeName}' with name '{scopeName}'");

#pragma warning disable IDISP001 // Dispose created.
                    scopeManager = new ScopeManager<T>(scopeName, createScopeFunction);
#pragma warning restore IDISP001 // Dispose created.

                    _instances[scopeName] = scopeManager;
                }

                scopeManager.AddRef();
                return scopeManager;
            }
        }
    }
}
