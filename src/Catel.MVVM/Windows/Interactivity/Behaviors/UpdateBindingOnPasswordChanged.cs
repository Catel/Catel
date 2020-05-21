// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateBindingOnPasswordChanged.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Interactivity
{
#if UWP
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Xaml.Behaviors;
    using UIEventArgs = System.EventArgs;
#endif

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
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof (string), typeof(UpdateBindingOnPasswordChanged),
#if NET || NETCORE
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
#else
            new PropertyMetadata(null,
#endif
                (sender, e) => ((UpdateBindingOnPasswordChanged)sender).OnPasswordChanged(e)));

        /// <summary>
        /// Called when the password has been changed.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPasswordChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject != null)
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

#if NET || NETCORE || UWP
            Password = AssociatedObject.Password;
#else
            var binding = AssociatedObject.GetBindingExpression(PasswordBox.PasswordProperty);
            if (binding != null)
            {
                binding.UpdateSource();
            }
#endif
        }
    }
}

#endif
