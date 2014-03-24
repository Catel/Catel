// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPhoneService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using Catel.MVVM;
    using Catel.MVVM.Views;

    /// <summary>
    /// Available tombstoning modes.
    /// </summary>
    public enum TombstoningMode
    {
        /// <summary>
        /// The view model will store and recover all values of all view model properties automatically.
        /// </summary>
        /// <remarks>
        /// This mode is not yet supported!
        /// </remarks>
        Auto,

        /// <summary>
        /// Tombstoning will be handled manually by the developer of the view models.
        /// </summary>
        Manual,

        /// <summary>
        /// Tombstoning capabilities are fully disabled for the view model.
        /// </summary>
        Disabled
    }

    /// <summary>
    /// Available startup modes.
    /// </summary>
    public enum StartupMode
    {
        /// <summary>
        /// The app is activated again.
        /// </summary>
        Activate = 1,

        /// <summary>
        /// The app is launched.
        /// </summary>
        Launch = 2
    }

    /// <summary>
    /// Phone service representing generic phone logic.
    /// </summary>
    public interface IPhoneService
    {
        /// <summary>
        /// Gets the startup mode.
        /// </summary>
        /// <value>The startup mode.</value>
        StartupMode StartupMode { get; }

        /// <summary>
        /// Determines whether the specified phone page can handle navigation.
        /// </summary>
        /// <param name="phonePage">The phone page.</param>
        /// <returns><c>true</c> if this instance can handle navigation; otherwise, <c>false</c>.</returns>
        bool CanHandleNavigation(IPhonePage phonePage);
    }
}