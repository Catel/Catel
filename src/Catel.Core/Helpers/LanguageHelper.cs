namespace Catel
{
    using System;
    using Catel.IoC;
    using Catel.Services;

    public static class LanguageHelper
    {
        private static readonly Lazy<ILanguageService> LanguageService = new Lazy<ILanguageService>(() =>
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            return dependencyResolver.ResolveRequired<ILanguageService>();
        });

        public static string? GetString(string resourceName) 
        {
            var languageService = LanguageService.Value;
            return languageService.GetString(resourceName);
        }

        public static string GetRequiredString(string resourceName)
        {
            var languageService = LanguageService.Value;
            return languageService.GetRequiredString(resourceName);
        }
    }
}
