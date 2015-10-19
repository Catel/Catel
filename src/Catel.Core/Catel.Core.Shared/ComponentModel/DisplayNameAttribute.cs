// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayNameAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.ComponentModel
{
    using System;
    using IoC;
    using Services;

    /// <summary>
    /// A custom implementation of the display name attribute that uses the <see cref="ILanguageService"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
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
                if (_languageService != null)
                {
                    return _languageService;
                }

                return DefaultLanguageService.Value;
            }
            set { _languageService = value; }
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
                return languageService.GetString(_resourceName);
            }
        }
    }
}

#endif