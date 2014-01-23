// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageBinding.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Windows.Markup
{
    using System;
    using Catel.IoC;
    using Catel.Services;

#if !NETFX_CORE
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Binding that uses the <see cref="ILanguageService" /> to retrieve the binding values.
    /// </summary>
    public class LanguageBinding : MarkupExtension
    {
        private readonly ILanguageService _languageService;

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
#if NET
        [ConstructorArgument("resourceName")]
#endif
        public string ResourceName { get; set; }
        #endregion

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_languageService == null)
            {
                return null;
            }


            if (string.IsNullOrWhiteSpace(ResourceName))
            {
                return null;
            }

            var resource = _languageService.GetString(ResourceName);
            return resource;
        }
    }
}