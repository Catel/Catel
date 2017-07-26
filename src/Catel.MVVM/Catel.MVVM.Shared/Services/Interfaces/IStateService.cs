// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStateService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    /// <summary>
    /// The state service which can store and restore states.
    /// </summary>
    public interface IStateService
    {
        #region Methods
        /// <summary>
        /// Stores the state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="state">The state.</param>
        void StoreState(string key, IState state);

        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        IState LoadState(string key);
        #endregion
    }
}