// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStateServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
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
        public static TState LoadState<TState>(this IStateService stateService, string key)
            where TState : class, IState
        {
            var state = stateService.LoadState(key);

            var typedState = state as TState;
            return typedState;
        }
    }
}