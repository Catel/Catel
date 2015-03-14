// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigatingEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Navigation
{
    /// <summary>
    /// Navigating event args.
    /// </summary>
    public class NavigatingEventArgs : NavigationEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatedEventArgs"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="navigationMode">The navigation mode.</param>
        public NavigatingEventArgs(string uri, NavigationMode navigationMode)
            : base(uri, navigationMode)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the navigation should be canceled.
        /// </summary>
        /// <value><c>true</c> if the navigation should cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }
    }
}