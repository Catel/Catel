namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class INavigationServiceExtensions
    {
        /// <summary>
        /// Navigates the specified location registered using the view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter, 
        /// and the value is the value of the parameter.</param>
        public static Task NavigateAsync<TViewModel>(INavigationService navigationService, Dictionary<string, object>? parameters = null)
        {
            ArgumentNullException.ThrowIfNull(navigationService);

            return navigationService.NavigateAsync(typeof(TViewModel), parameters);
        }
    }
}
