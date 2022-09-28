// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayNameAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.ComponentModel
{
    using System;
    using IoC;
    using Services;

    /// <summary>
    /// A custom implementation of the display name attribute that uses the <see cref="ILanguageService"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field)]
    public class DisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
    {
        private static readonly Lazy<ILanguageService> DefaultLanguageService = new Lazy<ILanguageService>(() => IoCConfiguration.DefaultDependencyResolver.Resolve<ILanguageService>());

        private readonly string _resourceName;
        private ILanguageService _languageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayNameAttribute"/> class.
        /// </summary>
        public DisplayNameAttribute(string resourceName)
            : base(string.Empty)
        {
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);

            _resourceName = resourceName;
        }

        /// <summary>
        /// Gets or sets the language service. By default or when set to <c>null</c>, this property will resolve the language
        /// service from the default <see cref="IDependencyResolver"/>.
        /// </summary>
        /// <value>The language service.</value>
        public ILanguageService LanguageService
        {
            get
            {
                if (_languageService is not null)
                {
                    return _languageService;
                }

                return DefaultLanguageService.Value;
            }
            set
            {
                _languageService = value;
            }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public override string DisplayName
        {
            get
            {
                var languageService = LanguageService;

                var displayName = languageService.GetString(_resourceName);
                if (string.IsNullOrWhiteSpace(displayName))
                {
                    displayName = _resourceName;
                }

                return displayName;
            }
        }

        /// <summary>
        /// Gets the resource name.
        /// </summary>
        /// <value>The resource name.</value>
        public string ResourceName
        {
            get
            {
                return _resourceName;
            }
        }
    }
}
