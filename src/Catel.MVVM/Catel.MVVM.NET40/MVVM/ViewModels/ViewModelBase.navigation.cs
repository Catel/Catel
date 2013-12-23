// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.navigation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;

    public partial class ViewModelBase
    {
        #region Fields
#if WINDOWS_PHONE
        private readonly Dictionary<string, string> _navigationContext = new Dictionary<string, string>();
#else
        private readonly Dictionary<string, object> _navigationContext = new Dictionary<string, object>();
#endif
        #endregion

        #region Constructors
        #endregion

        #region Properties
        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        /// <remarks>
        /// Note that the navigation contexts is first available in the <see cref="OnNavigationCompleted"/> method, 
        /// not in the constructor.
        /// </remarks>
        [ExcludeFromValidation]
#if WINDOWS_PHONE
        protected Dictionary<string, string> NavigationContext { get { return _navigationContext; } }
#else
        protected Dictionary<string, object> NavigationContext { get { return _navigationContext; } }
#endif

#if WINDOWS_PHONE

#endif
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the navigation is completed.
        /// </summary>
        /// <remarks>
        /// This should of course be a cleaner solution, but there is no other way to let a view-model
        /// know that navigation has completed. Another option is injection, but this would require every
        /// view-model for Windows Phone 7 to accept only the navigation context, which has actually nothing
        /// to do with the logic.
        /// <para />
        /// It is also possible to use the <see cref="OnNavigationCompleted"/> event.
        /// </remarks>
        public event EventHandler NavigationCompleted;
        #endregion

        #region Methods
        /// <summary>
        /// Updates the navigation context. The navigation context provided by this class is different
        /// from the <see cref="NavigationContext"/>. Therefore, this method updates the navigation context
        /// to match it to the values of the <paramref name="navigationContext"/>.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
#if SILVERLIGHT
        public void UpdateNavigationContext(Dictionary<string, string> navigationContext)
#else
        public void UpdateNavigationContext(Dictionary<string, object> navigationContext)
#endif
        {
            lock (_navigationContext)
            {
                _navigationContext.Clear();

                if (navigationContext != null)
                {
                    foreach (string key in navigationContext.Keys)
                    {
                        _navigationContext.Add(key, navigationContext[key]);
                    }
                }

                NavigationCompleted.SafeInvoke(this);

                OnNavigationCompleted();
            }
        }

        /// <summary>
        /// Called when the navigation has completed.
        /// </summary>
        /// <remarks>
        /// This should of course be a cleaner solution, but there is no other way to let a view-model
        /// know that navigation has completed. Another option is injection, but this would require every
        /// view-model for Windows Phone 7 to accept only the navigation context, which has actually nothing
        /// to do with the logic.
        /// <para />
        /// It is also possible to use the <see cref="NavigationCompleted"/> event.
        /// </remarks>
        protected virtual void OnNavigationCompleted() { }
        #endregion
    }
}