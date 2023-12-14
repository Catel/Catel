namespace Catel.Windows.Interactivity
{
    using System.Windows;
    using System.Windows.Controls;
    using System;
    using IoC;
    using Logging;
    using MVVM;

    /// <summary>
    /// The available actions to perform when a user is not able to view a specific UI element.
    /// </summary>
    public enum AuthenticationAction
    {
        /// <summary>
        /// Hides the associated control.
        /// </summary>
        Hide,

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
    public class Authentication : BehaviorBase<FrameworkElement>
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The authentication provider.
        /// </summary>
        private static readonly IAuthenticationProvider _authenticationProvider;

        static Authentication()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            _authenticationProvider = dependencyResolver.ResolveRequired<IAuthenticationProvider>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Authentication"/> class.
        /// </summary>
        public Authentication()
        {
            if (IsInDesignMode)
            {
                return;
            }
        }

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
        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register(nameof(Action), typeof(AuthenticationAction),
            typeof(Authentication), new PropertyMetadata(AuthenticationAction.Disable));

        /// <summary>
        /// Gets or sets the authentication tag which can be used to provide additional information to the <see cref="IAuthenticationProvider"/>.
        /// </summary>
        /// <value>The authentication tag.</value>
        public object? AuthenticationTag
        {
            get { return GetValue(AuthenticationTagProperty); }
            set { SetValue(AuthenticationTagProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AuthenticationTag.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty AuthenticationTagProperty =
            DependencyProperty.Register(nameof(AuthenticationTag), typeof(object), typeof(Authentication), new PropertyMetadata(null));

        /// <summary>
        /// Called when the associated object has been loaded.
        /// </summary>
        /// <exception cref="InvalidOperationException">No instance of <see cref="IAuthenticationProvider"/> is registered in the <see cref="IServiceLocator"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="Action"/> is set to <see cref="AuthenticationAction.Disable"/> and the associated object is not a <see cref="Control"/>.</exception>
        protected override void OnAssociatedObjectLoaded()
        {
            if (!_authenticationProvider.HasAccessToUIElement(AssociatedObject, AssociatedObject.Tag, AuthenticationTag))
            {
                Log.Debug("User has no access to UI element with tag '{0}' and authentication tag '{1}'",
                    ObjectToStringHelper.ToString(AssociatedObject.Tag), ObjectToStringHelper.ToString(AuthenticationTag));

                switch (Action)
                {
                    case AuthenticationAction.Hide:
                        AssociatedObject.Visibility = Visibility.Hidden;
                        break;

                    case AuthenticationAction.Collapse:
                        AssociatedObject.Visibility = Visibility.Collapsed;
                        break;

                    case AuthenticationAction.Disable:
                        AssociatedObject.IsEnabled = false;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
