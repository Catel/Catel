namespace Catel
{
    using System;
    using Catel.IoC;
    using Catel.Services;
    using Microsoft.Extensions.DependencyInjection;

    public static class LanguageHelper
    {
        private static readonly Lazy<ILanguageService> LanguageServiceLazy = new(() =>
        {
            return IoCContainer.ServiceProvider.GetRequiredService<ILanguageService>();
        });

        public static string? GetString(string resourceName)
        {
            return LanguageServiceLazy.Value.GetString(resourceName);
        }

        public static string GetRequiredString(string resourceName)
        {
            return LanguageServiceLazy.Value.GetRequiredString(resourceName);
        }
    }
}
