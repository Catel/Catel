namespace Catel.ComponentModel
{
    using System;
    using Catel.Logging;
    using Services;

    /// <summary>
    /// A custom implementation of the display name attribute that uses the <see cref="ILanguageService"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field)]
    public class DisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
    {
        private readonly string _resourceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayNameAttribute"/> class.
        /// </summary>
        public DisplayNameAttribute(ILanguageService languageService, string resourceName)
            : base(string.Empty)
        {
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);

            LanguageService = languageService;
            _resourceName = resourceName;
        }

        /// <summary>
        /// Gets or sets the language service.
        /// </summary>
        /// <value>The language service.</value>
        public ILanguageService LanguageService { get; set; }

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
