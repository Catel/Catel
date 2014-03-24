// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using Catel.MVVM.Views;
    

#if WINDOWS_PHONE
    using System.Windows;
    using Catel.Windows;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
#endif

#if XAMARIN || WINDOWS_PHONE
    /// <summary>
    /// Phone service.
    /// </summary>
    public class PhoneService : IPhoneService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneService"/> class.
        /// </summary>
        public PhoneService()
	    {
#if WINDOWS_PHONE
            StartupMode = (StartupMode)PhoneApplicationService.Current.StartupMode;
#endif
	    }

        /// <summary>
        /// Gets the startup mode.
        /// </summary>
        /// <value>The startup mode.</value>
        public StartupMode StartupMode { get; private set; }

        /// <summary>
        /// Determines whether the specified phone page can handle navigation.
        /// </summary>
        /// <param name="phonePage">The phone page.</param>
        /// <returns><c>true</c> if this instance can handle navigation; otherwise, <c>false</c>.</returns>
        public bool CanHandleNavigation(IPhonePage phonePage)
        {
            if (phonePage == null)
            {
                return false;
            }

            var rootFrame = Application.Current.RootVisual.FindVisualDescendant(e => e is PhoneApplicationFrame) as PhoneApplicationFrame;
            if (rootFrame == null)
            {
                return false;
            }

            var content = rootFrame.Content;
            return ReferenceEquals(content, phonePage);
        }


    }
#endif
}