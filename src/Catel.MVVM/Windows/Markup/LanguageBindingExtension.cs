﻿namespace Catel.Windows.Markup
{
    using System;
    using System.Globalization;
    using System.Windows;
    using Catel.IoC;
    using Catel.Services;
    using System.Windows.Markup;

    /// <summary>
    /// Binding that uses the <see cref="ILanguageService" /> to retrieve the binding values.
    /// </summary>
    public class LanguageBindingExtension : UpdatableMarkupExtension
    {
        private readonly ILanguageService _languageService;
        private Catel.IWeakEventListener? _onLanguageServiceLanguageUpdatedWeakListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageBindingExtension"/> class.
        /// </summary>
        public LanguageBindingExtension()
            : this(string.Empty)
        {
            // Keep empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageBindingExtension" /> class.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        public LanguageBindingExtension(string resourceName)
        {
            ResourceName = resourceName;

            var dependencyResolver = this.GetDependencyResolver();
            _languageService = dependencyResolver.ResolveRequired<ILanguageService>();
        }

        /// <summary>
        /// Gets or sets the resource name.
        /// </summary>
        /// <value>The resource name.</value>
        [ConstructorArgument("resourceName")]
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide design time messages or not.
        /// </summary>
        /// <value><c>true</c> if design time messages should be hidden; otherwise, <c>false</c>.</value>
        public bool HideDesignTimeMessages { get; set; }

        /// <summary>
        /// Gets or sets the culture. If set to <c>null</c>, it will be determined automatically.
        /// </summary>
        /// <value>The culture.</value>
        public CultureInfo? Culture { get; set; }

        /// <summary>
        /// The language updated event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Must be public because this uses weak events.
        /// </remarks>
        public void OnLanguageUpdated(object? sender, EventArgs e)
        {
            UpdateValue();
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        protected override object? ProvideDynamicValue(IServiceProvider? serviceProvider)
        {
            if (_languageService is null)
            {
                if (ShowDesignTimeMessages())
                {
                    return "[Language service not available]";
                }

                return null;
            }

            if (string.IsNullOrWhiteSpace(ResourceName))
            {
                if (ShowDesignTimeMessages())
                {
                    return "[ResourceName is null or white space]";
                }

                return null;
            }

            var resource = string.Empty;

            var culture = Culture;
            if (culture is not null)
            {
                resource = _languageService.GetString(ResourceName, culture);
            }
            else
            {
                resource = _languageService.GetString(ResourceName);
            }

            if (string.IsNullOrWhiteSpace(resource))
            {
                if (ShowDesignTimeMessages())
                {
                    return "[empty]";
                }
            }

            return resource;
        }

        /// <summary>
        /// Called when the target object is loaded.
        /// <para />
        /// Note that this method will only be called if the target object is a <see cref="FrameworkElement"/>.
        /// </summary>
        protected override void OnTargetObjectLoaded()
        {
            // CTL-925 Use weak events so unloaded elements (like a ComboBoxItem) can also update. The usage of 
            // weak events should prevent memory leaks
            var listener = _onLanguageServiceLanguageUpdatedWeakListener;
            if (listener is not null)
            {
                if (!ReferenceEquals(listener.Source, TargetObject))
                {
                    listener.Detach();
                    _onLanguageServiceLanguageUpdatedWeakListener = null;
                }

                if (!listener.IsSourceAlive)
                {
                    listener.Detach();
                    _onLanguageServiceLanguageUpdatedWeakListener = null;
                }
            }

            if (_onLanguageServiceLanguageUpdatedWeakListener is null)
            {
                _onLanguageServiceLanguageUpdatedWeakListener = this.SubscribeToWeakGenericEvent<EventArgs>(_languageService, "LanguageUpdated", OnLanguageUpdated);
            }

            //_languageService.LanguageUpdated += OnLanguageUpdated;
        }

        /// <summary>
        /// Called when the target object is unloaded.
        /// <para />
        /// Note that this method will only be called if the target object is a <see cref="FrameworkElement"/>.
        /// </summary>
        protected override void OnTargetObjectUnloaded()
        {
            //_languageService.LanguageUpdated -= OnLanguageUpdated;
        }

        private bool ShowDesignTimeMessages()
        {
            return !HideDesignTimeMessages && CatelEnvironment.IsInDesignMode;
        }
    }
}
