namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using Logging;

    /// <summary>
    /// The state service which can store and restore states.
    /// </summary>
    public class StateService : IStateService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, IState?> _states = new Dictionary<string, IState?>();

        /// <summary>
        /// Stores the state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="state">The state.</param>
        public void StoreState(string key, IState? state)
        {
            ArgumentNullException.ThrowIfNull(key);

            lock (_states)
            {
                Log.Debug($"Storing state '{key}'");

                _states[key] = state;
            }
        }

        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public IState? LoadState(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            lock (_states)
            {
                if (_states.TryGetValue(key, out var state))
                {
                    Log.Debug($"Loaded state '{key}'");

                    return state;
                }

                Log.Debug($"State '{key}' not found");

                return null;
            }
        }
    }
}
