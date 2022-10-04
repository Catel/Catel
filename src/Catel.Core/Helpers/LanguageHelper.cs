namespace Catel
{
    using System.Globalization;
    using IoC;
    using Services;

    /// <summary>
    /// Static wrapper around the service locator to easily retrieve language values.
    /// </summary>
    public static class LanguageHelper
    {
        private static ILanguageService LanguageService;

        /// <summary>
        /// Initializes static members of the <see cref="LanguageHelper"/> class.
        /// </summary>
        static LanguageHelper()
        {
            var serviceLocator = ServiceLocator.Default;

            LanguageService = serviceLocator.ResolveType<ILanguageService>();

            serviceLocator.TypeRegistered += (sender, e) =>
            {
                if (e.ServiceType == typeof (ILanguageService))
                {
                    LanguageService = serviceLocator.ResolveType<ILanguageService>();
                }
            };
        }

        /// <summary>
        /// Gets the string value using the specified culture.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.String.</returns>
        public static string GetString(string resourceName, CultureInfo? culture = null)
        {
            var value = string.Empty;

            if (culture is not null)
            {
                value = LanguageService.GetString(resourceName, culture);
            }
            else
            {
                value = LanguageService.GetString(resourceName);
            }

            return value;
        }
    }
}
