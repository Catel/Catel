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
            serviceLocator.RegisterTypeIfNotYetRegistered<IWrapControlService, WrapControlService>();

            ViewModelServiceHelper.RegisterDefaultViewModelServices(serviceLocator);

            var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

            var invalidateCommandManagerOnViewModelInitializationAuditor = typeFactory.CreateInstance<InvalidateCommandManagerOnViewModelInitializationAuditor>();
            AuditingManager.RegisterAuditor(invalidateCommandManagerOnViewModelInitializationAuditor);

            var subscribeKeyboardEventsOnViewModelCreationAuditor = typeFactory.CreateInstance<SubscribeKeyboardEventsOnViewModelCreationAuditor>();
            AuditingManager.RegisterAuditor(subscribeKeyboardEventsOnViewModelCreationAuditor);

            DesignTimeHelper.InitializeDesignTime();
        }
    }
}
