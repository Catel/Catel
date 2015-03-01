// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiCopManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Catel.Reflection;

    /// <summary>
    /// Manager class for the ApiCop feature.
    /// </summary>
    public static class ApiCopManager
    {
        #region Constants
        private static readonly List<IApiCopListener> _listeners = new List<IApiCopListener>();

        private static readonly Dictionary<Type, IApiCop> _cops = new Dictionary<Type, IApiCop>();

        private static readonly IApiCop _dummyApiCop = new ApiCop(typeof(object));
        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="ApiCopManager"/> class.
        /// </summary>
        static ApiCopManager()
        {
            IgnoredRules = new HashSet<string>();

            IsEnabled = Debugger.IsAttached;
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether ApiCop is enabled.
        /// </summary>
        /// <value><c>true</c> if ApiCop is enabled; otherwise, <c>false</c>.</value>
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// Gets the ignored rules.
        /// </summary>
        /// <value>The ignored rules.</value>
        public static HashSet<string> IgnoredRules { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the current class ApiCop.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IApiCop GetCurrentClassApiCop()
        {
            if (!IsEnabled)
            {
                return _dummyApiCop;
            }

            var callingType = StaticHelper.GetCallingType();

            return GetApiCop(callingType);
        }

        /// <summary>
        /// Gets the ApiCop for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static IApiCop GetApiCop(Type type)
        {
            if (!IsEnabled)
            {
                return _dummyApiCop;
            }

            Argument.IsNotNull("type", type);

            lock (_cops)
            {
                if (!_cops.ContainsKey(type))
                {
                    var cop = new ApiCop(type);

                    _cops.Add(type, cop);
                }

                return _cops[type];
            }
        }

        /// <summary>
        /// Gets all the currently registered listeners.
        /// </summary>
        /// <returns>An enumerable of all listeners.</returns>
        public static IEnumerable<IApiCopListener> GetListeners()
        {
            lock (_listeners)
            {
                return _listeners;
            }
        }

        /// <summary>
        /// Adds the ApiCop listener which will receive all ApiCop information.
        /// <para />
        /// This method does not check whether the <paramref name="listener"/> is already added to the list
        /// of registered listeners.
        /// </summary>
        /// <param name="listener">The listener.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="listener"/> is <c>null</c>.</exception>
        public static void AddListener(IApiCopListener listener)
        {
            if (!IsEnabled)
            {
                return;
            }

            Argument.IsNotNull("listener", listener);

            lock (_listeners)
            {
                _listeners.Add(listener);
            }
        }

        /// <summary>
        /// Removes the ApiCop which will stop receiving all ApiCop information.
        /// </summary>
        /// <param name="listener">The listener.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="listener"/> is <c>null</c>.</exception>
        public static void RemoveListener(IApiCopListener listener)
        {
            Argument.IsNotNull("listener", listener);

            lock (_listeners)
            {
                _listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Determines whether the specified listener is already registered or not.
        /// </summary>
        /// <param name="listener">The listener.</param>
        /// <returns>
        /// <c>true</c> if the specified listener is already registered; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="listener"/> is <c>null</c>.</exception>
        public static bool IsListenerRegistered(IApiCopListener listener)
        {
            Argument.IsNotNull("listener", listener);

            lock (_listeners)
            {
                return _listeners.Contains(listener);
            }
        }

        /// <summary>
        /// Clears all the current listeners.
        /// </summary>
        public static void ClearListeners()
        {
            lock (_listeners)
            {
                _listeners.Clear();
            }
        }

        /// <summary>
        /// Writes the results to all the registered listeners.
        /// </summary>
        public static void WriteResults()
        {
            lock (_listeners)
            {
                if (_listeners.Count == 0)
                {
                    return;
                }

                var results = new List<IApiCopResult>();
                foreach (var cop in _cops)
                {
                    var copResults = cop.Value.GetResults();
                    foreach (var copResult in copResults)
                    {
                        if (!IgnoredRules.Contains(copResult.Rule.Name))
                        {
                            results.Add(copResult);
                        }
                    }
                }

                foreach (var listener in _listeners)
                {
                    listener.WriteResults(results);
                }
            }
        }
        #endregion
    }
}