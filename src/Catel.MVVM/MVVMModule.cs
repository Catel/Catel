// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsControlsModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using MVVM;
    using MVVM.Views;
    using Services;
    using IoC;
    using Catel.MVVM.Auditing;

    /// <summary>
    /// MVVM module which allows the registration of default services in the service locator.
    /// </summary>
    public class MVVMModule : IServiceLocatorInitializer
    {
        /// <summary>
        /// Initializes the specified service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public void Initialize(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            serviceLocator.RegisterTypeIfNotYetRegistered<IDataContextSubscriptionService, DataContextSubscriptionService>();
            serviceLocator.RegisterTypeIfNotYetRegistered<ICommandManager, CommandManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewLoadManager, ViewLoadManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelWrapperService, ViewModelWrapperService>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewManager, ViewManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelManager, ViewModelManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IAutoCompletionService, AutoCompletionService>();

#if NET
            serviceLocator.RegisterTypeIfNotYetRegistered<IWrapControlService, WrapControlService>();
#endif

            ViewModelServiceHelper.RegisterDefaultViewModelServices(serviceLocator);

#if !XAMARIN
            var typeFactory = serviceLocator.ResolveType<ITypeFactory>();
            var auditor = typeFactory.CreateInstance<InvalidateCommandManagerOnViewModelInitializationAuditor>();
            AuditingManager.RegisterAuditor(auditor);
#endif

            DesignTimeHelper.InitializeDesignTime();
        }
    }
}
