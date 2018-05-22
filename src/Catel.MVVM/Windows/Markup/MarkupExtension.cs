// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MarkupExtension.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE

namespace Catel.Windows.Markup
{
    using System;
    using System.ComponentModel;
    using Threading;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows;
    using System.Windows.Data;
#endif

    /// <summary>
    /// Custom markup extension.
    /// </summary>
    public abstract class MarkupExtension : Binding, INotifyPropertyChanged
#if !NETFX_CORE
        , ISupportInitialize
#endif
    {
        private object _internalBindingValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkupExtension"/> class.
        /// </summary>
        protected MarkupExtension()
        {
            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            Source = this;
            Mode = BindingMode.OneWay;
            Path = new PropertyPath("InternalBindingValue");

#if NETFX_CORE
            Dispatcher.BeginInvoke(UpdateBinding);
#endif
        }

        /// <summary>
        /// Gets or sets the binding value.
        /// <para />
        /// Do not use this property, it's use to set up the binding manually.
        /// </summary>
        /// <value>The binding value.</value>
        public object InternalBindingValue
        {
            get
            {
                return _internalBindingValue;
            }
            set
            {
                _internalBindingValue = value;
                RaisePropertyChanged("InternalBindingValue");
            }
        }

#if !NETFX_CORE
        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        void ISupportInitialize.EndInit()
        {
            UpdateBinding();
        }
#endif

        /// <summary>
        /// Updates the binding.
        /// </summary>
        protected void UpdateBinding()
        {
            InternalBindingValue = ProvideValue(null);
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public abstract object ProvideValue(IServiceProvider serviceProvider);

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a property has been changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

#endif