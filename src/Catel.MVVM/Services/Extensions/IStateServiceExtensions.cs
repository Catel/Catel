namespace Catel.Services
{
    using System;

    /// <summary>
    /// Extensions for the state service.
    /// </summary>
    public static class IStateServiceExtensions
    {
        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="stateService">The state service.</param>
        /// <param name="key">The key.</param>
        /// <returns>The state or <c>null</c> if no state is found.</returns>
        public static TState? LoadState<TState>(this IStateService stateService, string key)
            where TState : class, IState
        {
            ArgumentNullException.ThrowIfNull(stateService);

            var state = stateService.LoadState(key);

            var typedState = state as TState;
            return typedState;
        }
    }
}
