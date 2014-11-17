// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Authentication.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !WIN80 && !XAMARIN

namespace Catel.Windows.Interactivity
{
#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using UIEventArgs = System.EventArgs;
#endif

    using System;
    using IoC;
    using Logging;
    using MVVM;

    /// <summary>
    /// The available actions to perform when a user is not able to view a specific UI element.
    /// </summary>
    public enum AuthenticationAction
    {
#if NET
        /// <summary>
        /// Hides the associated control.
        /// </summary>
        Hide,
#endif

        /// <summary>
        /// Collapses the associated control.
        /// </summary>
        Collapse,

        /// <summary>
        /// Disables the associated control.
        /// </summary>
        Disable
    }

    /// <summary>
    /// Authentication behavior to show/hide UI elements based on the some authentication parameters.
    /// </summary>
    /// <remarks>
    /// In Silverlight, the <c>IsEnabled</c> property is declared on <see cref="Control"/> instead of <see cref="FrameworkElement"/>. If the
    /// <see cref="Behavior{T}.AssociatedObject"/> is not a <see cref="Control"/>, but the <see cref="Action"/> is set to <see cref="AuthenticationAction.Disable"/>,
    /// a <see cref="InvalidOperationException"/> will be thrown.
    /// </remarks>
    public class Authentication : BehaviorBase<FrameworkElement>
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The authentication provider.
        /// </summary>
        private static IAuthenticationProvider _authenticationProvider;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Authentication"/> class.
        /// </summary>
        public Authentication()
        {
            if (IsInDesignMode)
            {
                return;
            }

            if (_authenticationProvider == null)
            {
                var dependencyResolver = this.GetDependencyResolver();
                _authenticationProvider = dependencyResolver.Resolve<IAuthenticationProvider>();
            }

            if (_authenticationProvider == null)
            {
                const string error = "No IAuthenticationProvider is registered, cannot use the Authentication behavior without an IAuthenticationProvider";
                Log.Error(error);
                throw new NotSupportedException(error);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the action to execute when the user has no access to the specified UI element.
        /// </summary>
        /// <value>The action.</value>
        public AuthenticationAction Action
        {
            get { return (AuthenticationAction)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Action.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register("Action", typeof(AuthenticationAction),
            typeof(Authentication), new PropertyMetadata(AuthenticationAction.Disable));

        /// <summary>
        /// Gets or sets the authentication tag which can be used to provide additional information to the <see cref="IAuthenticationProvider"/>.
        /// </summary>
        /// <value>The authentication tag.</value>
        public object AuthenticationTag
        {
            get { return GetValue(AuthenticationTagProperty); }
            set { SetValue(AuthenticationTagProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AuthenticationTag.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty AuthenticationTagProperty =
            DependencyProperty.Register("AuthenticationTag", typeof(object), typeof(Authentication), new PropertyMetadata(null));
        #endregion

        #region Methods
        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> has been loaded.
        /// </summary>
        /// <exception cref="InvalidOperationException">No instance of <see cref="IAuthenticationProvider"/> is registered in the <see cref="IServiceLocator"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="Action"/> is set to <see cref="AuthenticationAction.Disable"/> and the <see cref="Behavior{T}.AssociatedObject"/> is not a <see cref="Control"/>.</exception>
        protected override void OnAssociatedObjectLoaded()
        {
            if (!_authenticationProvider.HasAccessToUIElement(AssociatedObject, AssociatedObject.Tag, AuthenticationTag))
            {
                Log.Debug("User has no access to UI element with tag '{0}' and authentication tag '{1}'",
                    ObjectToStringHelper.ToString(AssociatedObject.Tag), ObjectToStringHelper.ToString(AuthenticationTag));

                switch (Action)
                {
#if NET
                    case AuthenticationAction.Hide:
                        AssociatedObject.Visibility = Visibility.Hidden;
                        break;
#endif

                    case AuthenticationAction.Collapse:
                        AssociatedObject.Visibility = Visibility.Collapsed;
                        break;

                    case AuthenticationAction.Disable:
#if SILVERLIGHT || NETFX_CORE
                        if (!(AssociatedObject is Control))
                        {
                            throw new InvalidOperationException("The AssociatedObject is not a Control instance, only AuthenticationAction.Collapse is allowed in SL, Windows Phone and WinRT");
                        }

                        ((Control)AssociatedObject).IsEnabled = false;
#else
                        AssociatedObject.IsEnabled = false;
#endif
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        #endregion
    }
}

#endif