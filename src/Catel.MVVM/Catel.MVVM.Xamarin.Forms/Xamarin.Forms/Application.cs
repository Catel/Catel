// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Application.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Xamarin.Forms
{
    using global::Xamarin.Forms;
    using IoC;
    using MVVM;

    /// <summary>
    /// The application base class.
    /// </summary>
    /// <typeparam name="TMainPage">
    /// The main page type.
    /// </typeparam>
    public class Application<TMainPage> : Application 
        where TMainPage : Page
    {
        /// <summary>
        /// The main application.
        /// </summary>
        protected Application()
        {
            var serviceLocator = ServiceLocator.Default;
            var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

            // TODO: Check if ModuleInit is enabled in runtime and execute this.
            /*
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var currentModuleType = assembly.ExportedTypes.FirstOrDefault(type => typeof(IServiceLocatorInitializer).IsAssignableFromEx(type));
            if (currentModuleType != null)
            {
                var currentModule = (IServiceLocatorInitializer)typeFactory.CreateInstance(currentModuleType);
                currentModule.Initialize(serviceLocator);
            }
            */

            // TODO: Improve this approach.
            var viewModelLocator = serviceLocator.ResolveType<IViewModelLocator>();
            var viewModelFactory = serviceLocator.ResolveType<IViewModelFactory>();
            var viewModelType = viewModelLocator.ResolveViewModel(typeof (TMainPage));
            var mainPage = typeFactory.CreateInstance<TMainPage>();
            
            mainPage.BindingContext = viewModelFactory.CreateViewModel(viewModelType, null);
            Initialize(mainPage);
        }

        /// <summary>
        /// Initialize the main page. 
        /// </summary>
        /// <param name="mainPage"></param>
        private void Initialize(TMainPage mainPage)
        {
            MainPage = CustomizeMainPage(mainPage);
        }

        /// <summary>
        /// Allow developers customize the application page.
        /// </summary>
        /// <param name="currentMainPage"></param>
        /// <returns>The application page.</returns>
        /// <remarks>If customization is <c>null</c> the <paramref name="currentMainPage"/> will be set as MainPage.</remarks>
        private Page CustomizeMainPage(TMainPage currentMainPage)
        {
            return OnCustomizeMainPage(currentMainPage) ?? currentMainPage;
        }

        /// <summary>
        /// Allow developers customize the application page.
        /// </summary>
        /// <param name="currentMainPage">The current main page.</param>
        /// <returns>The customized main page.</returns>
        /// <example>
        /// return new Navigation(currentMainPage);
        /// </example>
        protected virtual Page OnCustomizeMainPage(TMainPage currentMainPage)
        {
            return null;
        }
    }
}