// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageBinding.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Windows.Markup
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Threading;
    using Catel.IoC;
    using Catel.Services;

#if !NETFX_CORE
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Binding that uses the <see cref="ILanguageService" /> to retrieve the binding values.
    /// </summary>
    public class LanguageBinding : UpdatableMarkupExtension
    {
        private readonly ILanguageService _languageService;
        private Catel.IWeakEventListener _onLanguageUpdatedWeakListener;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageBinding"/> class.
        /// </summary>
        public LanguageBinding()
        {
            var dependencyResolver = this.GetDependencyResolver();
            _languageService = dependencyResolver.Resolve<ILanguageService>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageBinding" /> class.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        public LanguageBinding(string resourceName)
            : this()
        {
            ResourceName = resourceName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the resource name.
        /// </summary>
        /// <value>The resource name.</value>
#if NET || NETCORE
        [ConstructorArgument("resourceName")]
#endif
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
        public CultureInfo Culture { get; set; }
        #endregion

        /// <summary>
        /// The language updated event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Must be public because this uses weak events.
        /// </remarks>
        public void OnLanguageUpdated(object sender, EventArgs e)
        {
            UpdateValue();
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        protected override object ProvideDynamicValue(IServiceProvider serviceProvider)
        {
            if (_languageService == null)
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
            if (culture != null)
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
            var listener = _onLanguageUpdatedWeakListener;
            if (listener != null)
            {
                if (!ReferenceEquals(listener.Source, TargetObject))
                {
                    listener.Detach();
                    _onLanguageUpdatedWeakListener = null;
                }

                if (!listener.IsSourceAlive)
                {
                    listener.Detach();
                    _onLanguageUpdatedWeakListener = null;
                }
            }

            if (_onLanguageUpdatedWeakListener == null)
            {
                _onLanguageUpdatedWeakListener = this.SubscribeToWeakGenericEvent<EventArgs>(_languageService, "LanguageUpdated", OnLanguageUpdated);
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

#endif
