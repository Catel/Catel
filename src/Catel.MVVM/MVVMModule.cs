namespace Catel
{
    using MVVM;
    using MVVM.Views;
    using Services;
    using IoC;
    using Catel.MVVM.Auditing;
    using System;

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
            ArgumentNullException.ThrowIfNull(serviceLocator);

            serviceLocator.RegisterTypeIfNotYetRegistered<IDataContextSubscriptionService, DataContextSubscriptionService>();
            serviceLocator.RegisterTypeIfNotYetRegistered<ICommandManager, CommandManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewLoadManager, ViewLoadManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelWrapperService, ViewModelWrapperService>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewManager, ViewManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelManager, ViewModelManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IAutoCompletionService, AutoCompletionService>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IWrapControlService, WrapControlService>();

            ViewModelServiceHelper.RegisterDefaultViewModelServices(serviceLocator);

#pragma warning disable IDISP001
            var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();
#pragma warning restore IDISP001

            var invalidateCommandManagerOnViewModelInitializationAuditor = typeFactory.CreateRequiredInstance<InvalidateCommandManagerOnViewModelInitializationAuditor>();
            AuditingManager.RegisterAuditor(invalidateCommandManagerOnViewModelInitializationAuditor);

            var subscribeKeyboardEventsOnViewModelCreationAuditor = typeFactory.CreateRequiredInstance<SubscribeKeyboardEventsOnViewModelCreationAuditor>();
            AuditingManager.RegisterAuditor(subscribeKeyboardEventsOnViewModelCreationAuditor);

            DesignTimeHelper.InitializeDesignTime();
        }
    }
}
