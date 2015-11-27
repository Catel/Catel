// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ComponentModel.DataAnnotations
{
    using System;
    using IoC;
    using Services;

    /// <summary>
    /// A custom implementation of the display attribute that uses the <see cref="ILanguageService"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DisplayAttribute : System.ComponentModel.DataAnnotations.DisplayAttribute
    {
        private static readonly Lazy<ILanguageService> DefaultLanguageService = new Lazy<ILanguageService>(() => IoCConfiguration.DefaultDependencyResolver.Resolve<ILanguageService>());

        private readonly string _resourceName;
        private ILanguageService _languageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayNameAttribute"/> class.
        /// </summary>
        public DisplayAttribute(string resourceName)
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

        // TODO: override all the methods

        ///// <summary>
        ///// Gets the display name.
        ///// </summary>
        ///// <value>The display name.</value>
        //public override string DisplayName
        //{
        //    get
        //    {
        //        var languageService = LanguageService;
        //        return languageService.GetString(_resourceName);
        //    }
        //}
    }
}