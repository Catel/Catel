namespace Catel
{
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    public static class IServiceCollectionExtensions
    {
        public static bool IsRegistered<TService>(this IServiceCollection serviceCollection)
        {
            return serviceCollection.Any(x => x.ServiceType == typeof(TService));
        }
    }
}
