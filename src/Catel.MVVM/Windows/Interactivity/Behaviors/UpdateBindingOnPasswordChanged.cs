namespace Catel.Windows.Interactivity
{
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Xaml.Behaviors;
    using UIEventArgs = System.EventArgs;

    /// <summary>
    /// This behavior automatically updates the binding of a <see cref="PasswordBox"/> when the
    /// <c>PasswordChanged</c> event occurs.
    /// </summary>
    public class UpdateBindingOnPasswordChanged : BehaviorBase<PasswordBox>
    {
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password
        {
            get { return (string) GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        /// <summary>
        /// The Password Property
        /// </summary>
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(nameof(Password), typeof (string), typeof(UpdateBindingOnPasswordChanged),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) => ((UpdateBindingOnPasswordChanged)sender).OnPasswordChanged(e)));

        /// <summary>
        /// Called when the password has been changed.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPasswordChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject is not null)
            {
                if (AssociatedObject.Password != Password)
                {
                    AssociatedObject.Password = Password;
                }
            }
        }

        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            AssociatedObject.PasswordChanged += OnPasswordChanged;
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            AssociatedObject.PasswordChanged -= OnPasswordChanged;
        }

        /// <summary>
        /// Passwords the box password changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UIEventArgs"/> instance containing the event data.</param>
        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!IsEnabled)
            {
                return;
            }

            Password = AssociatedObject.Password;
        }
    }
}
