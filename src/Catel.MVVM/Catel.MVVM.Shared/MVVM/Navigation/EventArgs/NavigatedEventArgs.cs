// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigatedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Navigation
{
    /// <summary>
    /// Navigated event args.
    /// </summary>
    public class NavigatedEventArgs : NavigationEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatedEventArgs" /> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="navigationMode">The navigation mode.</param>
        public NavigatedEventArgs(string uri, NavigationMode navigationMode)
            : base(uri, navigationMode)
        {
        }
    }
}