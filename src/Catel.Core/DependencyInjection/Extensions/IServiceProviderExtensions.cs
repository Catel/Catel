namespace Catel
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public static class IServiceProviderExtensions
    {
        public static bool IsRegistered<TService>(this IServiceProvider serviceProvider)
        {
            return IsRegistered(serviceProvider, typeof(TService));
        }

        public static bool IsRegistered(this IServiceProvider serviceProvider, Type serviceType)
        {
            var serviceChecker = serviceProvider.GetRequiredService<IServiceProviderIsService>();

            return serviceChecker.IsService(serviceType);
        }
    }
}
