[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Catel.Tests")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v5.0", FrameworkDisplayName="")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.catelproject.com", "Catel.MVVM")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.catelproject.com", "Catel.MVVM.Converters")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.catelproject.com", "Catel.MVVM.Providers")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.catelproject.com", "Catel.MVVM.Views")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.catelproject.com", "Catel.Windows")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.catelproject.com", "Catel.Windows.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.catelproject.com", "Catel.Windows.Data")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.catelproject.com", "Catel.Windows.Interactivity")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.catelproject.com", "Catel.Windows.Markup")]
[assembly: System.Windows.Markup.XmlnsPrefix("http://schemas.catelproject.com", "catel")]
[assembly: System.Windows.ThemeInfo(System.Windows.ResourceDictionaryLocation.None, System.Windows.ResourceDictionaryLocation.SourceAssembly)]
namespace Catel
{
    public static class CatelEnvironment
    {
        public const string DefaultMultiLingualDependencyPropertyValue = "SET IN CONSTRUCTOR TO SUPPORT RUNTIME LANGUAGE SWITCHING";
        public static bool IsInDesignMode { get; }
        public static System.Windows.Window MainWindow { get; }
        public static bool BypassDevEnvCheck { get; set; }
        public static bool DisablePropertyChangeNotifications { get; set; }
        public static bool GetIsInDesignMode(bool initializeDesignTime) { }
        public static void RegisterDefaultViewModelServices() { }
    }
    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.All, AllowMultiple=true)]
    public class DesignTimeCodeAttribute : System.Attribute
    {
        public DesignTimeCodeAttribute(System.Type typeToConstruct) { }
    }
    public static class DesignTimeHelper
    {
        public static void InitializeDesignTime() { }
    }
    public class DesignTimeInitializer
    {
        public DesignTimeInitializer() { }
        public bool CanInitialize { get; }
        protected virtual void Initialize() { }
    }
    public static class ICommandManagerExtensions
    {
        public static void CreateCommandWithGesture(this Catel.MVVM.ICommandManager commandManager, System.Type containerType, string commandNameFieldName) { }
        public static System.Collections.Generic.Dictionary<string, System.Windows.Input.ICommand> FindCommandsByGesture(this Catel.MVVM.ICommandManager commandManager, Catel.Windows.Input.InputGesture inputGesture) { }
    }
    public static class INotifyPropertyChangedExtensions
    {
        public static void SubscribeToPropertyChanged(this System.ComponentModel.INotifyPropertyChanged notifyPropertyChanged, string propertyName, System.EventHandler<System.ComponentModel.PropertyChangedEventArgs> handler) { }
    }
    public class MVVMModule : Catel.IoC.IServiceLocatorInitializer
    {
        public MVVMModule() { }
        public void Initialize(Catel.IoC.IServiceLocator serviceLocator) { }
    }
    public static class StringExtensions
    {
        public static string GetUniqueControlName(this string controlName) { }
    }
    public static class ThemeHelper
    {
        public static void EnsureCatelMvvmThemeIsLoaded() { }
        public static void EnsureThemeIsLoaded(System.Uri resourceUri) { }
        public static void EnsureThemeIsLoaded(System.Uri resourceUri, System.Func<bool> predicate) { }
    }
}
namespace Catel.Data
{
    public class DispatcherObservableObject : Catel.Data.ObservableObject
    {
        public DispatcherObservableObject() { }
        protected override void RaisePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) { }
    }
}
namespace Catel.MVVM.Auditing
{
    public static class AuditingHelper
    {
        public static void RegisterViewModel(Catel.MVVM.IViewModel viewModel) { }
    }
    public class AuditingManager
    {
        public AuditingManager() { }
        public static bool IsAuditingEnabled { get; }
        public static int RegisteredAuditorsCount { get; }
        public static void Clear() { }
        public static void RegisterAuditor(Catel.MVVM.Auditing.IAuditor auditor) { }
        public static void RegisterAuditor<TAuditor>()
            where TAuditor :  class, Catel.MVVM.Auditing.IAuditor { }
        public static void UnregisterAuditor(Catel.MVVM.Auditing.IAuditor auditor) { }
    }
    public abstract class AuditorBase : Catel.MVVM.Auditing.IAuditor
    {
        protected AuditorBase() { }
        public System.Collections.Generic.HashSet<string> PropertiesToIgnore { get; }
        public virtual void OnCommandExecuted(Catel.MVVM.IViewModel viewModel, string commandName, Catel.MVVM.ICatelCommand command, object commandParameter) { }
        public virtual void OnPropertyChanged(Catel.MVVM.IViewModel viewModel, string propertyName, object newValue) { }
        public virtual void OnViewModelCanceled(Catel.MVVM.IViewModel viewModel) { }
        public virtual void OnViewModelCanceling(Catel.MVVM.IViewModel viewModel) { }
        public virtual void OnViewModelClosed(Catel.MVVM.IViewModel viewModel) { }
        public virtual void OnViewModelClosing(Catel.MVVM.IViewModel viewModel) { }
        public virtual void OnViewModelCreated(Catel.MVVM.IViewModel viewModel) { }
        public virtual void OnViewModelCreating(System.Type viewModelType) { }
        public virtual void OnViewModelInitialized(Catel.MVVM.IViewModel viewModel) { }
        public virtual void OnViewModelSaved(Catel.MVVM.IViewModel viewModel) { }
        public virtual void OnViewModelSaving(Catel.MVVM.IViewModel viewModel) { }
    }
    public interface IAuditor
    {
        System.Collections.Generic.HashSet<string> PropertiesToIgnore { get; }
        void OnCommandExecuted(Catel.MVVM.IViewModel viewModel, string commandName, Catel.MVVM.ICatelCommand command, object commandParameter);
        void OnPropertyChanged(Catel.MVVM.IViewModel viewModel, string propertyName, object newValue);
        void OnViewModelCanceled(Catel.MVVM.IViewModel viewModel);
        void OnViewModelCanceling(Catel.MVVM.IViewModel viewModel);
        void OnViewModelClosed(Catel.MVVM.IViewModel viewModel);
        void OnViewModelClosing(Catel.MVVM.IViewModel viewModel);
        void OnViewModelCreated(Catel.MVVM.IViewModel viewModel);
        void OnViewModelCreating(System.Type viewModelType);
        void OnViewModelInitialized(Catel.MVVM.IViewModel viewModel);
        void OnViewModelSaved(Catel.MVVM.IViewModel viewModel);
        void OnViewModelSaving(Catel.MVVM.IViewModel viewModel);
    }
    public class InvalidateCommandManagerOnViewModelInitializationAuditor : Catel.MVVM.Auditing.AuditorBase
    {
        public InvalidateCommandManagerOnViewModelInitializationAuditor(Catel.MVVM.ICommandManager commandManager, Catel.Services.IDispatcherService dispatcherService) { }
        public override void OnViewModelInitialized(Catel.MVVM.IViewModel viewModel) { }
    }
    public class SubscribeKeyboardEventsOnViewModelCreationAuditor : Catel.MVVM.Auditing.AuditorBase
    {
        public SubscribeKeyboardEventsOnViewModelCreationAuditor(Catel.MVVM.ICommandManager commandManager, Catel.Services.IDispatcherService dispatcherService) { }
        public override void OnViewModelCreated(Catel.MVVM.IViewModel viewModel) { }
    }
}
namespace Catel.MVVM
{
    public class CancelingEventArgs : Catel.MVVM.CancellableEventArgs
    {
        public CancelingEventArgs() { }
    }
    public abstract class CancellableEventArgs : System.EventArgs
    {
        public CancellableEventArgs() { }
        public bool Cancel { get; set; }
    }
    public class Command : Catel.MVVM.Command<object, object>
    {
        public Command(System.Action execute, System.Func<bool> canExecute = null, object tag = null) { }
    }
    public abstract class CommandBase
    {
        [System.CLSCompliant(false)]
        protected static readonly Catel.MVVM.IAuthenticationProvider AuthenticationProvider;
        protected static readonly Catel.Services.IDispatcherService DispatcherService;
        protected CommandBase() { }
    }
    public class CommandCanceledEventArgs : Catel.MVVM.CommandEventArgs
    {
        public CommandCanceledEventArgs(object commandParameter = null) { }
        public bool Cancel { get; set; }
    }
    public abstract class CommandContainerBase : Catel.MVVM.CommandContainerBase<object>
    {
        protected CommandContainerBase(string commandName, Catel.MVVM.ICommandManager commandManager) { }
    }
    public abstract class CommandContainerBase<TParameter> : Catel.MVVM.CommandContainerBase<TParameter, TParameter, Catel.MVVM.ITaskProgressReport>
    {
        protected CommandContainerBase(string commandName, Catel.MVVM.ICommandManager commandManager) { }
    }
    public abstract class CommandContainerBase<TExecuteParameter, TCanExecuteParameter> : Catel.MVVM.CommandContainerBase<TExecuteParameter, TCanExecuteParameter, Catel.MVVM.ITaskProgressReport>
    {
        protected CommandContainerBase(string commandName, Catel.MVVM.ICommandManager commandManager) { }
    }
    public abstract class CommandContainerBase<TExecuteParameter, TCanExecuteParameter, TPogress>
        where TPogress : Catel.MVVM.ITaskProgressReport
    {
        protected CommandContainerBase(string commandName, Catel.MVVM.ICommandManager commandManager) { }
        public string CommandName { get; }
        protected virtual bool CanExecute(TCanExecuteParameter parameter) { }
        protected virtual void Execute(TExecuteParameter parameter) { }
        protected virtual System.Threading.Tasks.Task ExecuteAsync(TExecuteParameter parameter) { }
        protected void InvalidateCommand() { }
    }
    public class CommandCreatedEventArgs : System.EventArgs
    {
        public CommandCreatedEventArgs(System.Windows.Input.ICommand command, string name) { }
        public System.Windows.Input.ICommand Command { get; }
        public string Name { get; }
    }
    public class CommandEventArgs : System.EventArgs
    {
        public CommandEventArgs(object commandParameter = null) { }
        public object CommandParameter { get; set; }
    }
    public class CommandExecutedEventArgs : System.EventArgs
    {
        public CommandExecutedEventArgs(Catel.MVVM.ICatelCommand command, object commandParameter = null, string commandPropertyName = null) { }
        public Catel.MVVM.ICatelCommand Command { get; }
        public object CommandParameter { get; }
        public string CommandPropertyName { get; }
    }
    public static class CommandHelper
    {
        public static Catel.MVVM.Command CreateCommand(System.Action execute, System.Linq.Expressions.Expression<System.Func<Catel.Data.IValidationSummary>> validationSummaryPropertyExpression, object tag = null) { }
        public static Catel.MVVM.Command<TExecuteParameter> CreateCommand<TExecuteParameter>(System.Action<TExecuteParameter> execute, System.Linq.Expressions.Expression<System.Func<Catel.Data.IValidationSummary>> validationSummaryPropertyExpression, object tag = null) { }
        public static Catel.MVVM.TaskCommand CreateTaskCommand(System.Func<System.Threading.Tasks.Task> execute, System.Linq.Expressions.Expression<System.Func<Catel.Data.IValidationSummary>> validationSummaryPropertyExpression, object tag = null) { }
        public static Catel.MVVM.TaskCommand<TExecuteParameter> CreateTaskCommand<TExecuteParameter>(System.Func<TExecuteParameter, System.Threading.Tasks.Task> execute, System.Linq.Expressions.Expression<System.Func<Catel.Data.IValidationSummary>> validationSummaryPropertyExpression, object tag = null) { }
    }
    public class CommandManager : Catel.MVVM.ICommandManager
    {
        public CommandManager() { }
        public bool IsKeyboardEventsSuspended { get; set; }
        public event System.EventHandler<Catel.MVVM.CommandCreatedEventArgs> CommandCreated;
        public void CreateCommand(string commandName, Catel.Windows.Input.InputGesture inputGesture = null, Catel.MVVM.ICompositeCommand compositeCommand = null, bool throwExceptionWhenCommandIsAlreadyCreated = true) { }
        public void ExecuteCommand(string commandName) { }
        public System.Windows.Input.ICommand GetCommand(string commandName) { }
        public System.Collections.Generic.IEnumerable<string> GetCommands() { }
        public Catel.Windows.Input.InputGesture GetInputGesture(string commandName) { }
        public Catel.Windows.Input.InputGesture GetOriginalInputGesture(string commandName) { }
        public void InvalidateCommands() { }
        public bool IsCommandCreated(string commandName) { }
        public void RegisterAction(string commandName, System.Action action) { }
        public void RegisterAction(string commandName, System.Action<object> action) { }
        public void RegisterCommand(string commandName, System.Windows.Input.ICommand command, Catel.MVVM.IViewModel viewModel = null) { }
        public void ResetInputGestures() { }
        public void SubscribeToKeyboardEvents() { }
        public void SubscribeToKeyboardEvents(System.Windows.FrameworkElement view) { }
        public void UnregisterAction(string commandName, System.Action action) { }
        public void UnregisterAction(string commandName, System.Action<object> action) { }
        public void UnregisterCommand(string commandName, System.Windows.Input.ICommand command) { }
        public void UpdateInputGesture(string commandName, Catel.Windows.Input.InputGesture inputGesture = null) { }
    }
    public class CommandManagerWrapper
    {
        public CommandManagerWrapper(System.Windows.FrameworkElement view, Catel.MVVM.ICommandManager commandManager = null) { }
        protected System.Windows.FrameworkElement View { get; }
    }
    public class CommandProgressChangedEventArgs<TProgress> : System.EventArgs
    {
        public CommandProgressChangedEventArgs(TProgress progress) { }
        public TProgress Progress { get; set; }
    }
    public class Command<TExecuteParameter> : Catel.MVVM.Command<TExecuteParameter, TExecuteParameter>
    {
        public Command(System.Action execute, System.Func<bool> canExecute = null, object tag = null) { }
        public Command(System.Action<TExecuteParameter> execute, System.Func<TExecuteParameter, bool> canExecute = null, object tag = null) { }
    }
    public class Command<TExecuteParameter, TCanExecuteParameter> : Catel.MVVM.CommandBase, Catel.MVVM.ICatelCommand, Catel.MVVM.ICatelCommand<TExecuteParameter, TCanExecuteParameter>, System.Windows.Input.ICommand
    {
        public Command(System.Action execute, System.Func<bool> canExecute = null, object tag = null) { }
        public Command(System.Action<TExecuteParameter> execute, System.Func<TCanExecuteParameter, bool> canExecute = null, object tag = null) { }
        public bool AutomaticallyDispatchEvents { get; set; }
        public object Tag { get; }
        public event System.EventHandler CanExecuteChanged;
        public event System.EventHandler<Catel.MVVM.CommandExecutedEventArgs> Executed;
        public bool CanExecute() { }
        public bool CanExecute(object parameter) { }
        public virtual bool CanExecute(TCanExecuteParameter parameter) { }
        public void Execute() { }
        public void Execute(object parameter) { }
        public void Execute(TExecuteParameter parameter) { }
        protected virtual void Execute(TExecuteParameter parameter, bool ignoreCanExecuteCheck) { }
        protected void InitializeActions(System.Action<TExecuteParameter> executeWithParameter, System.Action executeWithoutParameter, System.Func<TCanExecuteParameter, bool> canExecuteWithParameter, System.Func<bool> canExecuteWithoutParameter) { }
        public virtual void RaiseCanExecuteChanged() { }
        protected virtual void RaiseExecuted(object parameter) { }
    }
    public class CompositeCommand : Catel.MVVM.Command, Catel.MVVM.ICatelCommand, Catel.MVVM.ICompositeCommand, System.Windows.Input.ICommand
    {
        public CompositeCommand() { }
        public bool AllowPartialExecution { get; set; }
        public bool AtLeastOneMustBeExecutable { get; set; }
        public bool CheckCanExecuteOfAllCommandsToDetermineCanExecuteForCompositeCommand { get; set; }
        public System.Collections.Generic.IEnumerable<System.Action> GetActions() { }
        public System.Collections.Generic.IEnumerable<System.Action<object>> GetActionsWithParameter() { }
        public System.Collections.Generic.IEnumerable<System.Windows.Input.ICommand> GetCommands() { }
        public void RegisterAction(System.Action action) { }
        public void RegisterAction(System.Action<object> action) { }
        public void RegisterCommand(System.Windows.Input.ICommand command, Catel.MVVM.IViewModel viewModel = null) { }
        public void UnregisterAction(System.Action action) { }
        public void UnregisterAction(System.Action<object> action) { }
        public void UnregisterCommand(System.Windows.Input.ICommand command) { }
    }
    public enum DataContextSubscriptionMode
    {
        DirectDataContext = 0,
        InheritedDataContext = 1,
    }
    public class DataContextSubscriptionService : Catel.MVVM.IDataContextSubscriptionService
    {
        public DataContextSubscriptionService() { }
        public Catel.MVVM.DataContextSubscriptionMode DefaultDataContextSubscriptionMode { get; set; }
        public virtual Catel.MVVM.DataContextSubscriptionMode GetDataContextSubscriptionMode(System.Type viewType) { }
    }
    public class DefaultViewModelToModelMappingConverter : Catel.MVVM.ViewModelToModelConverterBase
    {
        public DefaultViewModelToModelMappingConverter(string[] propertyNames) { }
        public override bool CanConvert(System.Type[] types, System.Type outType, System.Type viewModelType) { }
        public override bool CanConvertBack(System.Type inType, System.Type[] outTypes, System.Type viewModelType) { }
        public override object Convert(object[] values, Catel.MVVM.IViewModel viewModel) { }
        public override object[] ConvertBack(object value, Catel.MVVM.IViewModel viewModel) { }
    }
    [System.CLSCompliant(false)]
    public interface IAuthenticationProvider
    {
        bool CanCommandBeExecuted(Catel.MVVM.ICatelCommand command, object commandParameter);
        [System.CLSCompliant(false)]
        bool HasAccessToUIElement(System.Windows.FrameworkElement element, object tag, object authenticationTag);
    }
    public interface ICatelCommand : System.Windows.Input.ICommand
    {
        object Tag { get; }
        event System.EventHandler<Catel.MVVM.CommandExecutedEventArgs> Executed;
        bool CanExecute();
        void Execute();
        void RaiseCanExecuteChanged();
    }
    public interface ICatelCommand<TExecuteParameter, TCanExecuteParameter> : Catel.MVVM.ICatelCommand, System.Windows.Input.ICommand
    {
        bool CanExecute(TCanExecuteParameter parameter);
        void Execute(TExecuteParameter parameter);
    }
    public interface ICatelTaskCommand<TProgress> : Catel.MVVM.ICatelCommand, System.Windows.Input.ICommand
        where TProgress : Catel.MVVM.ITaskProgressReport
    {
        Catel.MVVM.Command CancelCommand { get; }
        bool IsCancellationRequested { get; }
        bool IsExecuting { get; }
        event System.EventHandler<Catel.MVVM.CommandEventArgs> Canceled;
        event System.EventHandler<Catel.MVVM.CommandCanceledEventArgs> Executing;
        event System.EventHandler<Catel.MVVM.CommandProgressChangedEventArgs<TProgress>> ProgressChanged;
        void Cancel();
    }
    public interface ICommandManager
    {
        bool IsKeyboardEventsSuspended { get; set; }
        event System.EventHandler<Catel.MVVM.CommandCreatedEventArgs> CommandCreated;
        void CreateCommand(string commandName, Catel.Windows.Input.InputGesture inputGesture = null, Catel.MVVM.ICompositeCommand compositeCommand = null, bool throwExceptionWhenCommandIsAlreadyCreated = true);
        void ExecuteCommand(string commandName);
        System.Windows.Input.ICommand GetCommand(string commandName);
        System.Collections.Generic.IEnumerable<string> GetCommands();
        Catel.Windows.Input.InputGesture GetInputGesture(string commandName);
        Catel.Windows.Input.InputGesture GetOriginalInputGesture(string commandName);
        void InvalidateCommands();
        bool IsCommandCreated(string commandName);
        void RegisterAction(string commandName, System.Action action);
        void RegisterAction(string commandName, System.Action<object> action);
        void RegisterCommand(string commandName, System.Windows.Input.ICommand command, Catel.MVVM.IViewModel viewModel = null);
        void ResetInputGestures();
        void SubscribeToKeyboardEvents();
        void SubscribeToKeyboardEvents(System.Windows.FrameworkElement view);
        void UnregisterAction(string commandName, System.Action action);
        void UnregisterAction(string commandName, System.Action<object> action);
        void UnregisterCommand(string commandName, System.Windows.Input.ICommand command);
        void UpdateInputGesture(string commandName, Catel.Windows.Input.InputGesture inputGesture = null);
    }
    public interface ICompositeCommand : Catel.MVVM.ICatelCommand, System.Windows.Input.ICommand
    {
        bool AllowPartialExecution { get; set; }
        bool AtLeastOneMustBeExecutable { get; set; }
        System.Collections.Generic.IEnumerable<System.Action> GetActions();
        System.Collections.Generic.IEnumerable<System.Action<object>> GetActionsWithParameter();
        System.Collections.Generic.IEnumerable<System.Windows.Input.ICommand> GetCommands();
        void RegisterAction(System.Action action);
        void RegisterAction(System.Action<object> action);
        void RegisterCommand(System.Windows.Input.ICommand command, Catel.MVVM.IViewModel viewModel = null);
        void UnregisterAction(System.Action action);
        void UnregisterAction(System.Action<object> action);
        void UnregisterCommand(System.Windows.Input.ICommand command);
    }
    public interface IDataContextSubscriptionService
    {
        Catel.MVVM.DataContextSubscriptionMode DefaultDataContextSubscriptionMode { get; set; }
        Catel.MVVM.DataContextSubscriptionMode GetDataContextSubscriptionMode(System.Type viewType);
    }
    public interface ILocator
    {
        System.Collections.Generic.List<string> NamingConventions { get; }
        void ClearCache();
    }
    public interface IRelationalViewModel : Catel.Data.IValidatable, Catel.MVVM.IViewModel, System.ComponentModel.IDataErrorInfo, System.ComponentModel.IDataWarningInfo, System.ComponentModel.INotifyDataErrorInfo, System.ComponentModel.INotifyDataWarningInfo, System.ComponentModel.INotifyPropertyChanged
    {
        Catel.MVVM.IViewModel ParentViewModel { get; }
        void RegisterChildViewModel(Catel.MVVM.IViewModel childViewModel);
        void SetParentViewModel(Catel.MVVM.IViewModel parentViewModel);
        void UnregisterChildViewModel(Catel.MVVM.IViewModel childViewModel);
    }
    public interface ITaskProgressReport
    {
        string Status { get; }
    }
    public interface IUrlLocator : Catel.MVVM.ILocator
    {
        void Register(System.Type viewModelType, string url);
        string ResolveUrl(System.Type viewModelType, bool ensurePageExists = true);
    }
    public interface IViewLocator : Catel.MVVM.ILocator
    {
        bool IsCompatible(System.Type viewModelType, System.Type viewType);
        void Register(System.Type viewModelType, System.Type viewType);
        System.Type ResolveView(System.Type viewModelType);
    }
    public interface IViewModel : Catel.Data.IValidatable, System.ComponentModel.IDataErrorInfo, System.ComponentModel.IDataWarningInfo, System.ComponentModel.INotifyDataErrorInfo, System.ComponentModel.INotifyDataWarningInfo, System.ComponentModel.INotifyPropertyChanged
    {
        bool IsCanceled { get; }
        bool IsClosed { get; }
        bool IsSaved { get; }
        string Title { get; }
        int UniqueIdentifier { get; }
        event Catel.AsyncEventHandler<System.EventArgs> CanceledAsync;
        event Catel.AsyncEventHandler<Catel.MVVM.CancelingEventArgs> CancelingAsync;
        event Catel.AsyncEventHandler<Catel.MVVM.ViewModelClosedEventArgs> ClosedAsync;
        event Catel.AsyncEventHandler<System.EventArgs> ClosingAsync;
        event Catel.AsyncEventHandler<Catel.MVVM.CommandExecutedEventArgs> CommandExecutedAsync;
        event Catel.AsyncEventHandler<System.EventArgs> InitializedAsync;
        event Catel.AsyncEventHandler<System.EventArgs> SavedAsync;
        event Catel.AsyncEventHandler<Catel.MVVM.SavingEventArgs> SavingAsync;
        System.Threading.Tasks.Task<bool> CancelViewModelAsync();
        System.Threading.Tasks.Task CloseViewModelAsync(bool? result);
        System.Threading.Tasks.Task InitializeViewModelAsync();
        System.Threading.Tasks.Task<bool> SaveViewModelAsync();
    }
    public interface IViewModelCommandManager
    {
        void AddHandler(System.Func<Catel.MVVM.IViewModel, string, System.Windows.Input.ICommand, object, System.Threading.Tasks.Task> handler);
        void InvalidateCommands(bool force = false);
    }
    public interface IViewModelContainer : System.ComponentModel.INotifyPropertyChanged
    {
        Catel.MVVM.IViewModel ViewModel { get; }
        event System.EventHandler<System.EventArgs> ViewModelChanged;
    }
    public static class IViewModelExtensions
    {
        public static System.Threading.Tasks.Task<bool> AwaitCancelingAsync(this Catel.MVVM.ViewModelBase viewModel, int timeout = 50) { }
        public static System.Threading.Tasks.Task AwaitClosingAsync(this Catel.MVVM.ViewModelBase viewModel, int timeout = 50) { }
        public static System.Threading.Tasks.Task<bool> AwaitSavingAsync(this Catel.MVVM.ViewModelBase viewModel, int timeout = 50) { }
        public static System.Threading.Tasks.Task<bool> CancelAndCloseViewModelAsync(this Catel.MVVM.IViewModel viewModel) { }
        public static bool? GetResult(this Catel.MVVM.IViewModel viewModel) { }
        public static System.Threading.Tasks.Task<bool> SaveAndCloseViewModelAsync(this Catel.MVVM.IViewModel viewModel) { }
    }
    public interface IViewModelFactory
    {
        bool CanReuseViewModel(System.Type viewType, System.Type expectedViewModelType, System.Type actualViewModelType, Catel.MVVM.IViewModel viewModelAsDataContext);
        Catel.MVVM.IViewModel CreateViewModel(System.Type viewModelType, object dataContext, object tag = null);
        bool IsViewModelWithModelInjection(System.Type viewModelType);
    }
    public static class IViewModelFactoryExtensions
    {
        public static TViewModel CreateViewModel<TViewModel>(this Catel.MVVM.IViewModelFactory viewModelFactory, object dataContext, object tag = null)
            where TViewModel : Catel.MVVM.IViewModel { }
    }
    public interface IViewModelLocator : Catel.MVVM.ILocator
    {
        bool IsCompatible(System.Type viewType, System.Type viewModelType);
        void Register(System.Type viewType, System.Type viewModelType);
        System.Type ResolveViewModel(System.Type viewType);
    }
    public static class IViewModelLocatorExtensions
    {
        public static void Register<TView, TViewModel>(this Catel.MVVM.IViewModelLocator viewModelLocator) { }
        public static System.Type ResolveViewModel<TView>(this Catel.MVVM.IViewModelLocator viewModelLocator) { }
    }
    public interface IViewModelManager
    {
        System.Collections.Generic.IEnumerable<Catel.MVVM.IViewModel> ActiveViewModels { get; }
        System.Collections.Generic.IEnumerable<Catel.MVVM.IRelationalViewModel> GetChildViewModels(Catel.MVVM.IViewModel parentViewModel);
        System.Collections.Generic.IEnumerable<Catel.MVVM.IRelationalViewModel> GetChildViewModels(int parentUniqueIdentifier);
        Catel.MVVM.IViewModel GetFirstOrDefaultInstance(System.Type viewModelType);
        TViewModel GetFirstOrDefaultInstance<TViewModel>()
            where TViewModel : Catel.MVVM.IViewModel;
        Catel.MVVM.IViewModel GetViewModel(int uniqueIdentifier);
        Catel.MVVM.IViewModel[] GetViewModelsOfModel(object model);
        void RegisterModel(Catel.MVVM.IViewModel viewModel, object model);
        void RegisterViewModelInstance(Catel.MVVM.IViewModel viewModel);
        void UnregisterAllModels(Catel.MVVM.IViewModel viewModel);
        void UnregisterModel(Catel.MVVM.IViewModel viewModel, object model);
        void UnregisterViewModelInstance(Catel.MVVM.IViewModel viewModel);
    }
    public interface IViewModelToModelConverter
    {
        bool CanConvert(System.Type[] types, System.Type outType, System.Type viewModelType);
        bool CanConvertBack(System.Type inType, System.Type[] outTypes, System.Type viewModelType);
        object Convert(object[] values, Catel.MVVM.IViewModel viewModel);
        object[] ConvertBack(object value, Catel.MVVM.IViewModel viewModel);
        bool ShouldConvert(string propertyName);
    }
    public class InvalidViewModelException : System.Exception
    {
        public InvalidViewModelException(string message) { }
    }
    public abstract class LocatorBase : Catel.MVVM.ILocator
    {
        protected LocatorBase() { }
        public System.Collections.Generic.List<string> NamingConventions { get; }
        protected void AddItemToCache(string valueToResolve, string resolvedValue) { }
        public void ClearCache() { }
        protected abstract System.Collections.Generic.IEnumerable<string> GetDefaultNamingConventions();
        protected string GetItemFromCache(string valueToResolve) { }
        protected void Register(string valueToResolve, string resolvedValue) { }
        protected virtual string Resolve(string valueToResolve) { }
        protected abstract string ResolveNamingConvention(string assembly, string typeToResolveName, string namingConvention);
        protected virtual System.Collections.Generic.IEnumerable<string> ResolveValues(string valueToResolve) { }
    }
    public class ModelAttribute : System.Attribute
    {
        public ModelAttribute() { }
        public bool SupportIEditableObject { get; set; }
        public bool SupportValidation { get; set; }
        public static bool SupportIEditableObjectDefaultValue { get; set; }
        public static bool SupportValidationDefaultValue { get; set; }
    }
    public enum ModelCleanUpMode
    {
        CancelEdit = 0,
        EndEdit = 1,
    }
    public class ModelNotRegisteredException : System.Exception
    {
        public ModelNotRegisteredException(string modelName, string propertyDeclaringViewModelToModelAttribute) { }
        public string ModelName { get; }
        public string PropertyDeclaringViewModelToModelAttribute { get; }
    }
    public static class ModuleInitializer
    {
        public static void Initialize() { }
    }
    public static class ObjectExtensions
    {
        public static bool IsSentinelBindingObject(this object dataContext) { }
    }
    public class ObjectToDisplayNameConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public ObjectToDisplayNameConverter() { }
        public Catel.Services.ILanguageService LanguageService { get; set; }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
        protected string GetDisplayName(Catel.ComponentModel.DisplayNameAttribute attribute) { }
    }
    public class ProgressiveTaskCommand<TProgress> : Catel.MVVM.TaskCommand<object, object, TProgress>
        where TProgress : Catel.MVVM.ITaskProgressReport
    {
        public ProgressiveTaskCommand(System.Func<System.Threading.CancellationToken, System.IProgress<TProgress>, System.Threading.Tasks.Task> execute, System.Func<bool> canExecute = null, System.Action<TProgress> reportProgress = null, object tag = null) { }
    }
    public class ProgressiveTaskCommand<TProgress, TExecuteParameter> : Catel.MVVM.TaskCommand<TExecuteParameter, TExecuteParameter, TProgress>
        where TProgress : Catel.MVVM.ITaskProgressReport
    {
        public ProgressiveTaskCommand(System.Func<System.Threading.CancellationToken, System.IProgress<TProgress>, System.Threading.Tasks.Task> execute, System.Func<bool> canExecute = null, System.Action<TProgress> reportProgress = null, object tag = null) { }
    }
    public class PropertyNotFoundInModelException : System.Exception
    {
        public PropertyNotFoundInModelException(string viewModelPropertyName, string modelName, string modelPropertyName) { }
        public string ModelName { get; }
        public string ModelPropertyName { get; }
        public string ViewModelPropertyName { get; }
    }
    public class SavingEventArgs : Catel.MVVM.CancellableEventArgs
    {
        public SavingEventArgs() { }
    }
    public class TaskCommand : Catel.MVVM.TaskCommand<object, object, Catel.MVVM.ITaskProgressReport>
    {
        public TaskCommand(System.Func<System.Threading.Tasks.Task> execute, System.Func<bool> canExecute = null, object tag = null) { }
        public TaskCommand(System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task> execute, System.Func<bool> canExecute = null, object tag = null) { }
    }
    public class TaskCommand<TExecuteParameter> : Catel.MVVM.TaskCommand<TExecuteParameter, TExecuteParameter>
    {
        public TaskCommand(System.Func<TExecuteParameter, System.Threading.Tasks.Task> execute, System.Func<TExecuteParameter, bool> canExecute = null, object tag = null) { }
        public TaskCommand(System.Func<TExecuteParameter, System.Threading.CancellationToken, System.Threading.Tasks.Task> execute, System.Func<TExecuteParameter, bool> canExecute = null, object tag = null) { }
    }
    public class TaskCommand<TExecuteParameter, TCanExecuteParameter> : Catel.MVVM.TaskCommand<TExecuteParameter, TCanExecuteParameter, Catel.MVVM.ITaskProgressReport>
    {
        public TaskCommand(System.Func<TExecuteParameter, System.Threading.Tasks.Task> execute, System.Func<TCanExecuteParameter, bool> canExecute = null, object tag = null) { }
        public TaskCommand(System.Func<TExecuteParameter, System.Threading.CancellationToken, System.Threading.Tasks.Task> execute, System.Func<TCanExecuteParameter, bool> canExecute = null, object tag = null) { }
    }
    public class TaskCommand<TExecuteParameter, TCanExecuteParameter, TProgress> : Catel.MVVM.Command<TExecuteParameter, TCanExecuteParameter>, Catel.MVVM.ICatelCommand, Catel.MVVM.ICatelTaskCommand<TProgress>, System.Windows.Input.ICommand
        where TProgress : Catel.MVVM.ITaskProgressReport
    {
        public TaskCommand(System.Func<System.Threading.Tasks.Task> execute, System.Func<bool> canExecute = null, object tag = null) { }
        public TaskCommand(System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task> execute, System.Func<bool> canExecute = null, object tag = null) { }
        public TaskCommand(System.Func<TExecuteParameter, System.Threading.Tasks.Task> execute, System.Func<TCanExecuteParameter, bool> canExecute = null, object tag = null) { }
        public TaskCommand(System.Func<TExecuteParameter, System.Threading.CancellationToken, System.Threading.Tasks.Task> execute, System.Func<TCanExecuteParameter, bool> canExecute = null, object tag = null) { }
        public TaskCommand(System.Func<System.Threading.CancellationToken, System.IProgress<TProgress>, System.Threading.Tasks.Task> execute, System.Func<bool> canExecute = null, System.Action<TProgress> reportProgress = null, object tag = null) { }
        public TaskCommand(System.Func<TExecuteParameter, System.Threading.CancellationToken, System.IProgress<TProgress>, System.Threading.Tasks.Task> execute, System.Func<TCanExecuteParameter, bool> canExecute = null, System.Action<TProgress> reportProgress = null, object tag = null) { }
        public Catel.MVVM.Command CancelCommand { get; }
        public bool IsCancellationRequested { get; }
        public bool IsExecuting { get; }
        public bool SwallowExceptions { get; set; }
        public System.Threading.Tasks.Task Task { get; }
        public event System.EventHandler<Catel.MVVM.CommandEventArgs> Canceled;
        public event System.EventHandler<Catel.MVVM.CommandCanceledEventArgs> Executing;
        public event System.EventHandler<Catel.MVVM.CommandProgressChangedEventArgs<TProgress>> ProgressChanged;
        public override bool CanExecute(TCanExecuteParameter parameter) { }
        public void Cancel() { }
        protected override void Execute(TExecuteParameter parameter, bool ignoreCanExecuteCheck) { }
        public override void RaiseCanExecuteChanged() { }
    }
    public class UrlLocator : Catel.MVVM.LocatorBase, Catel.MVVM.ILocator, Catel.MVVM.IUrlLocator
    {
        public UrlLocator() { }
        protected override System.Collections.Generic.IEnumerable<string> GetDefaultNamingConventions() { }
        public void Register(System.Type viewModelType, string url) { }
        protected override string ResolveNamingConvention(string assembly, string typeToResolveName, string namingConvention) { }
        public virtual string ResolveUrl(System.Type viewModelType, bool ensurePageExists = true) { }
    }
    public static class ViewHelper
    {
        public static System.Windows.FrameworkElement ConstructViewWithViewModel(System.Type viewType, object dataContext) { }
        public static T ConstructViewWithViewModel<T>(System.Type viewType, object dataContext)
            where T : System.Windows.FrameworkElement { }
    }
    public class ViewLocator : Catel.MVVM.LocatorBase, Catel.MVVM.ILocator, Catel.MVVM.IViewLocator
    {
        public ViewLocator() { }
        protected override System.Collections.Generic.IEnumerable<string> GetDefaultNamingConventions() { }
        public virtual bool IsCompatible(System.Type viewModelType, System.Type viewType) { }
        public void Register(System.Type viewModelType, System.Type viewType) { }
        protected override string ResolveNamingConvention(string assembly, string typeToResolveName, string namingConvention) { }
        public virtual System.Type ResolveView(System.Type viewModelType) { }
    }
    public abstract class ViewModelBase : Catel.Data.ValidatableModelBase, Catel.Data.IValidatable, Catel.IUniqueIdentifyable, Catel.MVVM.IRelationalViewModel, Catel.MVVM.IViewModel, System.ComponentModel.IDataErrorInfo, System.ComponentModel.IDataWarningInfo, System.ComponentModel.INotifyDataErrorInfo, System.ComponentModel.INotifyDataWarningInfo, System.ComponentModel.INotifyPropertyChanged
    {
        protected readonly Catel.Data.IObjectAdapter _objectAdapter;
        protected static readonly Catel.MVVM.IViewModelManager ViewModelManager;
        protected ViewModelBase() { }
        protected ViewModelBase(bool supportIEditableObject, bool ignoreMultipleModelsWarning = false, bool skipViewModelAttributesInitialization = false) { }
        protected ViewModelBase(Catel.IoC.IServiceLocator serviceLocator, bool supportIEditableObject = true, bool ignoreMultipleModelsWarning = false, bool skipViewModelAttributesInitialization = false) { }
        [Catel.Data.ExcludeFromValidation]
        protected bool DeferValidationUntilFirstSaveCall { get; set; }
        [Catel.Data.ExcludeFromValidation]
        protected Catel.IoC.IDependencyResolver DependencyResolver { get; }
        [Catel.Data.ExcludeFromValidation]
        protected bool DispatchPropertyChangedEvent { get; set; }
        [Catel.Data.ExcludeFromValidation]
        public override bool HasErrors { get; }
        [Catel.Data.ExcludeFromValidation]
        protected bool InvalidateCommandsOnPropertyChanged { get; set; }
        [Catel.Data.ExcludeFromValidation]
        public bool IsCanceled { get; }
        [Catel.Data.ExcludeFromValidation]
        protected bool IsCanceling { get; }
        [Catel.Data.ExcludeFromValidation]
        public bool IsClosed { get; }
        protected bool IsClosing { get; }
        [Catel.Data.ExcludeFromValidation]
        protected bool IsInitialized { get; }
        [Catel.Data.ExcludeFromValidation]
        protected bool IsInitializing { get; }
        [Catel.Data.ExcludeFromValidation]
        public bool IsSaved { get; }
        [Catel.Data.ExcludeFromValidation]
        protected bool IsSaving { get; }
        [Catel.Data.ExcludeFromValidation]
        protected Catel.MVVM.Navigation.NavigationContext NavigationContext { get; }
        [Catel.Data.ExcludeFromValidation]
        public Catel.MVVM.IViewModel ParentViewModel { get; }
        [Catel.Data.ExcludeFromValidation]
        protected System.TimeSpan ThrottlingRate { get; set; }
        [Catel.Data.ExcludeFromValidation]
        public virtual string Title { get; set; }
        [Catel.Data.ExcludeFromValidation]
        public int UniqueIdentifier { get; }
        [Catel.Data.ExcludeFromValidation]
        protected bool ValidateModelsOnInitialization { get; set; }
        [Catel.Data.ExcludeFromValidation]
        protected Catel.MVVM.IViewModelCommandManager ViewModelCommandManager { get; }
        [Catel.Data.ExcludeFromValidation]
        public System.DateTime ViewModelConstructionTime { get; }
        public event Catel.AsyncEventHandler<System.EventArgs> CanceledAsync;
        public event Catel.AsyncEventHandler<Catel.MVVM.CancelingEventArgs> CancelingAsync;
        public event Catel.AsyncEventHandler<Catel.MVVM.ViewModelClosedEventArgs> ClosedAsync;
        public event Catel.AsyncEventHandler<System.EventArgs> ClosingAsync;
        public event Catel.AsyncEventHandler<Catel.MVVM.CommandExecutedEventArgs> CommandExecutedAsync;
        public event Catel.AsyncEventHandler<System.EventArgs> InitializedAsync;
        public event System.EventHandler NavigationCompleted;
        public event Catel.AsyncEventHandler<System.EventArgs> SavedAsync;
        public event Catel.AsyncEventHandler<Catel.MVVM.SavingEventArgs> SavingAsync;
        protected virtual System.Threading.Tasks.Task<bool> CancelAsync() { }
        public System.Threading.Tasks.Task<bool> CancelViewModelAsync() { }
        protected virtual System.Threading.Tasks.Task CloseAsync() { }
        public System.Threading.Tasks.Task CloseViewModelAsync(bool? result) { }
        protected object[] GetAllModels() { }
        protected System.Collections.Generic.IEnumerable<Catel.MVVM.IViewModel> GetChildViewModels() { }
        protected virtual int GetObjectId(Catel.Services.IObjectIdGenerator<int> objectIdGenerator) { }
        protected virtual System.Type GetObjectIdGeneratorType() { }
        protected virtual System.Threading.Tasks.Task InitializeAsync() { }
        protected virtual void InitializeModel(string modelProperty, object model) { }
        public System.Threading.Tasks.Task InitializeViewModelAsync() { }
        protected void InitializeViewModelAttributes() { }
        protected bool IsModelRegistered(string name) { }
        protected virtual System.Threading.Tasks.Task OnClosedAsync(bool? result) { }
        protected virtual System.Threading.Tasks.Task OnClosingAsync() { }
        protected virtual void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) { }
        protected virtual void OnNavigationCompleted() { }
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) { }
        protected override void OnValidating(Catel.Data.IValidationContext validationContext) { }
        protected override void OnValidatingBusinessRules(Catel.Data.IValidationContext validationContext) { }
        protected override void OnValidatingFields(Catel.Data.IValidationContext validationContext) { }
        protected override void RaisePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) { }
        protected void RegisterViewModelServices(Catel.IoC.IServiceLocator serviceLocator) { }
        protected void ResetModel(string modelProperty, Catel.MVVM.ModelCleanUpMode modelCleanUpMode) { }
        protected virtual System.Threading.Tasks.Task<bool> SaveAsync() { }
        public System.Threading.Tasks.Task<bool> SaveViewModelAsync() { }
        public override string ToString() { }
        protected virtual void UninitializeModel(string modelProperty, object model, Catel.MVVM.ModelCleanUpMode modelCleanUpMode) { }
        protected void UpdateExplicitViewModelToModelMappings() { }
        public void UpdateNavigationContext(Catel.MVVM.Navigation.NavigationContext navigationContext) { }
        public override void Validate(bool force = false) { }
    }
    public class ViewModelClosedEventArgs : System.EventArgs
    {
        public ViewModelClosedEventArgs(Catel.MVVM.IViewModel viewModel, bool? result) { }
        public bool? Result { get; }
        public Catel.MVVM.IViewModel ViewModel { get; }
    }
    public class ViewModelCommandManager : Catel.MVVM.IViewModelCommandManager
    {
        public void AddHandler(System.Func<Catel.MVVM.IViewModel, string, System.Windows.Input.ICommand, object, System.Threading.Tasks.Task> handler) { }
        public void InvalidateCommands(bool force = false) { }
        public static Catel.MVVM.IViewModelCommandManager Create(Catel.MVVM.IViewModel viewModel) { }
    }
    public static class ViewModelExtensions
    {
        public static Catel.Data.IValidationSummary GetValidationSummary(this Catel.MVVM.ViewModelBase viewModel, bool includeChildViewModelValidations) { }
        public static Catel.Data.IValidationSummary GetValidationSummary(this Catel.MVVM.ViewModelBase viewModel, bool includeChildViewModelValidations, object tag) { }
        public static Catel.MVVM.IViewModelCommandManager GetViewModelCommandManager(this Catel.MVVM.ViewModelBase viewModel) { }
        public static bool IsValidationSummaryOutdated(this Catel.MVVM.ViewModelBase viewModel, long lastUpdated, bool includeChildViewModelValidations) { }
    }
    public class ViewModelFactory : Catel.MVVM.IViewModelFactory
    {
        public ViewModelFactory(Catel.IoC.ITypeFactory typeFactory, Catel.IoC.IServiceLocator serviceLocator) { }
        public virtual bool CanReuseViewModel(System.Type viewType, System.Type expectedViewModelType, System.Type actualViewModelType, Catel.MVVM.IViewModel viewModelAsDataContext) { }
        public virtual Catel.MVVM.IViewModel CreateViewModel(System.Type viewModelType, object dataContext, object tag = null) { }
        public virtual bool IsViewModelWithModelInjection(System.Type viewModelType) { }
    }
    public enum ViewModelLifetimeManagement
    {
        Automatic = 0,
        PartlyManual = 1,
        FullyManual = 2,
    }
    public class ViewModelLocator : Catel.MVVM.LocatorBase, Catel.MVVM.ILocator, Catel.MVVM.IViewModelLocator
    {
        public ViewModelLocator() { }
        protected override System.Collections.Generic.IEnumerable<string> GetDefaultNamingConventions() { }
        public virtual bool IsCompatible(System.Type viewType, System.Type viewModelType) { }
        public void Register(System.Type viewType, System.Type viewModelType) { }
        protected override string ResolveNamingConvention(string assembly, string typeToResolveName, string namingConvention) { }
        public virtual System.Type ResolveViewModel(System.Type viewType) { }
    }
    public class ViewModelManager : Catel.MVVM.IViewModelManager
    {
        public ViewModelManager() { }
        public System.Collections.Generic.IEnumerable<Catel.MVVM.IViewModel> ActiveViewModels { get; }
        public int ViewModelCount { get; }
        public System.Collections.Generic.IEnumerable<Catel.MVVM.IRelationalViewModel> GetChildViewModels(Catel.MVVM.IViewModel parentViewModel) { }
        public System.Collections.Generic.IEnumerable<Catel.MVVM.IRelationalViewModel> GetChildViewModels(int parentUniqueIdentifier) { }
        public Catel.MVVM.IViewModel GetFirstOrDefaultInstance(System.Type viewModelType) { }
        public TViewModel GetFirstOrDefaultInstance<TViewModel>()
            where TViewModel : Catel.MVVM.IViewModel { }
        public Catel.MVVM.IViewModel GetViewModel(int uniqueIdentifier) { }
        public Catel.MVVM.IViewModel[] GetViewModelsOfModel(object model) { }
        public void RegisterModel(Catel.MVVM.IViewModel viewModel, object model) { }
        public void RegisterViewModelInstance(Catel.MVVM.IViewModel viewModel) { }
        public void UnregisterAllModels(Catel.MVVM.IViewModel viewModel) { }
        public void UnregisterModel(Catel.MVVM.IViewModel viewModel, object model) { }
        public void UnregisterViewModelInstance(Catel.MVVM.IViewModel viewModel) { }
    }
    public static class ViewModelManagerExtensions
    {
        public static System.Threading.Tasks.Task CancelAndCloseViewModelsAsync(this Catel.MVVM.IViewModelManager viewModelManager, System.Func<Catel.MVVM.IViewModel, bool> predicate) { }
        public static System.Threading.Tasks.Task SaveAndCloseViewModelsAsync(this Catel.MVVM.IViewModelManager viewModelManager, System.Func<Catel.MVVM.IViewModel, bool> predicate) { }
    }
    public class ViewModelNotRegisteredException : System.Exception
    {
        public ViewModelNotRegisteredException(System.Type viewModelType) { }
        public System.Type ViewModelType { get; }
    }
    public static class ViewModelServiceHelper
    {
        public static void RegisterDefaultViewModelServices(Catel.IoC.IServiceLocator serviceLocator) { }
    }
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.All)]
    public class ViewModelToModelAttribute : System.Attribute
    {
        public ViewModelToModelAttribute(string model = "", string property = "") { }
        public object[] AdditionalConstructorArgs { get; set; }
        public string[] AdditionalPropertiesToWatch { get; set; }
        public System.Type ConverterType { get; set; }
        public Catel.MVVM.ViewModelToModelMode Mode { get; set; }
        public string Model { get; }
        public string Property { get; }
    }
    public abstract class ViewModelToModelConverterBase : Catel.MVVM.IViewModelToModelConverter
    {
        protected ViewModelToModelConverterBase(string[] propertyNames) { }
        public string[] PropertyNames { get; }
        public abstract bool CanConvert(System.Type[] types, System.Type outType, System.Type viewModelType);
        public abstract bool CanConvertBack(System.Type inType, System.Type[] outTypes, System.Type viewModelType);
        public abstract object Convert(object[] values, Catel.MVVM.IViewModel viewModel);
        public abstract object[] ConvertBack(object value, Catel.MVVM.IViewModel viewModel);
        public bool ShouldConvert(string propertyName) { }
    }
    public class ViewModelToModelMapping
    {
        public ViewModelToModelMapping(System.Reflection.PropertyInfo viewModelPropertyInfo, System.Type modelPropertyType, Catel.MVVM.ViewModelToModelAttribute attribute) { }
        public ViewModelToModelMapping(string viewModelProperty, System.Type viewModelPropertyType, System.Type modelPropertyType, Catel.MVVM.ViewModelToModelAttribute attribute) { }
        public ViewModelToModelMapping(string viewModelProperty, System.Type viewModelPropertyType, string modelProperty, System.Type modelPropertyType, string valueProperty, Catel.MVVM.ViewModelToModelMode mode, System.Type converterType, object[] additionalConstructorArgs, string[] additionalPropertiesToWatch) { }
        public Catel.MVVM.IViewModelToModelConverter Converter { get; }
        public System.Type ConverterType { get; }
        public System.Collections.Generic.HashSet<string> IgnoredProperties { get; }
        public Catel.MVVM.ViewModelToModelMode Mode { get; }
        public string ModelProperty { get; }
        public System.Type ModelPropertyType { get; }
        public string[] ValueProperties { get; }
        public string ViewModelProperty { get; }
        public System.Type ViewModelPropertyType { get; }
        public override string ToString() { }
    }
    public enum ViewModelToModelMode
    {
        TwoWay = 0,
        OneWay = 1,
        OneWayToSource = 2,
        Explicit = 3,
    }
    public class WrongViewModelTypeException : System.Exception
    {
        public WrongViewModelTypeException(System.Type actualType, System.Type expectedType) { }
        public System.Type ActualType { get; }
        public System.Type ExpectedType { get; }
    }
}
namespace Catel.MVVM.Converters
{
    [System.Windows.Data.ValueConversion(typeof(object), typeof(object))]
    public class AreEqualMultiValueConverter : System.Windows.Markup.MarkupExtension, System.Windows.Data.IMultiValueConverter
    {
        public AreEqualMultiValueConverter() { }
        public object Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
        public object[] ConvertBack(object value, System.Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) { }
        public override object ProvideValue(System.IServiceProvider serviceProvider) { }
    }
    [System.Windows.Data.ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    public class BooleanToCollapsingVisibilityConverter : Catel.MVVM.Converters.VisibilityConverterBase
    {
        public BooleanToCollapsingVisibilityConverter() { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter) { }
        protected override bool IsVisible(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(bool), typeof(double))]
    public class BooleanToGrayscaleConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public BooleanToGrayscaleConverter() { }
        public double FalseResult { get; set; }
        public double TrueResult { get; set; }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    public class BooleanToHidingVisibilityConverter : Catel.MVVM.Converters.BooleanToCollapsingVisibilityConverter
    {
        public BooleanToHidingVisibilityConverter() { }
    }
    [System.Windows.Data.ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanToOppositeBooleanConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public BooleanToOppositeBooleanConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToTextConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public BooleanToTextConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    public abstract class CollapsingVisibilityConverterBase : Catel.MVVM.Converters.VisibilityConverterBase
    {
        protected CollapsingVisibilityConverterBase() { }
    }
    public class CollectionToCollapsingVisibilityConverter : Catel.MVVM.Converters.VisibilityConverterBase
    {
        public CollectionToCollapsingVisibilityConverter() { }
        public CollectionToCollapsingVisibilityConverter(System.Windows.Visibility visibility) { }
        protected override bool IsVisible(object value, System.Type targetType, object parameter) { }
    }
    public class CollectionToCountConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public CollectionToCountConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    public class CollectionToHidingVisibilityConverter : Catel.MVVM.Converters.CollectionToCollapsingVisibilityConverter
    {
        public CollectionToHidingVisibilityConverter() { }
    }
    [System.Windows.Data.ValueConversion(typeof(System.Windows.Media.Color), typeof(System.Windows.Media.Brush))]
    public class ColorToBrushConverter : Catel.MVVM.Converters.ValueConverterBase<System.Windows.Media.Color, System.Windows.Media.Brush>
    {
        public ColorToBrushConverter() { }
        protected override object Convert(System.Windows.Media.Color value, System.Type targetType, object parameter) { }
        protected override object ConvertBack(System.Windows.Media.Brush value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(System.Collections.IEnumerable), typeof(bool))]
    public class ContainsItemsConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public ContainsItemsConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    public static class ConverterHelper
    {
        public static readonly object UnsetValue;
        public static bool ShouldInvert(object parameter) { }
    }
    public class DebugConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public DebugConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(string), typeof(System.Windows.Visibility))]
    public class EmptyStringToCollapsingVisibilityConverter : Catel.MVVM.Converters.VisibilityConverterBase
    {
        public EmptyStringToCollapsingVisibilityConverter() { }
        protected override bool IsVisible(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(string), typeof(System.Windows.Visibility))]
    public class EmptyStringToHidingVisibilityConverter : Catel.MVVM.Converters.EmptyStringToCollapsingVisibilityConverter
    {
        public EmptyStringToHidingVisibilityConverter() { }
    }
    public class EnumToCollapsingVisibilityConverter : Catel.MVVM.Converters.VisibilityConverterBase
    {
        public EnumToCollapsingVisibilityConverter() { }
        protected override bool IsVisible(object value, System.Type targetType, object parameter) { }
    }
    public class EnumToHidingVisibilityConverter : Catel.MVVM.Converters.EnumToCollapsingVisibilityConverter
    {
        public EnumToHidingVisibilityConverter() { }
    }
    public abstract class EventArgsConverterBase<TArgs> : Catel.MVVM.Converters.IEventArgsConverter
        where TArgs : System.EventArgs
    {
        protected EventArgsConverterBase() { }
        protected abstract object Convert(object sender, TArgs args);
    }
    [System.Windows.Data.ValueConversion(typeof(object), typeof(string))]
    public class FormattingConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public FormattingConverter() { }
        protected FormattingConverter(string defaultFormatString) { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(System.Collections.Generic.ICollection<System.Windows.Controls.ValidationError>), typeof(string))]
    public class GetFirstValidationErrorConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public GetFirstValidationErrorConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    public abstract class HidingVisibilityConverterBase : Catel.MVVM.Converters.VisibilityConverterBase
    {
        protected HidingVisibilityConverterBase() { }
    }
    public interface IEventArgsConverter
    {
        object Convert(object sender, object args);
    }
    public interface IValueConverter : System.Windows.Data.IValueConverter { }
    [System.Windows.Data.ValueConversion(typeof(string), typeof(System.Nullable<int>))]
    public class IntToStringConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public IntToStringConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(System.Nullable<bool>), typeof(bool))]
    public class IsSelectedConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public IsSelectedConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(System.Nullable<int>), typeof(bool))]
    public class IsSelectedValueConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public IsSelectedValueConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter) { }
    }
    public class LanguageConverter : Catel.MVVM.Converters.ValueConverterBase<string>
    {
        public LanguageConverter() { }
        protected override object Convert(string value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(string), typeof(object))]
    public class MethodToValueConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public MethodToValueConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(int), typeof(int))]
    public class MultiplyConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public MultiplyConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter) { }
    }
    public class PlatformToBooleanConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public PlatformToBooleanConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(object), typeof(bool))]
    public class ReferenceToBooleanConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public ReferenceToBooleanConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(object), typeof(System.Windows.Visibility))]
    public class ReferenceToCollapsingVisibilityConverter : Catel.MVVM.Converters.VisibilityConverterBase
    {
        public ReferenceToCollapsingVisibilityConverter() { }
        protected override bool IsVisible(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(object), typeof(System.Windows.Visibility))]
    public class ReferenceToHidingVisibilityConverter : Catel.MVVM.Converters.ReferenceToCollapsingVisibilityConverter
    {
        public ReferenceToHidingVisibilityConverter() { }
    }
    [System.Windows.Data.ValueConversion(typeof(System.DateTime), typeof(string))]
    public class ShortDateFormattingConverter : Catel.MVVM.Converters.FormattingConverter
    {
        public ShortDateFormattingConverter() { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversion(typeof(string), typeof(System.Nullable<int>))]
    public class StringToIntConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public StringToIntConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter) { }
    }
    public class StringToTypeConverter : System.ComponentModel.TypeConverter
    {
        public StringToTypeConverter() { }
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { }
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { }
    }
    public class TextToLowerCaseConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public TextToLowerCaseConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    public class TextToUpperCaseConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public TextToUpperCaseConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    public abstract class ValueConverterBase : Catel.MVVM.Converters.ValueConverterBase<object>
    {
        protected ValueConverterBase() { }
    }
    public abstract class ValueConverterBase<TConvert> : Catel.MVVM.Converters.ValueConverterBase<TConvert, object>
    {
        protected ValueConverterBase() { }
    }
    public abstract class ValueConverterBase<TConvert, TConvertBack> : System.Windows.Markup.MarkupExtension, Catel.MVVM.Converters.IValueConverter, System.Windows.Data.IValueConverter
    {
        public ValueConverterBase() { }
        [System.ComponentModel.TypeConverter(typeof(Catel.MVVM.Converters.StringToTypeConverter))]
        public System.Type BackOverrideType { get; set; }
        protected System.Globalization.CultureInfo CurrentCulture { get; }
        public Catel.MVVM.Converters.IValueConverter Link { get; set; }
        [System.ComponentModel.TypeConverter(typeof(Catel.MVVM.Converters.StringToTypeConverter))]
        public System.Type OverrideType { get; set; }
        public bool SupportInversionUsingCommandParameter { get; set; }
        protected abstract object Convert(TConvert value, System.Type targetType, object parameter);
        public virtual object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
        protected virtual object ConvertBack(TConvertBack value, System.Type targetType, object parameter) { }
        public virtual object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
        protected virtual bool IsConvertable<T>(object value) { }
        public override object ProvideValue(System.IServiceProvider serviceProvider) { }
    }
    [System.Windows.Data.ValueConversion(typeof(object), typeof(object))]
    public class ViewModelToViewConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public ViewModelToViewConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    public abstract class VisibilityConverterBase : Catel.MVVM.Converters.ValueConverterBase
    {
        protected VisibilityConverterBase(System.Windows.Visibility notVisibleVisibility) { }
        public System.Windows.Visibility NotVisibleVisibility { get; }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
        protected abstract bool IsVisible(object value, System.Type targetType, object parameter);
    }
}
namespace Catel.MVVM.Navigation
{
    public class NavigatedEventArgs : Catel.MVVM.Navigation.NavigationEventArgsBase
    {
        public NavigatedEventArgs(string uri, Catel.MVVM.Navigation.NavigationMode navigationMode) { }
    }
    public class NavigatingEventArgs : Catel.MVVM.Navigation.NavigationEventArgsBase
    {
        public NavigatingEventArgs(string uri, Catel.MVVM.Navigation.NavigationMode navigationMode) { }
        public bool Cancel { get; set; }
    }
    public class NavigationAdapter : Catel.MVVM.Navigation.NavigationAdapterBase
    {
        public NavigationAdapter(Catel.MVVM.Views.IView navigationTarget, object navigationRoot) { }
        public Catel.MVVM.Navigation.NavigationContext NavigationContext { get; }
        public object NavigationRoot { get; }
        public Catel.MVVM.Views.IView NavigationTarget { get; }
        public System.Type NavigationTargetType { get; }
        public event System.EventHandler<Catel.MVVM.Navigation.NavigatedEventArgs> NavigatedAway;
        public event System.EventHandler<Catel.MVVM.Navigation.NavigatedEventArgs> NavigatedTo;
        public event System.EventHandler<Catel.MVVM.Navigation.NavigatingEventArgs> NavigatingAway;
        protected override bool CanHandleNavigation() { }
        protected override string GetNavigationUri(object target) { }
        public string GetNavigationUriForTargetPage() { }
        protected void RaiseNavigatedAway(Catel.MVVM.Navigation.NavigatedEventArgs e) { }
        protected void RaiseNavigatedTo(Catel.MVVM.Navigation.NavigatedEventArgs e) { }
        protected void RaiseNavigatingAway(Catel.MVVM.Navigation.NavigatingEventArgs e) { }
        public void UninitializeNavigationService() { }
    }
    public abstract class NavigationAdapterBase
    {
        protected NavigationAdapterBase() { }
        protected virtual bool CanHandleNavigation() { }
        protected abstract string GetNavigationUri(object target);
    }
    public class NavigationContext
    {
        public NavigationContext() { }
        public System.Collections.Generic.Dictionary<string, object> Values { get; }
    }
    public abstract class NavigationEventArgsBase : System.EventArgs
    {
        protected NavigationEventArgsBase(string uri, Catel.MVVM.Navigation.NavigationMode navigationMode) { }
        public Catel.MVVM.Navigation.NavigationMode NavigationMode { get; }
        public string Uri { get; }
    }
    public static class NavigationEventArgsExtensions
    {
        public static string GetUriWithoutQueryInfo(this System.Windows.Navigation.NavigatingCancelEventArgs e) { }
        public static string GetUriWithoutQueryInfo(this System.Windows.Navigation.NavigationEventArgs e) { }
        public static string GetUriWithoutQueryInfo(this string uri) { }
        public static bool IsNavigationForView(this System.Windows.Navigation.NavigatingCancelEventArgs e, System.Type viewType) { }
        public static bool IsNavigationForView(this System.Windows.Navigation.NavigationEventArgs e, System.Type viewType) { }
        public static bool IsNavigationForView(this string uriString, System.Type viewType) { }
        public static bool IsNavigationToExternal(this System.Uri uri) { }
        public static bool IsNavigationToExternal(this string uriString) { }
    }
    public enum NavigationMode
    {
        Back = 0,
        Forward = 1,
        New = 2,
        Refresh = 3,
        Unknown = 4,
    }
    public static class NavigationModeExtensions
    {
        public static Catel.MVVM.Navigation.NavigationMode Convert(this System.Windows.Navigation.NavigationMode navigationMode) { }
    }
}
namespace Catel.MVVM.Providers
{
    public class DetermineViewModelInstanceEventArgs : System.EventArgs
    {
        public DetermineViewModelInstanceEventArgs(object dataContext) { }
        public object DataContext { get; }
        public bool DoNotCreateViewModel { get; set; }
        public Catel.MVVM.IViewModel ViewModel { get; set; }
    }
    public class DetermineViewModelTypeEventArgs : System.EventArgs
    {
        public DetermineViewModelTypeEventArgs(object dataContext) { }
        public object DataContext { get; }
        public System.Type ViewModelType { get; set; }
    }
    public abstract class LogicBase : Catel.Data.ObservableObject, Catel.IUniqueIdentifyable, Catel.MVVM.Views.IViewLoadState
    {
        protected readonly object _lockObject;
        protected static readonly Catel.MVVM.Views.IViewLoadManager ViewLoadManager;
        protected LogicBase(Catel.MVVM.Views.IView targetView, System.Type viewModelType = null, Catel.MVVM.IViewModel viewModel = null) { }
        protected virtual bool CanViewBeLoaded { get; }
        public bool HasVmProperty { get; }
        protected bool IgnoreNullDataContext { get; set; }
        protected bool IsClosingViewModel { get; }
        protected bool IsLoading { get; }
        public bool IsTargetViewLoaded { get; }
        protected bool IsUnloading { get; }
        protected System.WeakReference LastKnownDataContext { get; }
        protected Catel.MVVM.Views.IView TargetView { get; set; }
        protected System.Type TargetViewType { get; }
        public int UniqueIdentifier { get; }
        public Catel.MVVM.IViewModel ViewModel { get; set; }
        public Catel.MVVM.Providers.LogicViewModelBehavior ViewModelBehavior { get; }
        protected Catel.MVVM.IViewModelFactory ViewModelFactory { get; }
        public Catel.MVVM.ViewModelLifetimeManagement ViewModelLifetimeManagement { get; set; }
        public System.Type ViewModelType { get; }
        public event System.EventHandler<Catel.MVVM.Providers.DetermineViewModelInstanceEventArgs> DetermineViewModelInstance;
        public event System.EventHandler<Catel.MVVM.Providers.DetermineViewModelTypeEventArgs> DetermineViewModelType;
        public event System.EventHandler<System.EventArgs> Loaded;
        public event System.EventHandler<System.ComponentModel.PropertyChangedEventArgs> TargetViewPropertyChanged;
        public event System.EventHandler<System.EventArgs> Unloaded;
        public event Catel.AsyncEventHandler<System.EventArgs> ViewModelCanceledAsync;
        public event System.EventHandler<System.EventArgs> ViewModelChanged;
        public event Catel.AsyncEventHandler<Catel.MVVM.ViewModelClosedEventArgs> ViewModelClosedAsync;
        public event System.EventHandler<System.ComponentModel.PropertyChangedEventArgs> ViewModelPropertyChanged;
        public event Catel.AsyncEventHandler<System.EventArgs> ViewModelSavedAsync;
        public System.Threading.Tasks.Task<bool> CancelAndCloseViewModelAsync() { }
        public virtual System.Threading.Tasks.Task<bool> CancelViewModelAsync() { }
        public virtual System.Threading.Tasks.Task CloseViewModelAsync(bool? result) { }
        public virtual System.Threading.Tasks.Task CloseViewModelAsync(bool? result, bool dispose) { }
        protected System.Threading.Tasks.Task CompleteViewModelClosingAsync() { }
        protected Catel.MVVM.IViewModel ConstructViewModelUsingArgumentOrDefaultConstructor(object injectionObject) { }
        protected Catel.MVVM.IViewModel CreateViewModelByUsingDataContextOrConstructor() { }
        protected virtual object GetDataContext(Catel.MVVM.Views.IView view) { }
        protected bool IsCurrentDataContext(Catel.MVVM.Views.DataContextChangedEventArgs e) { }
        public virtual void OnTargetViewDataContextChanged(object sender, Catel.MVVM.Views.DataContextChangedEventArgs e) { }
        public virtual System.Threading.Tasks.Task OnTargetViewLoadedAsync(object sender, System.EventArgs e) { }
        public virtual void OnTargetViewPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) { }
        public virtual System.Threading.Tasks.Task OnTargetViewUnloadedAsync(object sender, System.EventArgs e) { }
        protected virtual void OnViewLoadedManagerLoaded(object sender, Catel.MVVM.Views.ViewLoadEventArgs e) { }
        public void OnViewLoadedManagerLoadedInternal(object sender, Catel.MVVM.Views.ViewLoadEventArgs e) { }
        protected virtual void OnViewLoadedManagerLoading(object sender, Catel.MVVM.Views.ViewLoadEventArgs e) { }
        public void OnViewLoadedManagerLoadingInternal(object sender, Catel.MVVM.Views.ViewLoadEventArgs e) { }
        protected virtual void OnViewLoadedManagerUnloaded(object sender, Catel.MVVM.Views.ViewLoadEventArgs e) { }
        public void OnViewLoadedManagerUnloadedInternal(object sender, Catel.MVVM.Views.ViewLoadEventArgs e) { }
        protected virtual void OnViewLoadedManagerUnloading(object sender, Catel.MVVM.Views.ViewLoadEventArgs e) { }
        public void OnViewLoadedManagerUnloadingInternal(object sender, Catel.MVVM.Views.ViewLoadEventArgs e) { }
        public virtual System.Threading.Tasks.Task OnViewModelCanceledAsync(object sender, System.EventArgs e) { }
        protected virtual void OnViewModelChanged() { }
        protected virtual void OnViewModelChanging() { }
        public virtual System.Threading.Tasks.Task OnViewModelClosedAsync(object sender, Catel.MVVM.ViewModelClosedEventArgs e) { }
        public virtual void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) { }
        public virtual System.Threading.Tasks.Task OnViewModelSavedAsync(object sender, System.EventArgs e) { }
        public System.Threading.Tasks.Task<bool> SaveAndCloseViewModelAsync() { }
        public virtual System.Threading.Tasks.Task<bool> SaveViewModelAsync() { }
        protected abstract void SetDataContext(object newDataContext);
        public virtual void ValidateViewModel() { }
    }
    public static class LogicExtensions
    {
        public static TValue GetValue<TLogic, TValue>(this Catel.MVVM.Providers.LogicBase logic, System.Func<TLogic, TValue> function)
            where TLogic : Catel.MVVM.Providers.LogicBase { }
        public static TValue GetValue<TLogic, TValue>(this Catel.MVVM.Providers.LogicBase logic, System.Func<TLogic, TValue> function, TValue defaultValue)
            where TLogic : Catel.MVVM.Providers.LogicBase { }
        public static void SetValue<TLogic>(this Catel.MVVM.Providers.LogicBase logic, System.Action<TLogic> action)
            where TLogic : Catel.MVVM.Providers.LogicBase { }
    }
    public enum LogicViewModelBehavior
    {
        Injected = 0,
        Dynamic = 1,
    }
    public abstract class NavigationLogicBase<T> : Catel.MVVM.Providers.LogicBase
        where T :  class, Catel.MVVM.Views.IView
    {
        protected NavigationLogicBase(T targetPage, System.Type viewModelType = null) { }
        public T TargetPage { get; }
        protected void EnsureViewModel() { }
        protected virtual void OnNavigatedAwayFromPage(Catel.MVVM.Navigation.NavigatedEventArgs e) { }
        protected virtual void OnNavigatedToPage(Catel.MVVM.Navigation.NavigatedEventArgs e) { }
        protected virtual void OnNavigatingAwayFromPage(Catel.MVVM.Navigation.NavigatingEventArgs e) { }
        public override System.Threading.Tasks.Task OnTargetViewLoadedAsync(object sender, System.EventArgs e) { }
        public override System.Threading.Tasks.Task OnTargetViewUnloadedAsync(object sender, System.EventArgs e) { }
    }
    public class PageLogic : Catel.MVVM.Providers.NavigationLogicBase<Catel.MVVM.Views.IPage>
    {
        public PageLogic(Catel.MVVM.Views.IPage targetPage, System.Type viewModelType = null) { }
        protected override bool CanViewBeLoaded { get; }
        protected override void OnViewModelChanged() { }
        protected override void SetDataContext(object newDataContext) { }
    }
    public enum UnloadBehavior
    {
        CloseViewModel = 0,
        SaveAndCloseViewModel = 1,
        CancelAndCloseViewModel = 2,
    }
    public class UserControlLogic : Catel.MVVM.Providers.LogicBase
    {
        public UserControlLogic(Catel.MVVM.Views.IView targetView, System.Type viewModelType = null, Catel.MVVM.IViewModel viewModel = null) { }
        public bool CreateWarningAndErrorValidatorForViewModel { get; set; }
        public bool DisableWhenNoViewModel { get; set; }
        protected bool HasParentViewModelContainer { get; }
        protected bool IsSubscribedToParentViewModel { get; }
        public bool SkipSearchingForInfoBarMessageControl { get; set; }
        public bool SupportParentViewModelContainers { get; set; }
        public Catel.MVVM.Providers.UnloadBehavior UnloadBehavior { get; set; }
        public static bool DefaultCreateWarningAndErrorValidatorForViewModelValue { get; set; }
        public static bool DefaultSkipSearchingForInfoBarMessageControlValue { get; set; }
        public static bool DefaultSupportParentViewModelContainersValue { get; set; }
        public static Catel.MVVM.Providers.UnloadBehavior DefaultUnloadBehaviorValue { get; set; }
        public Catel.MVVM.Views.IViewModelWrapper CreateViewModelWrapper(bool force = false) { }
        public object GetViewModelWrapper() { }
        public override void OnTargetViewDataContextChanged(object sender, Catel.MVVM.Views.DataContextChangedEventArgs e) { }
        public override System.Threading.Tasks.Task OnTargetViewLoadedAsync(object sender, System.EventArgs e) { }
        public override System.Threading.Tasks.Task OnTargetViewUnloadedAsync(object sender, System.EventArgs e) { }
        protected override void OnViewLoadedManagerLoading(object sender, Catel.MVVM.Views.ViewLoadEventArgs e) { }
        protected override void OnViewLoadedManagerUnloading(object sender, Catel.MVVM.Views.ViewLoadEventArgs e) { }
        protected override void OnViewModelChanged() { }
        protected override void OnViewModelChanging() { }
        protected override void SetDataContext(object newDataContext) { }
    }
    public class WindowLogic : Catel.MVVM.Providers.LogicBase
    {
        public WindowLogic(Catel.MVVM.Views.IView targetWindow, System.Type viewModelType = null, Catel.MVVM.IViewModel viewModel = null) { }
        public bool ForceCloseAfterSettingDialogResult { get; set; }
        public override System.Threading.Tasks.Task OnTargetViewUnloadedAsync(object sender, System.EventArgs e) { }
        public void OnTargetWindowClosed(object sender, System.EventArgs e) { }
        public override System.Threading.Tasks.Task OnViewModelClosedAsync(object sender, Catel.MVVM.ViewModelClosedEventArgs e) { }
        protected override void SetDataContext(object newDataContext) { }
    }
}
namespace Catel.MVVM.Views
{
    public class DataContextChangedEventArgs : System.EventArgs
    {
        public DataContextChangedEventArgs(object oldContext, object newContext) { }
        public bool AreEqual { get; }
        public object NewContext { get; }
        public object OldContext { get; }
    }
    public class FastViewPropertySelector : Catel.MVVM.Views.ViewPropertySelector
    {
        public FastViewPropertySelector() { }
        public override bool MustSubscribeToAllViewProperties(System.Type targetViewType) { }
    }
    public interface IDataWindow : Catel.MVVM.IViewModelContainer, Catel.MVVM.Views.IView, System.ComponentModel.INotifyPropertyChanged { }
    public interface INavigationView : Catel.MVVM.IViewModelContainer, Catel.MVVM.Views.IView, System.ComponentModel.INotifyPropertyChanged { }
    public interface IPage : Catel.MVVM.IViewModelContainer, Catel.MVVM.Views.INavigationView, Catel.MVVM.Views.IView, System.ComponentModel.INotifyPropertyChanged { }
    public interface IUserControl : Catel.MVVM.IViewModelContainer, Catel.MVVM.Views.IView, System.ComponentModel.INotifyPropertyChanged
    {
        bool DisableWhenNoViewModel { get; set; }
        System.Windows.DependencyObject Parent { get; }
        bool SkipSearchingForInfoBarMessageControl { get; set; }
        bool SupportParentViewModelContainers { get; set; }
        Catel.MVVM.ViewModelLifetimeManagement ViewModelLifetimeManagement { get; set; }
    }
    public interface IView : Catel.MVVM.IViewModelContainer, System.ComponentModel.INotifyPropertyChanged
    {
        object DataContext { get; set; }
        bool IsEnabled { get; set; }
        object Tag { get; set; }
        event System.EventHandler<Catel.MVVM.Views.DataContextChangedEventArgs> DataContextChanged;
        event System.EventHandler<System.EventArgs> Loaded;
        event System.EventHandler<System.EventArgs> Unloaded;
    }
    public interface IViewLoadManager
    {
        event System.EventHandler<Catel.MVVM.Views.ViewLoadEventArgs> ViewLoaded;
        event System.EventHandler<Catel.MVVM.Views.ViewLoadEventArgs> ViewLoading;
        event System.EventHandler<Catel.MVVM.Views.ViewLoadEventArgs> ViewUnloaded;
        event System.EventHandler<Catel.MVVM.Views.ViewLoadEventArgs> ViewUnloading;
        void AddView(Catel.MVVM.Views.IViewLoadState viewLoadState);
        void CleanUp();
    }
    public interface IViewLoadState
    {
        Catel.MVVM.Views.IView View { get; }
        event System.EventHandler<System.EventArgs> Loaded;
        event System.EventHandler<System.EventArgs> Unloaded;
    }
    public interface IViewManager
    {
        System.Collections.Generic.IEnumerable<Catel.MVVM.Views.IView> ActiveViews { get; }
        Catel.MVVM.Views.IView GetFirstOrDefaultInstance(System.Type viewType);
        Catel.MVVM.Views.IView[] GetViewsOfViewModel(Catel.MVVM.IViewModel viewModel);
        void RegisterView(Catel.MVVM.Views.IView view);
        void UnregisterView(Catel.MVVM.Views.IView view);
    }
    public static class IViewManagerExtensions
    {
        public static TView GetFirstOrDefaultInstance<TView>(this Catel.MVVM.IViewModelManager viewManager)
            where TView : Catel.MVVM.Views.IView { }
    }
    public interface IViewModelWrapper { }
    public interface IViewPropertySelector
    {
        void AddPropertyToSubscribe(string propertyName, System.Type targetViewType);
        System.Collections.Generic.List<string> GetViewPropertiesToSubscribeTo(System.Type targetViewType);
        bool MustSubscribeToAllViewProperties(System.Type targetViewType);
    }
    public static class ViewExtensions
    {
        public static void AutoDetectViewPropertiesToSubscribe(this System.Type viewType) { }
        public static void Dispatch(this Catel.MVVM.Views.IView view, System.Action action) { }
        public static void EnsureVisualTree(this Catel.MVVM.Views.IView view) { }
        public static System.Windows.DependencyObject FindParentByPredicate(this Catel.MVVM.Views.IView view, System.Predicate<object> predicate) { }
        public static System.Windows.DependencyObject FindParentByPredicate(this System.Windows.FrameworkElement view, System.Predicate<object> predicate, int maxDepth) { }
        public static Catel.MVVM.IViewModelContainer FindParentViewModelContainer(this Catel.MVVM.Views.IView view) { }
        public static object GetParent(this Catel.MVVM.Views.IView view) { }
        public static System.Windows.FrameworkElement GetParent(this System.Windows.FrameworkElement element) { }
        public static System.Windows.FrameworkElement[] GetPossibleParents(this System.Windows.FrameworkElement element) { }
        public static string[] GetProperties(this Catel.MVVM.Views.IView view) { }
        public static void SubscribeToPropertyChanged(this Catel.MVVM.Views.IView view, string propertyName, System.EventHandler<System.ComponentModel.PropertyChangedEventArgs> handler) { }
    }
    public class ViewLoadEventArgs : System.EventArgs
    {
        public ViewLoadEventArgs(Catel.MVVM.Views.IView view) { }
        public Catel.MVVM.Views.IView View { get; }
    }
    public class ViewLoadManager : Catel.MVVM.Views.IViewLoadManager
    {
        public ViewLoadManager() { }
        public event System.EventHandler<Catel.MVVM.Views.ViewLoadEventArgs> ViewLoaded;
        public event System.EventHandler<Catel.MVVM.Views.ViewLoadEventArgs> ViewLoading;
        public event System.EventHandler<Catel.MVVM.Views.ViewLoadEventArgs> ViewUnloaded;
        public event System.EventHandler<Catel.MVVM.Views.ViewLoadEventArgs> ViewUnloading;
        public void AddView(Catel.MVVM.Views.IViewLoadState viewLoadState) { }
        public void CleanUp() { }
        protected void InvokeViewLoadEvent(Catel.MVVM.Views.IView view, Catel.MVVM.Views.ViewLoadStateEvent viewLoadStateEvent) { }
    }
    public enum ViewLoadStateEvent
    {
        Loading = 0,
        Loaded = 1,
        Unloading = 2,
        Unloaded = 3,
    }
    public class ViewManager : Catel.MVVM.Views.IViewManager
    {
        public ViewManager() { }
        public System.Collections.Generic.IEnumerable<Catel.MVVM.Views.IView> ActiveViews { get; }
        public Catel.MVVM.Views.IView GetFirstOrDefaultInstance(System.Type viewType) { }
        public virtual Catel.MVVM.Views.IView[] GetViewsOfViewModel(Catel.MVVM.IViewModel viewModel) { }
        public virtual void RegisterView(Catel.MVVM.Views.IView view) { }
        public virtual void UnregisterView(Catel.MVVM.Views.IView view) { }
    }
    public class ViewModelWrapper : Catel.MVVM.Views.IViewModelWrapper
    {
        public ViewModelWrapper(object contentToWrap) { }
        public void UpdateViewModel(Catel.MVVM.IViewModel viewModel) { }
    }
    public class ViewPropertySelector : Catel.MVVM.Views.IViewPropertySelector
    {
        public ViewPropertySelector() { }
        public void AddPropertyToSubscribe(string propertyName, System.Type targetViewType) { }
        public virtual System.Collections.Generic.List<string> GetViewPropertiesToSubscribeTo(System.Type targetViewType) { }
        public virtual bool MustSubscribeToAllViewProperties(System.Type targetViewType) { }
    }
    public class ViewStack
    {
        public ViewStack(Catel.MVVM.Views.IView view, bool isViewLoaded) { }
        public bool IsOutdated { get; }
        public bool IsViewStackLoaded { get; }
        public event System.EventHandler<Catel.MVVM.Views.ViewStackPartEventArgs> ViewLoaded;
        public event System.EventHandler<Catel.MVVM.Views.ViewStackPartEventArgs> ViewStackLoaded;
        public event System.EventHandler<Catel.MVVM.Views.ViewStackPartEventArgs> ViewStackUnloaded;
        public event System.EventHandler<Catel.MVVM.Views.ViewStackPartEventArgs> ViewUnloaded;
        public bool AddChild(Catel.MVVM.Views.IView view, Catel.MVVM.Views.ViewStack parentViewStack) { }
        public bool AddChild(Catel.MVVM.Views.ViewStack viewStack, Catel.MVVM.Views.ViewStack parentViewStack) { }
        public void CheckForOutdatedChildren() { }
        public bool ContainsView(Catel.MVVM.Views.IView view) { }
        public void Dispose() { }
        public void MarkAsLoaded() { }
        public void MarkAsUnloaded() { }
        public void NotifyThatParentIsReadyToAcceptLoadedMessages() { }
    }
    public class ViewStackPartEventArgs : System.EventArgs
    {
        public ViewStackPartEventArgs(Catel.MVVM.Views.IView view) { }
        public Catel.MVVM.Views.IView View { get; }
    }
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.All, AllowMultiple=false, Inherited=false)]
    public class ViewToViewModelAttribute : System.Attribute
    {
        public ViewToViewModelAttribute(string viewModelPropertyName = "") { }
        public Catel.MVVM.Views.ViewToViewModelMappingType MappingType { get; set; }
        public string ViewModelPropertyName { get; }
    }
    public enum ViewToViewModelMappingType
    {
        TwoWayDoNothing = 0,
        TwoWayViewWins = 1,
        TwoWayViewModelWins = 2,
        ViewToViewModel = 3,
        ViewModelToView = 4,
    }
    public class WeakViewInfo
    {
        public WeakViewInfo(Catel.MVVM.Views.IView view, bool isViewLoaded = false) { }
        public WeakViewInfo(Catel.MVVM.Views.IViewLoadState viewLoadState, bool isViewLoaded = false) { }
        public bool IsAlive { get; }
        public bool IsLoaded { get; }
        public Catel.MVVM.Views.IView View { get; }
        public event System.EventHandler<System.EventArgs> Loaded;
        public event System.EventHandler<System.EventArgs> Unloaded;
        public void OnViewLoadStateLoaded(object sender, System.EventArgs e) { }
        public void OnViewLoadStateUnloaded(object sender, System.EventArgs e) { }
        public void OnViewLoaded(object sender, System.EventArgs e) { }
        public void OnViewUnloaded(object sender, System.EventArgs e) { }
    }
}
namespace Catel.Services
{
    public class ApplicationClosingEventArgs : System.EventArgs
    {
        public ApplicationClosingEventArgs() { }
        public bool Cancel { get; set; }
    }
    public class AutoCompletionService : Catel.Services.IAutoCompletionService
    {
        public AutoCompletionService(Catel.Data.IObjectAdapter objectAdapter) { }
        public virtual string[] GetAutoCompleteValues(string property, string filter, System.Collections.IEnumerable source) { }
    }
    public class BusyIndicatorService : Catel.Services.IBusyIndicatorService
    {
        public BusyIndicatorService(Catel.Services.ILanguageService languageService, Catel.Services.IDispatcherService dispatcherService) { }
        public int ShowCounter { get; }
        public void Hide() { }
        public void Pop() { }
        public void Push(string status = "") { }
        public void Show(string status = "") { }
        public void Show(Catel.Services.BusyIndicatorWorkAsyncDelegate workDelegate, string status = "") { }
        public void Show(Catel.Services.BusyIndicatorWorkDelegate workDelegate, string status = "") { }
        public void UpdateStatus(string status) { }
        public void UpdateStatus(int currentItem, int totalItems, string statusFormat = "") { }
    }
    public delegate System.Threading.Tasks.Task BusyIndicatorWorkAsyncDelegate();
    public delegate void BusyIndicatorWorkDelegate();
    public class ContentReadyEventArgs : System.EventArgs
    {
        public ContentReadyEventArgs(System.IO.Stream imageStream) { }
        public System.IO.Stream ImageStream { get; }
    }
    public class DetermineDirectoryContext
    {
        public DetermineDirectoryContext() { }
        public string DirectoryName { get; set; }
        public string Filter { get; set; }
        public string InitialDirectory { get; set; }
        public bool ShowNewFolderButton { get; set; }
        public string Title { get; set; }
    }
    public class DetermineDirectoryResult
    {
        public DetermineDirectoryResult() { }
        public string DirectoryName { get; set; }
        public bool Result { get; set; }
    }
    public abstract class DetermineFileContext
    {
        protected DetermineFileContext() { }
        public bool AddExtension { get; set; }
        public bool CheckFileExists { get; set; }
        public bool CheckPathExists { get; set; }
        public string FileName { get; set; }
        public string Filter { get; set; }
        public int FilterIndex { get; set; }
        public string InitialDirectory { get; set; }
        public string Title { get; set; }
        public bool ValidateNames { get; set; }
    }
    public abstract class DetermineFileResult
    {
        protected DetermineFileResult() { }
        public string FileName { get; set; }
        public bool Result { get; set; }
    }
    public class DetermineOpenFileContext : Catel.Services.DetermineFileContext
    {
        public DetermineOpenFileContext() { }
        public bool IsMultiSelect { get; set; }
    }
    public class DetermineOpenFileResult : Catel.Services.DetermineFileResult
    {
        public DetermineOpenFileResult() { }
        public string[] FileNames { get; set; }
    }
    public class DetermineSaveFileContext : Catel.Services.DetermineFileContext
    {
        public DetermineSaveFileContext() { }
    }
    public class DetermineSaveFileResult : Catel.Services.DetermineFileResult
    {
        public DetermineSaveFileResult() { }
    }
    public class DispatcherProviderService : Catel.Services.IDispatcherProviderService
    {
        public DispatcherProviderService() { }
        public virtual object GetApplicationDispatcher() { }
        public virtual object GetCurrentDispatcher() { }
    }
    public class DispatcherService : Catel.Services.IDispatcherService
    {
        public DispatcherService(Catel.Services.IDispatcherProviderService dispatcherProviderService) { }
        protected virtual System.Windows.Threading.Dispatcher CurrentDispatcher { get; }
        public virtual void BeginInvoke(System.Action action, bool onlyBeginInvokeWhenNoAccess = true) { }
        public virtual void Invoke(System.Action action, bool onlyInvokeWhenNoAccess = true) { }
        public virtual System.Threading.Tasks.Task InvokeAsync(System.Action action) { }
        public virtual System.Threading.Tasks.Task InvokeAsync(System.Delegate method, params object[] args) { }
        public virtual System.Threading.Tasks.Task<T> InvokeAsync<T>(System.Func<T> func) { }
        public virtual System.Threading.Tasks.Task InvokeTaskAsync(System.Func<System.Threading.Tasks.Task> actionAsync) { }
        public virtual System.Threading.Tasks.Task InvokeTaskAsync(System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task> actionAsync, System.Threading.CancellationToken cancellationToken) { }
        public virtual System.Threading.Tasks.Task<T> InvokeTaskAsync<T>(System.Func<System.Threading.Tasks.Task<T>> funcAsync) { }
        public virtual System.Threading.Tasks.Task<T> InvokeTaskAsync<T>(System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<T>> funcAsync, System.Threading.CancellationToken cancellationToken) { }
    }
    public abstract class FileServiceBase : Catel.Services.ViewModelServiceBase, Catel.Services.IFileSupport
    {
        protected FileServiceBase() { }
        protected virtual void ConfigureFileDialog(Microsoft.Win32.FileDialog fileDialog, Catel.Services.DetermineFileContext context) { }
        protected virtual string GetInitialDirectory(Catel.Services.DetermineFileContext context) { }
    }
    public interface IAutoCompletionService
    {
        string[] GetAutoCompleteValues(string property, string filter, System.Collections.IEnumerable source);
    }
    public interface IBusyIndicatorService
    {
        int ShowCounter { get; }
        void Hide();
        void Pop();
        void Push(string status = "");
        void Show(string status = "");
        void Show(Catel.Services.BusyIndicatorWorkAsyncDelegate workDelegate, string status = "");
        void Show(Catel.Services.BusyIndicatorWorkDelegate workDelegate, string status = "");
        void UpdateStatus(string status);
        void UpdateStatus(int currentItem, int totalItems, string statusFormat = "");
    }
    public static class IBusyIndicatorServiceExtensions
    {
        public static System.IDisposable HideTemporarily(this Catel.Services.IBusyIndicatorService busyIndicatorService) { }
        public static System.IDisposable PushInScope(this Catel.Services.IBusyIndicatorService busyIndicatorService, string status = "") { }
    }
    public interface IDispatcherProviderService
    {
        object GetApplicationDispatcher();
        object GetCurrentDispatcher();
    }
    public interface IFileSupport { }
    public interface IMessageService
    {
        System.Threading.Tasks.Task<Catel.Services.MessageResult> ShowAsync(string message, string caption = "", Catel.Services.MessageButton button = 1, Catel.Services.MessageImage icon = 0);
        System.Threading.Tasks.Task<Catel.Services.MessageResult> ShowErrorAsync(System.Exception exception);
        System.Threading.Tasks.Task<Catel.Services.MessageResult> ShowErrorAsync(string message, string caption = "");
        System.Threading.Tasks.Task<Catel.Services.MessageResult> ShowInformationAsync(string message, string caption = "");
        System.Threading.Tasks.Task<Catel.Services.MessageResult> ShowWarningAsync(string message, string caption = "");
    }
    public interface INavigationRootService
    {
        object GetNavigationRoot();
    }
    public interface INavigationService
    {
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        event System.EventHandler<System.EventArgs> ApplicationClosed;
        event System.EventHandler<Catel.Services.ApplicationClosingEventArgs> ApplicationClosing;
        System.Threading.Tasks.Task<bool> CloseApplicationAsync();
        int GetBackStackCount();
        System.Threading.Tasks.Task GoBackAsync();
        System.Threading.Tasks.Task GoForwardAsync();
        System.Threading.Tasks.Task NavigateAsync(System.Uri uri);
        System.Threading.Tasks.Task NavigateAsync(string uri, System.Collections.Generic.Dictionary<string, object> parameters = null);
        System.Threading.Tasks.Task NavigateAsync(System.Type viewModelType, System.Collections.Generic.Dictionary<string, object> parameters = null);
        void Register(string name, System.Uri uri);
        void Register(System.Type viewModelType, System.Uri uri);
        void RemoveAllBackEntries();
        void RemoveBackEntry();
        bool Unregister(string name);
        bool Unregister(System.Type viewModelType);
    }
    public static class INavigationServiceExtensions
    {
        public static System.Threading.Tasks.Task NavigateAsync<TViewModel>(Catel.Services.INavigationService navigationService, System.Collections.Generic.Dictionary<string, object> parameters = null) { }
    }
    public interface IOpenFileService : Catel.Services.IFileSupport
    {
        System.Threading.Tasks.Task<Catel.Services.DetermineOpenFileResult> DetermineFileAsync(Catel.Services.DetermineOpenFileContext context);
    }
    public interface IProcessService
    {
        System.Threading.Tasks.Task<Catel.Services.ProcessResult> RunAsync(Catel.Services.ProcessContext processContext);
        void StartProcess(Catel.Services.ProcessContext processContext, Catel.Services.ProcessCompletedDelegate processCompletedCallback = null);
        void StartProcess(string fileName, string arguments = "", Catel.Services.ProcessCompletedDelegate processCompletedCallback = null);
    }
    public interface ISaveFileService : Catel.Services.IFileSupport
    {
        System.Threading.Tasks.Task<Catel.Services.DetermineSaveFileResult> DetermineFileAsync(Catel.Services.DetermineSaveFileContext context);
    }
    public interface ISelectDirectoryService
    {
        System.Threading.Tasks.Task<Catel.Services.DetermineDirectoryResult> DetermineDirectoryAsync(Catel.Services.DetermineDirectoryContext context);
    }
    public interface IState { }
    public interface IStateService
    {
        Catel.Services.IState LoadState(string key);
        void StoreState(string key, Catel.Services.IState state);
    }
    public static class IStateServiceExtensions
    {
        public static TState LoadState<TState>(this Catel.Services.IStateService stateService, string key)
            where TState :  class, Catel.Services.IState { }
    }
    public interface IUIVisualizerService
    {
        bool IsRegistered(string name);
        void Register(string name, System.Type windowType, bool throwExceptionIfExists = true);
        System.Threading.Tasks.Task<Catel.Services.UIVisualizerResult> ShowAsync(Catel.MVVM.IViewModel viewModel, System.EventHandler<Catel.Services.UICompletedEventArgs> completedProc = null);
        System.Threading.Tasks.Task<Catel.Services.UIVisualizerResult> ShowAsync(string name, object data, System.EventHandler<Catel.Services.UICompletedEventArgs> completedProc = null);
        System.Threading.Tasks.Task<Catel.Services.UIVisualizerResult> ShowDialogAsync(Catel.MVVM.IViewModel viewModel, System.EventHandler<Catel.Services.UICompletedEventArgs> completedProc = null);
        System.Threading.Tasks.Task<Catel.Services.UIVisualizerResult> ShowDialogAsync(string name, object data, System.EventHandler<Catel.Services.UICompletedEventArgs> completedProc = null);
        bool Unregister(string name);
    }
    public static class IUIVisualizerServiceExtensions
    {
        public static bool? ActivateWindow(System.Windows.Window window) { }
        public static bool IsRegistered(this Catel.Services.IUIVisualizerService uiVisualizerService, System.Type viewModelType) { }
        public static bool IsRegistered<TViewModel>(this Catel.Services.IUIVisualizerService uiVisualizerService) { }
        public static void Register(this Catel.Services.IUIVisualizerService uiVisualizerService, System.Type viewModelType, System.Type windowType, bool throwExceptionIfExists = true) { }
        public static void Register<TViewModel, TView>(this Catel.Services.IUIVisualizerService uiVisualizerService, bool throwExceptionIfExists = true) { }
        public static System.Threading.Tasks.Task<Catel.Services.UIVisualizerResult> ShowAsync<TViewModel>(this Catel.Services.IUIVisualizerService uiVisualizerService, object model = null, System.EventHandler<Catel.Services.UICompletedEventArgs> completedProc = null)
            where TViewModel : Catel.MVVM.IViewModel { }
        public static System.Threading.Tasks.Task<Catel.Services.UIVisualizerResult> ShowDialogAsync<TViewModel>(this Catel.Services.IUIVisualizerService uiVisualizerService, object model = null, System.EventHandler<Catel.Services.UICompletedEventArgs> completedProc = null)
            where TViewModel : Catel.MVVM.IViewModel { }
        public static System.Threading.Tasks.Task ShowOrActivateAsync<TViewModel>(this Catel.Services.IUIVisualizerService uiVisualizerService, object dataContext = null, object scope = null)
            where TViewModel : Catel.MVVM.IViewModel { }
        public static System.Threading.Tasks.Task<bool?> ShowOrActivateAsync<TViewModel>(this Catel.Services.IUIVisualizerService uiVisualizerService, object model = null, object scope = null, System.EventHandler<Catel.Services.UICompletedEventArgs> completedProc = null)
            where TViewModel : Catel.MVVM.IViewModel { }
        public static bool Unregister(this Catel.Services.IUIVisualizerService uiVisualizerService, System.Type viewModelType) { }
        public static bool Unregister<TViewModel>(this Catel.Services.IUIVisualizerService uiVisualizerService) { }
    }
    public interface IViewContextService
    {
        object GetContext(Catel.MVVM.Views.IView view);
    }
    public interface IViewModelService : Catel.Services.IService { }
    public interface IViewModelWrapperService
    {
        Catel.MVVM.Views.IViewModelWrapper GetWrapper(Catel.MVVM.Views.IView view);
        bool IsWrapped(Catel.MVVM.Views.IView view);
        Catel.MVVM.Views.IViewModelWrapper Wrap(Catel.MVVM.Views.IView view, object viewModelSource, Catel.Services.WrapOptions wrapOptions);
    }
    public interface IWrapControlService
    {
        bool CanBeWrapped(System.Windows.FrameworkElement frameworkElement);
        System.Windows.FrameworkElement GetWrappedElement(System.Windows.Controls.Grid wrappedGrid, Catel.Services.WrapControlServiceWrapOptions wrapOption);
        System.Windows.FrameworkElement GetWrappedElement(System.Windows.Controls.Grid wrappedGrid, string controlName);
        T GetWrappedElement<T>(System.Windows.Controls.Grid wrappedGrid, Catel.Services.WrapControlServiceWrapOptions wrapOption)
            where T : System.Windows.FrameworkElement;
        T GetWrappedElement<T>(System.Windows.Controls.Grid wrappedGrid, string controlName)
            where T : System.Windows.FrameworkElement;
        System.Windows.Controls.Grid Wrap(System.Windows.FrameworkElement frameworkElement, Catel.Services.WrapControlServiceWrapOptions wrapOptions, System.Windows.Controls.ContentControl parentContentControl = null);
        System.Windows.Controls.Grid Wrap(System.Windows.FrameworkElement frameworkElement, Catel.Services.WrapControlServiceWrapOptions wrapOptions, Catel.Windows.DataWindowButton[] buttons, System.Windows.Controls.ContentControl parentContentControl);
    }
    [System.Flags]
    public enum MessageButton
    {
        OK = 1,
        OKCancel = 2,
        YesNo = 4,
        YesNoCancel = 8,
    }
    public enum MessageImage
    {
        None = 0,
        Information = 1,
        Question = 2,
        Exclamation = 3,
        Error = 4,
        Stop = 5,
        Warning = 6,
    }
    public enum MessageResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 3,
        No = 4,
    }
    public class MessageService : Catel.Services.ViewModelServiceBase, Catel.Services.IMessageService
    {
        public MessageService(Catel.Services.IDispatcherService dispatcherService, Catel.Services.ILanguageService languageService) { }
        public virtual System.Threading.Tasks.Task<Catel.Services.MessageResult> ShowAsync(string message, string caption = "", Catel.Services.MessageButton button = 1, Catel.Services.MessageImage icon = 0) { }
        public virtual System.Threading.Tasks.Task<Catel.Services.MessageResult> ShowErrorAsync(System.Exception exception) { }
        public virtual System.Threading.Tasks.Task<Catel.Services.MessageResult> ShowErrorAsync(string message, string caption = "") { }
        public virtual System.Threading.Tasks.Task<Catel.Services.MessageResult> ShowInformationAsync(string message, string caption = "") { }
        protected virtual System.Threading.Tasks.Task<Catel.Services.MessageResult> ShowMessageBoxAsync(string message, string caption = "", Catel.Services.MessageButton button = 1, Catel.Services.MessageImage icon = 0) { }
        public virtual System.Threading.Tasks.Task<Catel.Services.MessageResult> ShowWarningAsync(string message, string caption = "") { }
        protected static Catel.Services.MessageResult TranslateMessageBoxResult(System.Windows.MessageBoxResult result) { }
        protected static System.Windows.MessageBoxButton TranslateMessageButton(Catel.Services.MessageButton button) { }
        protected static System.Windows.MessageBoxImage TranslateMessageImage(Catel.Services.MessageImage image) { }
    }
    public static class NamingConvention
    {
        public const string Assembly = "[AS]";
        public const string Current = "[CURRENT]";
        public const string Up = "[UP]";
        public const string ViewModelName = "[VM]";
        public const string ViewName = "[VW]";
        public static string GetParentPath(string path) { }
        public static string GetParentPath(string path, string separator) { }
        public static string GetParentSeparator(string path) { }
        public static string ResolveNamingConvention(System.Collections.Generic.Dictionary<string, string> constantsWithValues, string conventionToUse) { }
        public static string ResolveNamingConvention(System.Collections.Generic.Dictionary<string, string> constantsWithValues, string conventionToUse, string value) { }
        public static string ResolveViewByViewModelName(string assembly, string fullViewModelName, string conventionToUse) { }
        public static string ResolveViewModelByViewName(string assembly, string fullViewName, string conventionToUse) { }
    }
    public class NavigationRootService : Catel.Services.INavigationRootService
    {
        public NavigationRootService() { }
        protected virtual System.Windows.Controls.Frame GetApplicationRootFrame() { }
        public virtual object GetNavigationRoot() { }
    }
    public class NavigationService : Catel.Services.NavigationServiceBase, Catel.Services.INavigationService
    {
        protected readonly Catel.Services.INavigationRootService NavigationRootService;
        public NavigationService(Catel.Services.INavigationRootService navigationRootService) { }
        public override bool CanGoBack { get; }
        public override bool CanGoForward { get; }
        protected System.Windows.Controls.Frame RootFrame { get; }
        public event System.EventHandler<System.EventArgs> ApplicationClosed;
        public event System.EventHandler<Catel.Services.ApplicationClosingEventArgs> ApplicationClosing;
        public System.Threading.Tasks.Task<bool> CloseApplicationAsync() { }
        public override int GetBackStackCount() { }
        public virtual System.Threading.Tasks.Task GoBackAsync() { }
        public virtual System.Threading.Tasks.Task GoForwardAsync() { }
        public virtual System.Threading.Tasks.Task NavigateAsync(System.Uri uri) { }
        public virtual System.Threading.Tasks.Task NavigateAsync(string uri, System.Collections.Generic.Dictionary<string, object> parameters = null) { }
        public virtual System.Threading.Tasks.Task NavigateAsync(System.Type viewModelType, System.Collections.Generic.Dictionary<string, object> parameters = null) { }
        public virtual void Register(string name, System.Uri uri) { }
        public virtual void Register(System.Type viewModelType, System.Uri uri) { }
        public override void RemoveAllBackEntries() { }
        public override void RemoveBackEntry() { }
        protected override string ResolveNavigationTarget(System.Type viewModelType) { }
        public virtual bool Unregister(string name) { }
        public virtual bool Unregister(System.Type viewModelType) { }
    }
    public abstract class NavigationServiceBase : Catel.Services.ViewModelServiceBase
    {
        protected NavigationServiceBase() { }
        public abstract bool CanGoBack { get; }
        public abstract bool CanGoForward { get; }
        public abstract int GetBackStackCount();
        public abstract void RemoveAllBackEntries();
        public abstract void RemoveBackEntry();
        protected abstract string ResolveNavigationTarget(System.Type viewModelType);
    }
    public class OpenFileService : Catel.Services.FileServiceBase, Catel.Services.IFileSupport, Catel.Services.IOpenFileService
    {
        public OpenFileService() { }
        public virtual System.Threading.Tasks.Task<Catel.Services.DetermineOpenFileResult> DetermineFileAsync(Catel.Services.DetermineOpenFileContext context) { }
    }
    public class PageNotRegisteredException : System.Exception
    {
        public PageNotRegisteredException(string name) { }
        public string Name { get; }
    }
    public delegate void ProcessCompletedDelegate(Catel.Services.ProcessContext processorContext, int exitCode);
    public class ProcessContext
    {
        public ProcessContext() { }
        public string Arguments { get; set; }
        public string FileName { get; set; }
        public bool UseShellExecute { get; set; }
        public string Verb { get; set; }
        public string WorkingDirectory { get; set; }
    }
    public class ProcessResult
    {
        public ProcessResult(Catel.Services.ProcessContext context) { }
        public Catel.Services.ProcessContext Context { get; }
        public int ExitCode { get; set; }
    }
    public class ProcessService : Catel.Services.IProcessService
    {
        public ProcessService() { }
        public virtual System.Threading.Tasks.Task<Catel.Services.ProcessResult> RunAsync(Catel.Services.ProcessContext processContext) { }
        public virtual void StartProcess(Catel.Services.ProcessContext processContext, Catel.Services.ProcessCompletedDelegate processCompletedCallback = null) { }
        public virtual void StartProcess(string fileName, string arguments = "", Catel.Services.ProcessCompletedDelegate processCompletedCallback = null) { }
    }
    public class SaveFileService : Catel.Services.FileServiceBase, Catel.Services.IFileSupport, Catel.Services.ISaveFileService
    {
        public SaveFileService() { }
        public virtual System.Threading.Tasks.Task<Catel.Services.DetermineSaveFileResult> DetermineFileAsync(Catel.Services.DetermineSaveFileContext context) { }
    }
    public class SelectDirectoryService : Catel.Services.ViewModelServiceBase, Catel.Services.ISelectDirectoryService
    {
        public SelectDirectoryService() { }
        public virtual System.Threading.Tasks.Task<Catel.Services.DetermineDirectoryResult> DetermineDirectoryAsync(Catel.Services.DetermineDirectoryContext context) { }
    }
    public struct Size : System.IEquatable<Catel.Services.Size>
    {
        public Size(double width, double height) { }
        public double Height { get; set; }
        public double Width { get; set; }
        public bool Equals(Catel.Services.Size size) { }
        public override bool Equals(object obj) { }
        public override int GetHashCode() { }
        public override string ToString() { }
        public static bool operator !=(Catel.Services.Size a, Catel.Services.Size b) { }
        public static bool operator ==(Catel.Services.Size a, Catel.Services.Size b) { }
    }
    public class StateService : Catel.Services.IStateService
    {
        public StateService() { }
        public Catel.Services.IState LoadState(string key) { }
        public void StoreState(string key, Catel.Services.IState state) { }
    }
    public class UICompletedEventArgs : System.EventArgs
    {
        public UICompletedEventArgs(Catel.Services.UIVisualizerResult result) { }
        public object DataContext { get; }
        public bool? DialogResult { get; }
        public Catel.Services.UIVisualizerResult Result { get; }
    }
    public class UIVisualizerResult
    {
        public UIVisualizerResult(bool? result, object data, object dataContext, object window) { }
        public object Data { get; }
        public object DataContext { get; }
        public bool? DialogResult { get; }
        public object Window { get; }
        public TViewModel GetViewModel<TViewModel>()
            where TViewModel :  class, Catel.MVVM.IViewModel { }
    }
    public class UIVisualizerService : Catel.Services.ViewModelServiceBase, Catel.Services.IUIVisualizerService
    {
        protected readonly System.Collections.Generic.Dictionary<string, System.Type> RegisteredWindows;
        public UIVisualizerService(Catel.MVVM.IViewLocator viewLocator, Catel.Services.IDispatcherService dispatcherService) { }
        protected virtual System.Threading.Tasks.Task<System.Windows.FrameworkElement> CreateWindowAsync(string name, object data, bool isModal, System.Action<Catel.Services.UIVisualizerResult> completedProc) { }
        protected virtual System.Threading.Tasks.Task<System.Windows.FrameworkElement> CreateWindowAsync(System.Type windowType, object data, bool isModal, System.Action<Catel.Services.UIVisualizerResult> completedProc) { }
        protected virtual void EnsureViewIsRegistered(string name) { }
        protected virtual System.Windows.FrameworkElement GetActiveWindow() { }
        protected virtual void HandleCloseSubscription(object window, object data, bool isModal, System.Action<Catel.Services.UIVisualizerResult> completedProc) { }
        public virtual bool IsRegistered(string name) { }
        public virtual void Register(string name, System.Type windowType, bool throwExceptionIfExists = true) { }
        protected virtual void RegisterViewForViewModelIfRequired(System.Type viewModelType) { }
        public virtual System.Threading.Tasks.Task<Catel.Services.UIVisualizerResult> ShowAsync(Catel.MVVM.IViewModel viewModel, System.EventHandler<Catel.Services.UICompletedEventArgs> completedProc = null) { }
        public virtual System.Threading.Tasks.Task<Catel.Services.UIVisualizerResult> ShowAsync(string name, object data, System.EventHandler<Catel.Services.UICompletedEventArgs> completedProc = null) { }
        public virtual System.Threading.Tasks.Task<Catel.Services.UIVisualizerResult> ShowDialogAsync(Catel.MVVM.IViewModel viewModel, System.EventHandler<Catel.Services.UICompletedEventArgs> completedProc = null) { }
        public virtual System.Threading.Tasks.Task<Catel.Services.UIVisualizerResult> ShowDialogAsync(string name, object data, System.EventHandler<Catel.Services.UICompletedEventArgs> completedProc = null) { }
        protected virtual System.Threading.Tasks.Task<Catel.Services.UIVisualizerResult> ShowWindowAsync(System.Windows.FrameworkElement window, object data, bool showModal) { }
        public virtual bool Unregister(string name) { }
    }
    public class ViewContextService : Catel.Services.IViewContextService
    {
        public ViewContextService() { }
        public object GetContext(Catel.MVVM.Views.IView view) { }
    }
    public abstract class ViewModelServiceBase : Catel.Services.ServiceBase, Catel.Services.IService, Catel.Services.IViewModelService
    {
        protected ViewModelServiceBase() { }
    }
    public class ViewModelWrapperService : Catel.Services.ViewModelWrapperServiceBase, Catel.Services.IViewModelWrapperService
    {
        public ViewModelWrapperService() { }
        public Catel.MVVM.Views.IViewModelWrapper GetWrapper(Catel.MVVM.Views.IView view) { }
        protected override bool IsViewWrapped(Catel.MVVM.Views.IView view) { }
        public bool IsWrapped(Catel.MVVM.Views.IView view) { }
        public Catel.MVVM.Views.IViewModelWrapper Wrap(Catel.MVVM.Views.IView view, object viewModelSource, Catel.Services.WrapOptions wrapOptions) { }
    }
    public abstract class ViewModelWrapperServiceBase
    {
        protected ViewModelWrapperServiceBase() { }
        protected abstract bool IsViewWrapped(Catel.MVVM.Views.IView view);
    }
    public class WindowNotRegisteredException : System.Exception
    {
        public WindowNotRegisteredException(string name) { }
        public string Name { get; }
    }
    public class WrapControlService : Catel.Services.IWrapControlService
    {
        public WrapControlService() { }
        public bool CanBeWrapped(System.Windows.FrameworkElement frameworkElement) { }
        public System.Windows.FrameworkElement GetWrappedElement(System.Windows.Controls.Grid wrappedGrid, Catel.Services.WrapControlServiceWrapOptions wrapOption) { }
        public System.Windows.FrameworkElement GetWrappedElement(System.Windows.Controls.Grid wrappedGrid, string controlName) { }
        public T GetWrappedElement<T>(System.Windows.Controls.Grid wrappedGrid, Catel.Services.WrapControlServiceWrapOptions wrapOption)
            where T : System.Windows.FrameworkElement { }
        public T GetWrappedElement<T>(System.Windows.Controls.Grid wrappedGrid, string controlName)
            where T : System.Windows.FrameworkElement { }
        public System.Windows.Controls.Grid Wrap(System.Windows.FrameworkElement frameworkElement, Catel.Services.WrapControlServiceWrapOptions wrapOptions, System.Windows.Controls.ContentControl parentContentControl = null) { }
        public System.Windows.Controls.Grid Wrap(System.Windows.FrameworkElement frameworkElement, Catel.Services.WrapControlServiceWrapOptions wrapOptions, Catel.Windows.DataWindowButton[] buttons, System.Windows.Controls.ContentControl parentContentControl) { }
    }
    public static class WrapControlServiceControlNames
    {
        public const string ButtonsWrapPanelName = "_ButtonsWrapPanel";
        public const string DefaultCancelButtonName = "cancelButton";
        public const string DefaultOkButtonName = "okButton";
        public const string InfoBarMessageControlName = "_InfoBarMessageControl";
        public const string InternalGridName = "_InternalGridName";
        public const string MainContentHolderName = "_MainContentHolder";
        public const string WarningAndErrorValidatorName = "_WarningAndErrorValidator";
    }
    [System.Flags]
    public enum WrapControlServiceWrapOptions
    {
        GenerateInlineInfoBarMessageControl = 1,
        GenerateOverlayInfoBarMessageControl = 2,
        GenerateWarningAndErrorValidatorForDataContext = 4,
        ExplicitlyAddApplicationResourcesDictionary = 8,
        GenerateAdornerDecorator = 16,
        All = 29,
    }
    [System.Flags]
    public enum WrapOptions
    {
        None = 0,
        CreateWarningAndErrorValidatorForViewModel = 1,
        Force = 2,
    }
}
namespace Catel.Windows
{
    public static class ApplicationExtensions
    {
        public static System.Windows.Window GetActiveWindow(this System.Windows.Application application) { }
    }
    public static class DataContextChangedHelper
    {
        public static void AddDataContextChangedHandler(this System.Windows.FrameworkElement element, System.EventHandler<Catel.Windows.Data.DependencyPropertyValueChangedEventArgs> handler) { }
        public static void RemoveDataContextChangedHandler(this System.Windows.FrameworkElement element, System.EventHandler<Catel.Windows.Data.DependencyPropertyValueChangedEventArgs> handler) { }
    }
    public class DataWindow : System.Windows.Window, Catel.MVVM.IViewModelContainer, Catel.MVVM.Views.IDataWindow, Catel.MVVM.Views.IView, System.ComponentModel.INotifyPropertyChanged
    {
        public DataWindow() { }
        public DataWindow(Catel.MVVM.IViewModel viewModel) { }
        public DataWindow(Catel.Windows.DataWindowMode mode, System.Collections.Generic.IEnumerable<Catel.Windows.DataWindowButton> additionalButtons = null, Catel.Windows.DataWindowDefaultButton defaultButton = 0, bool setOwnerAndFocus = true, Catel.Windows.InfoBarMessageControlGenerationMode infoBarMessageControlGenerationMode = 1, bool focusFirstControl = true) { }
        public DataWindow(Catel.MVVM.IViewModel viewModel, Catel.Windows.DataWindowMode mode, System.Collections.Generic.IEnumerable<Catel.Windows.DataWindowButton> additionalButtons = null, Catel.Windows.DataWindowDefaultButton defaultButton = 0, bool setOwnerAndFocus = true, Catel.Windows.InfoBarMessageControlGenerationMode infoBarMessageControlGenerationMode = 1, bool focusFirstControl = true) { }
        protected bool CanClose { get; set; }
        public bool CanCloseUsingEscape { get; set; }
        protected System.Collections.ObjectModel.ReadOnlyCollection<System.Windows.Input.ICommand> Commands { get; }
        protected Catel.Windows.DataWindowDefaultButton DefaultButton { get; }
        protected Catel.Windows.DataWindowMode Mode { get; }
        public Catel.MVVM.IViewModel ViewModel { get; }
        public Catel.MVVM.ViewModelLifetimeManagement ViewModelLifetimeManagement { get; set; }
        public System.Type ViewModelType { get; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public event System.EventHandler<System.EventArgs> ViewModelChanged;
        public event System.EventHandler<System.ComponentModel.PropertyChangedEventArgs> ViewModelPropertyChanged;
        protected void AddCustomButton(Catel.Windows.DataWindowButton dataWindowButton) { }
        protected virtual System.Threading.Tasks.Task<bool> ApplyChangesAsync() { }
        protected virtual System.Threading.Tasks.Task<bool> DiscardChangesAsync() { }
        protected System.Threading.Tasks.Task ExecuteApplyAsync() { }
        protected System.Threading.Tasks.Task ExecuteCancelAsync() { }
        protected void ExecuteClose() { }
        protected System.Threading.Tasks.Task ExecuteOkAsync() { }
        protected virtual void Initialize() { }
        protected bool OnApplyCanExecute() { }
        protected System.Threading.Tasks.Task OnApplyExecuteAsync() { }
        protected bool OnCancelCanExecute() { }
        protected System.Threading.Tasks.Task OnCancelExecuteAsync() { }
        protected bool OnCloseCanExecute() { }
        protected void OnCloseExecute() { }
        protected override void OnInitialized(System.EventArgs e) { }
        protected virtual void OnInternalGridChanged() { }
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e) { }
        protected virtual void OnLoaded(System.EventArgs e) { }
        protected bool OnOkCanExecute() { }
        protected System.Threading.Tasks.Task OnOkExecuteAsync() { }
        protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) { }
        protected virtual void OnUnloaded(System.EventArgs e) { }
        protected virtual void OnViewModelChanged() { }
        protected virtual System.Threading.Tasks.Task OnViewModelClosedAsync(object sender, Catel.MVVM.ViewModelClosedEventArgs e) { }
        protected virtual void OnViewModelPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) { }
        protected void RaiseCanExecuteChangedForAllCommands() { }
        protected virtual void RaisePropertyChanged(string propertyName) { }
        protected virtual bool ValidateData() { }
    }
    public class DataWindowButton
    {
        public DataWindowButton(string text, string commandBindingPath) { }
        public DataWindowButton(string text, System.Windows.Input.ICommand command) { }
        public DataWindowButton(string text, string commandBindingPath, string contentBindingPath = null, System.Windows.Data.IValueConverter contentValueConverter = null, string visibilityBindingPath = null, System.Windows.Data.IValueConverter visibilityValueConverter = null) { }
        public DataWindowButton(string text, System.Windows.Input.ICommand command, string contentBindingPath = null, System.Windows.Data.IValueConverter contentValueConverter = null, string visibilityBindingPath = null, System.Windows.Data.IValueConverter visibilityValueConverter = null) { }
        public System.Windows.Input.ICommand Command { get; }
        public string CommandBindingPath { get; }
        public string ContentBindingPath { get; }
        public System.Windows.Data.IValueConverter ContentValueConverter { get; }
        public bool IsCancel { get; set; }
        public bool IsDefault { get; set; }
        public string Text { get; }
        public string VisibilityBindingPath { get; }
        public System.Windows.Data.IValueConverter VisibilityValueConverter { get; }
        public static Catel.Windows.DataWindowButton FromAsync(string text, System.Func<System.Threading.Tasks.Task> executeAsync, System.Func<bool> canExecute = null) { }
        public static Catel.Windows.DataWindowButton FromAsync(string text, System.Func<System.Threading.Tasks.Task> executeAsync, System.Func<bool> canExecute = null, string contentBindingPath = null, System.Windows.Data.IValueConverter contentValueConverter = null, string visibilityBindingPath = null, System.Windows.Data.IValueConverter visibilityValueConverter = null) { }
        public static Catel.Windows.DataWindowButton FromSync(string text, System.Action execute, System.Func<bool> canExecute = null) { }
        public static Catel.Windows.DataWindowButton FromSync(string text, System.Action execute, System.Func<bool> canExecute = null, string contentBindingPath = null, System.Windows.Data.IValueConverter contentValueConverter = null, string visibilityBindingPath = null, System.Windows.Data.IValueConverter visibilityValueConverter = null) { }
    }
    public enum DataWindowDefaultButton
    {
        OK = 0,
        Apply = 1,
        Close = 2,
        None = 3,
    }
    public enum DataWindowMode
    {
        OkCancel = 0,
        OkCancelApply = 1,
        Close = 2,
        Custom = 3,
    }
    public static class DependencyObjectExtensions
    {
        public static object FindLogicalAncestor(this System.Windows.DependencyObject startElement, System.Predicate<object> condition, int maxDepth = -1) { }
        public static T FindLogicalAncestorByType<T>(this System.Windows.DependencyObject startElement) { }
        public static T FindLogicalAncestorByType<T>(this System.Windows.DependencyObject startElement, int maxDepth) { }
        public static object FindLogicalOrVisualAncestor(this System.Windows.DependencyObject startElement, System.Predicate<object> condition, int maxDepth = -1) { }
        public static T FindLogicalOrVisualAncestorByType<T>(this System.Windows.DependencyObject startElement) { }
        public static T FindLogicalOrVisualAncestorByType<T>(this System.Windows.DependencyObject startElement, int maxDepth) { }
        public static System.Windows.DependencyObject FindLogicalRoot(this System.Windows.DependencyObject startElement) { }
        public static object FindVisualAncestor(this System.Windows.DependencyObject startElement, System.Predicate<object> condition, int maxDepth = -1) { }
        public static T FindVisualAncestorByType<T>(this System.Windows.DependencyObject startElement) { }
        public static T FindVisualAncestorByType<T>(this System.Windows.DependencyObject startElement, int maxDepth) { }
        public static System.Windows.DependencyObject FindVisualDescendant(this System.Windows.DependencyObject startElement, System.Predicate<object> condition) { }
        public static System.Windows.DependencyObject FindVisualDescendantByName(this System.Windows.DependencyObject startElement, string name) { }
        public static T FindVisualDescendantByType<T>(this System.Windows.DependencyObject startElement)
            where T : System.Windows.DependencyObject { }
        public static object FindVisualRoot(this System.Windows.DependencyObject startElement) { }
        public static System.Collections.Generic.IEnumerable<System.Windows.DependencyObject> GetChildren(this System.Windows.DependencyObject parent) { }
        public static System.Windows.DependencyObject GetLogicalParent(this System.Windows.DependencyObject element) { }
        public static System.Windows.DependencyObject GetVisualParent(this System.Windows.DependencyObject element) { }
        public static bool IsElementWithName(this System.Windows.DependencyObject dependencyObject, string name) { }
    }
    public static class FrameworkElementExtensions
    {
        public static void FixBlurriness(this System.Windows.FrameworkElement element) { }
        public static void HideValidationAdorner(this System.Windows.FrameworkElement frameworkElement) { }
        public static bool IsVisible(this System.Windows.FrameworkElement element) { }
        public static bool IsVisibleToUser(this System.Windows.FrameworkElement element) { }
        public static bool IsVisibleToUser(this System.Windows.FrameworkElement element, System.Windows.FrameworkElement container) { }
    }
    public enum InfoBarMessageControlGenerationMode
    {
        None = 0,
        Inline = 1,
        Overlay = 2,
    }
    public static class PopupHelper
    {
        public static System.Collections.Generic.IEnumerable<System.Windows.Controls.Primitives.Popup> Popups { get; }
        public static System.Collections.Generic.IEnumerable<System.Windows.Controls.Primitives.Popup> GetAllPopups() { }
    }
    public static class ResourceHelper
    {
        public static void EnsurePackUriIsAllowed() { }
        public static string GetResourceUri(string resourceUri, string shortAssemblyName = null) { }
        public static bool XamlPageExists(string uriString) { }
        public static bool XamlPageExists(System.Uri uri) { }
    }
    public static class UIElementExtensions
    {
        public static void FocusFirstControl(this System.Windows.ContentElement element, bool focusParentsFirst = true) { }
        public static void FocusFirstControl(this System.Windows.UIElement element, bool focusParentsFirst = true) { }
        public static System.Windows.UIElement GetFocusedControl(this System.Windows.UIElement element) { }
        public static void MoveFocus(this System.Windows.ContentElement element, System.Windows.Input.FocusNavigationDirection direction, int hops) { }
        public static void MoveFocus(this System.Windows.IInputElement element, System.Windows.Input.FocusNavigationDirection direction, int hops) { }
        public static void MoveFocus(this System.Windows.UIElement element, System.Windows.Input.FocusNavigationDirection direction, int hops) { }
    }
    public class Window : Catel.Windows.DataWindow
    {
        public Window() { }
    }
    public static class WindowExtensions
    {
        public static void ApplyIconFromApplication(this System.Windows.Window window) { }
        public static void BringWindowToTop(this System.Windows.Window window) { }
        public static bool CanSetDialogResult(this System.Windows.Window window) { }
        public static System.IntPtr GetWindowHandle(this System.Windows.Window window) { }
        public static void RemoveIcon(this System.Windows.Window window) { }
        public static void SetOwnerWindow(this System.Windows.Window window, bool forceNewOwner = false, bool focusFirstControl = false) { }
        public static void SetOwnerWindow(this System.Windows.Window window, System.IntPtr owner, bool forceNewOwner = false) { }
        public static void SetOwnerWindow(this System.Windows.Window window, System.Windows.Window owner, bool forceNewOwner = false) { }
        public static void SetOwnerWindowAndFocus(this System.Windows.Window window, bool forceNewOwner = false, bool focusFirstControl = true) { }
        public static void SetOwnerWindowAndFocus(this System.Windows.Window window, System.IntPtr owner, bool forceNewOwner = false) { }
        public static void SetOwnerWindowAndFocus(this System.Windows.Window window, System.Windows.Window owner, bool forceNewOwner = false) { }
    }
}
namespace Catel.Windows.Controls
{
    [System.Windows.TemplatePart(Name="PART_MessageBar", Type=typeof(System.Windows.FrameworkElement))]
    public class InfoBarMessageControl : System.Windows.Controls.ContentControl
    {
        public static readonly System.Windows.DependencyProperty InfoMessageProperty;
        public static readonly System.Windows.DependencyProperty MessageCountProperty;
        public static readonly System.Windows.DependencyProperty ModeProperty;
        public static readonly System.Windows.DependencyProperty TextProperty;
        public InfoBarMessageControl() { }
        public System.Collections.ObjectModel.ObservableCollection<string> ErrorMessageCollection { get; }
        public string InfoMessage { get; set; }
        public int MessageCount { get; }
        public Catel.Windows.Controls.InfoBarMessageControlMode Mode { get; set; }
        public string Text { get; set; }
        public System.Collections.ObjectModel.ObservableCollection<string> WarningMessageCollection { get; }
        public static string DefaultTextPropertyValue { get; set; }
        public override void OnApplyTemplate() { }
        public void SubscribeWarningAndErrorValidator(Catel.Windows.Controls.WarningAndErrorValidator validator) { }
        public void UnsubscribeWarningAndErrorValidator(Catel.Windows.Controls.WarningAndErrorValidator validator) { }
    }
    public enum InfoBarMessageControlMode
    {
        Inline = 0,
        Overlay = 1,
    }
    [System.Windows.Data.ValueConversion(typeof(Catel.Windows.Controls.InfoBarMessageControlMode), typeof(object), ParameterType=typeof(Catel.Windows.Controls.InfoBarMessageControlMode))]
    public class InfoBarMessageControlVisibilityConverter : Catel.MVVM.Converters.IValueConverter, System.Windows.Data.IValueConverter
    {
        public InfoBarMessageControlVisibilityConverter() { }
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
    }
    public class Page : System.Windows.Controls.Page, Catel.MVVM.IViewModelContainer, Catel.MVVM.Views.INavigationView, Catel.MVVM.Views.IPage, Catel.MVVM.Views.IView, System.ComponentModel.INotifyPropertyChanged
    {
        public Page() { }
        public Catel.MVVM.IViewModel ViewModel { get; }
        public Catel.MVVM.ViewModelLifetimeManagement ViewModelLifetimeManagement { get; set; }
        public System.Type ViewModelType { get; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public event System.EventHandler<System.EventArgs> ViewModelChanged;
        public event System.EventHandler<System.ComponentModel.PropertyChangedEventArgs> ViewModelPropertyChanged;
        protected virtual void OnLoaded(System.EventArgs e) { }
        protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) { }
        protected virtual void OnUnloaded(System.EventArgs e) { }
        protected virtual void OnViewModelChanged() { }
        protected virtual void OnViewModelPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) { }
        protected virtual void RaisePropertyChanged(string propertyName) { }
    }
    public class UserControl : System.Windows.Controls.UserControl, Catel.MVVM.IViewModelContainer, Catel.MVVM.Views.IUserControl, Catel.MVVM.Views.IView, System.ComponentModel.INotifyPropertyChanged
    {
        public UserControl() { }
        public UserControl(Catel.MVVM.IViewModel viewModel) { }
        public bool CreateWarningAndErrorValidatorForViewModel { get; set; }
        public bool DisableWhenNoViewModel { get; set; }
        public bool SkipSearchingForInfoBarMessageControl { get; set; }
        public bool SupportParentViewModelContainers { get; set; }
        public Catel.MVVM.Providers.UnloadBehavior UnloadBehavior { get; set; }
        public Catel.MVVM.IViewModel ViewModel { get; }
        public Catel.MVVM.ViewModelLifetimeManagement ViewModelLifetimeManagement { get; set; }
        public System.Type ViewModelType { get; }
        public static bool DefaultCreateWarningAndErrorValidatorForViewModelValue { get; set; }
        public static bool DefaultSkipSearchingForInfoBarMessageControlValue { get; set; }
        public static bool DefaultSupportParentViewModelContainersValue { get; set; }
        public static Catel.MVVM.Providers.UnloadBehavior DefaultUnloadBehaviorValue { get; set; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public event System.EventHandler<System.EventArgs> ViewModelChanged;
        public event System.EventHandler<System.ComponentModel.PropertyChangedEventArgs> ViewModelPropertyChanged;
        protected override void AddChild(object value) { }
        protected virtual void OnLoaded(System.EventArgs e) { }
        protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) { }
        protected virtual void OnUnloaded(System.EventArgs e) { }
        protected virtual void OnViewModelChanged() { }
        protected virtual System.Threading.Tasks.Task OnViewModelClosedAsync(object sender, Catel.MVVM.ViewModelClosedEventArgs e) { }
        protected virtual void OnViewModelPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) { }
        protected virtual void RaisePropertyChanged(string propertyName) { }
    }
    public enum ValidationEventAction
    {
        Added = 0,
        Removed = 1,
        Refresh = 2,
        ClearAll = 3,
    }
    public class ValidationEventArgs : System.EventArgs
    {
        public Catel.Windows.Controls.ValidationEventAction Action { get; }
        public string Message { get; }
        public Catel.Windows.Controls.ValidationType Type { get; }
        public object Value { get; }
    }
    public enum ValidationType
    {
        Warning = 0,
        Error = 1,
    }
    public class WarningAndErrorValidator : System.Windows.Controls.Control, Catel.IUniqueIdentifyable
    {
        public static readonly System.Windows.DependencyProperty AutomaticallyRegisterToInfoBarMessageControlProperty;
        public static readonly System.Windows.DependencyProperty SourceProperty;
        public WarningAndErrorValidator() { }
        public bool AutomaticallyRegisterToInfoBarMessageControl { get; set; }
        public object Source { get; set; }
        public int UniqueIdentifier { get; }
        public event System.EventHandler<Catel.Windows.Controls.ValidationEventArgs> Validation;
    }
}
namespace Catel.Windows.Data
{
    public static class BindingHelper
    {
        public static void ClearBinding(System.Windows.DependencyObject dependencyObject, System.Windows.DependencyProperty dependencyProperty) { }
        public static object GetBindingValue(System.Windows.FrameworkElement frameworkElement, System.Windows.Data.BindingBase binding) { }
    }
    public class BindingWithValidation : System.Windows.Data.Binding
    {
        public BindingWithValidation() { }
        public BindingWithValidation(string path) { }
    }
    public static class DependencyPropertyChangedHelper
    {
        public static bool IsRealDependencyProperty(this System.Windows.FrameworkElement frameworkElement, string propertyName) { }
        public static void SubscribeToAllDependencyProperties(this System.Windows.FrameworkElement frameworkElement, System.EventHandler<Catel.Windows.Data.DependencyPropertyValueChangedEventArgs> handler) { }
        public static void SubscribeToDataContext(this System.Windows.FrameworkElement frameworkElement, System.EventHandler<Catel.Windows.Data.DependencyPropertyValueChangedEventArgs> handler, bool inherited) { }
        public static void SubscribeToDependencyProperty(this System.Windows.FrameworkElement frameworkElement, string propertyName, System.EventHandler<Catel.Windows.Data.DependencyPropertyValueChangedEventArgs> handler) { }
        public static void UnsubscribeFromAllDependencyProperties(this System.Windows.FrameworkElement frameworkElement, System.EventHandler<Catel.Windows.Data.DependencyPropertyValueChangedEventArgs> handler) { }
        public static void UnsubscribeFromDataContext(this System.Windows.FrameworkElement frameworkElement, System.EventHandler<Catel.Windows.Data.DependencyPropertyValueChangedEventArgs> handler, bool inherited) { }
        public static void UnsubscribeFromDependencyProperty(this System.Windows.FrameworkElement frameworkElement, string propertyName, System.EventHandler<Catel.Windows.Data.DependencyPropertyValueChangedEventArgs> handler) { }
    }
    public static class DependencyPropertyHelper
    {
        public static System.Collections.Generic.List<Catel.Windows.Data.DependencyPropertyInfo> GetDependencyProperties(System.Type viewType) { }
        public static System.Collections.Generic.List<Catel.Windows.Data.DependencyPropertyInfo> GetDependencyProperties(this System.Windows.FrameworkElement frameworkElement) { }
        public static System.Windows.DependencyProperty GetDependencyPropertyByName(this System.Windows.FrameworkElement frameworkElement, string propertyName) { }
        public static string GetDependencyPropertyCacheKey(System.Type viewType, string propertyName) { }
        public static string GetDependencyPropertyCacheKeyPrefix(System.Type viewType) { }
        public static string GetDependencyPropertyName(this System.Windows.FrameworkElement frameworkElement, System.Windows.DependencyProperty dependencyProperty) { }
    }
    [System.Diagnostics.DebuggerDisplay("{PropertyName}")]
    public class DependencyPropertyInfo
    {
        public DependencyPropertyInfo(System.Windows.DependencyProperty dependencyProperty, string propertyName) { }
        public System.Windows.DependencyProperty DependencyProperty { get; }
        public string PropertyName { get; }
    }
    public class DependencyPropertyValueChangedEventArgs : System.EventArgs
    {
        public DependencyPropertyValueChangedEventArgs(string propertyName, System.Windows.DependencyProperty dependencyProperty, object oldValue, object newValue) { }
        public System.Windows.DependencyProperty DependencyProperty { get; }
        public System.Windows.DependencyPropertyChangedEventArgs FxEventArgs { get; }
        public object NewValue { get; }
        public object OldValue { get; }
        public string PropertyName { get; }
    }
}
namespace Catel.Windows.Input
{
    public class InputGesture : Catel.Data.ModelBase
    {
        public static readonly Catel.Data.IPropertyData KeyProperty;
        public static readonly Catel.Data.IPropertyData ModifiersProperty;
        public InputGesture() { }
        public InputGesture(System.Windows.Input.Key key) { }
        public InputGesture(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifiers) { }
        public System.Windows.Input.Key Key { get; set; }
        public System.Windows.Input.ModifierKeys Modifiers { get; set; }
        protected bool Equals(Catel.Windows.Input.InputGesture other) { }
        public override bool Equals(object obj) { }
        public override int GetHashCode() { }
        public bool Matches(System.Windows.Input.KeyEventArgs eventArgs) { }
        public override string ToString() { }
    }
    public static class InputGestureExtensions
    {
        public static bool IsEmpty(this Catel.Windows.Input.InputGesture inputGesture) { }
    }
    public static class KeyboardHelper
    {
        public static bool AreKeyboardModifiersPressed(System.Windows.Input.ModifierKeys modifier, bool checkForExactModifiers = true) { }
        public static System.Collections.Generic.List<System.Windows.Input.ModifierKeys> GetCurrentlyPressedModifiers() { }
    }
}
namespace Catel.Windows.Interactivity
{
    public class Authentication : Catel.Windows.Interactivity.BehaviorBase<System.Windows.FrameworkElement>
    {
        public static readonly System.Windows.DependencyProperty ActionProperty;
        public static readonly System.Windows.DependencyProperty AuthenticationTagProperty;
        public Authentication() { }
        public Catel.Windows.Interactivity.AuthenticationAction Action { get; set; }
        public object AuthenticationTag { get; set; }
        protected override void OnAssociatedObjectLoaded() { }
    }
    public enum AuthenticationAction
    {
        Hide = 0,
        Collapse = 1,
        Disable = 2,
    }
    public class AutoCompletion : Catel.Windows.Interactivity.BehaviorBase<System.Windows.Controls.TextBox>
    {
        public static readonly System.Windows.DependencyProperty ItemsSourceProperty;
        public static readonly System.Windows.DependencyProperty PropertyNameProperty;
        public static readonly System.Windows.DependencyProperty UseAutoCompletionServiceProperty;
        public AutoCompletion() { }
        public System.Collections.IEnumerable ItemsSource { get; set; }
        public string PropertyName { get; set; }
        public bool UseAutoCompletionService { get; set; }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
        protected override void OnIsEnabledChanged() { }
    }
    public class AutoScroll : Catel.Windows.Interactivity.BehaviorBase<System.Windows.Controls.ItemsControl>
    {
        public static readonly System.Windows.DependencyProperty ScrollDirectionProperty;
        public static readonly System.Windows.DependencyProperty ScrollOnLoadedProperty;
        public static readonly System.Windows.DependencyProperty ScrollTresholdProperty;
        public AutoScroll() { }
        public Catel.Windows.Interactivity.ScrollDirection ScrollDirection { get; set; }
        public bool ScrollOnLoaded { get; set; }
        public int ScrollTreshold { get; set; }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
    }
    public abstract class BehaviorBase<T> : Microsoft.Xaml.Behaviors.Behavior<T>, Catel.Windows.Interactivity.IBehavior
        where T : System.Windows.FrameworkElement
    {
        public static readonly System.Windows.DependencyProperty IsEnabledProperty;
        protected BehaviorBase() { }
        protected System.Globalization.CultureInfo Culture { get; }
        public bool IsAssociatedObjectLoaded { get; }
        public bool IsEnabled { get; set; }
        protected bool IsInDesignMode { get; }
        protected virtual void Initialize() { }
        protected virtual void OnAssociatedObjectLoaded() { }
        protected virtual void OnAssociatedObjectUnloaded() { }
        protected override void OnAttached() { }
        protected override void OnDetaching() { }
        protected virtual void OnIsEnabledChanged() { }
        protected virtual void Uninitialize() { }
        protected virtual void ValidateRequiredProperties() { }
    }
    public class BehaviorEventArgs : System.EventArgs
    {
        public BehaviorEventArgs(Catel.Windows.Interactivity.IBehavior behavior) { }
        public Catel.Windows.Interactivity.IBehavior Behavior { get; }
    }
    public abstract class CommandBehaviorBase<T> : Catel.Windows.Interactivity.BehaviorBase<T>
        where T : System.Windows.FrameworkElement
    {
        public static readonly System.Windows.DependencyProperty CommandParameterProperty;
        public static readonly System.Windows.DependencyProperty CommandProperty;
        public static readonly System.Windows.DependencyProperty ModifiersProperty;
        protected CommandBehaviorBase() { }
        public System.Windows.Input.ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public System.Windows.Input.ModifierKeys Modifiers { get; set; }
        protected virtual bool CanExecuteCommand() { }
        protected virtual bool CanExecuteCommand(object parameter) { }
        protected virtual void ExecuteCommand() { }
        protected virtual void ExecuteCommand(object parameter) { }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
        protected virtual void OnCommandCanExecuteChanged() { }
        protected virtual void OnCommandChanged() { }
        protected virtual void OnCommandParameterChanged() { }
    }
    public abstract class CommandEventTriggerBase<T> : Catel.Windows.Interactivity.EventTriggerBase<T>
        where T : System.Windows.FrameworkElement
    {
        public static readonly System.Windows.DependencyProperty CommandParameterProperty;
        public static readonly System.Windows.DependencyProperty CommandProperty;
        public static readonly System.Windows.DependencyProperty ModifiersProperty;
        protected CommandEventTriggerBase() { }
        public System.Windows.Input.ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public System.Windows.Input.ModifierKeys Modifiers { get; set; }
        protected virtual bool CanExecuteCommand() { }
        protected virtual bool CanExecuteCommand(object parameter) { }
        protected virtual void ExecuteCommand() { }
        protected virtual void ExecuteCommand(object parameter) { }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
        protected virtual void OnCommandCanExecuteChanged() { }
        protected virtual void OnCommandChanged() { }
        protected virtual void OnCommandParameterChanged() { }
    }
    public abstract class CommandTriggerActionBase<T> : Catel.Windows.Interactivity.TriggerActionBase<T>
        where T : System.Windows.FrameworkElement
    {
        public static readonly System.Windows.DependencyProperty CommandParameterProperty;
        public static readonly System.Windows.DependencyProperty CommandProperty;
        public static readonly System.Windows.DependencyProperty ModifiersProperty;
        protected CommandTriggerActionBase() { }
        public System.Windows.Input.ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public System.Windows.Input.ModifierKeys Modifiers { get; set; }
        protected virtual bool CanExecuteCommand() { }
        protected virtual bool CanExecuteCommand(object parameter) { }
        protected virtual void ExecuteCommand() { }
        protected virtual void ExecuteCommand(object parameter) { }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
        protected virtual void OnCommandCanExecuteChanged() { }
        protected virtual void OnCommandChanged() { }
        protected virtual void OnCommandParameterChanged() { }
    }
    public abstract class CommandTriggerBase<T> : Catel.Windows.Interactivity.TriggerBase<T>
        where T : System.Windows.FrameworkElement
    {
        public static readonly System.Windows.DependencyProperty CommandParameterProperty;
        public static readonly System.Windows.DependencyProperty CommandProperty;
        public static readonly System.Windows.DependencyProperty ModifiersProperty;
        protected CommandTriggerBase() { }
        public System.Windows.Input.ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public System.Windows.Input.ModifierKeys Modifiers { get; set; }
        protected virtual bool CanExecuteCommand() { }
        protected virtual bool CanExecuteCommand(object parameter) { }
        protected virtual void ExecuteCommand() { }
        protected virtual void ExecuteCommand(object parameter) { }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
        protected virtual void OnCommandCanExecuteChanged() { }
        protected virtual void OnCommandChanged() { }
        protected virtual void OnCommandParameterChanged() { }
    }
    public class DelayBindingUpdate : Catel.Windows.Interactivity.BehaviorBase<System.Windows.FrameworkElement>
    {
        public DelayBindingUpdate() { }
        public string DependencyPropertyName { get; set; }
        public string PropertyName { get; set; }
        public int UpdateDelay { get; set; }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
        protected override void ValidateRequiredProperties() { }
    }
    public class DoubleClickToCommand : Catel.Windows.Interactivity.CommandBehaviorBase<System.Windows.FrameworkElement>
    {
        public static readonly System.Windows.DependencyProperty AutoFixListBoxItemTemplateProperty;
        public DoubleClickToCommand() { }
        public DoubleClickToCommand(System.Action action, int doubleClickMilliseconds = 500) { }
        public bool AutoFixListBoxItemTemplate { get; set; }
        protected virtual System.Collections.Generic.IEnumerable<System.Windows.UIElement> GetHitElements(System.Windows.Point mousePosition) { }
        protected virtual bool IsElementHit(System.Windows.Point mousePosition) { }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
    }
    public class EventToCommand : Catel.Windows.Interactivity.CommandTriggerActionBase<System.Windows.FrameworkElement>
    {
        public static readonly System.Windows.DependencyProperty DisableAssociatedObjectOnCannotExecuteProperty;
        public EventToCommand() { }
        public bool DisableAssociatedObjectOnCannotExecute { get; set; }
        public Catel.MVVM.Converters.IEventArgsConverter EventArgsConverter { get; set; }
        public bool PassEventArgsToCommand { get; set; }
        public bool PreventInvocationIfAssociatedObjectIsDisabled { get; set; }
        public void Invoke() { }
        protected override void Invoke(object parameter) { }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnCommandCanExecuteChanged() { }
    }
    public abstract class EventTriggerBase<T> : Microsoft.Xaml.Behaviors.EventTriggerBase<T>, Catel.Windows.Interactivity.ITrigger
        where T : System.Windows.FrameworkElement
    {
        public static readonly System.Windows.DependencyProperty IsEnabledProperty;
        protected EventTriggerBase() { }
        protected T AssociatedObject { get; }
        public bool IsAssociatedObjectLoaded { get; }
        public bool IsEnabled { get; set; }
        protected bool IsInDesignMode { get; }
        protected override string GetEventName() { }
        protected virtual void Initialize() { }
        protected virtual void OnAssociatedObjectLoaded() { }
        protected virtual void OnAssociatedObjectUnloaded() { }
        protected override sealed void OnAttached() { }
        protected override sealed void OnDetaching() { }
        protected virtual void OnIsEnabledChanged() { }
        protected virtual void Uninitialize() { }
        protected virtual void ValidateRequiredProperties() { }
    }
    public class Focus : Catel.Windows.Interactivity.FocusBehaviorBase
    {
        public static readonly System.Windows.DependencyProperty EventNameProperty;
        public static readonly System.Windows.DependencyProperty FocusMomentProperty;
        public static readonly System.Windows.DependencyProperty PropertyNameProperty;
        public static readonly System.Windows.DependencyProperty SourceProperty;
        public Focus() { }
        public string EventName { get; set; }
        public Catel.Windows.Interactivity.FocusMoment FocusMoment { get; set; }
        public string PropertyName { get; set; }
        public object Source { get; set; }
        protected override void OnAssociatedObjectLoaded() { }
    }
    public class FocusBehaviorBase : Catel.Windows.Interactivity.BehaviorBase<System.Windows.FrameworkElement>
    {
        public static readonly System.Windows.DependencyProperty FocusDelayProperty;
        public FocusBehaviorBase() { }
        public int FocusDelay { get; set; }
        protected bool IsFocusAlreadySet { get; }
        protected void StartFocus() { }
    }
    public class FocusFirstControl : Catel.Windows.Interactivity.BehaviorBase<System.Windows.FrameworkElement>
    {
        public static readonly System.Windows.DependencyProperty FocusParentsFirstProperty;
        public FocusFirstControl() { }
        public bool FocusParentsFirst { get; set; }
        protected override void OnAssociatedObjectLoaded() { }
    }
    public enum FocusMoment
    {
        Loaded = 0,
        PropertyChanged = 1,
        Event = 2,
    }
    public class FocusOnKeyPress : Catel.Windows.Interactivity.FocusBehaviorBase
    {
        public static readonly System.Windows.DependencyProperty KeyProperty;
        public static readonly System.Windows.DependencyProperty ModifiersProperty;
        public FocusOnKeyPress() { }
        public System.Windows.Input.Key Key { get; set; }
        public System.Windows.Input.ModifierKeys Modifiers { get; set; }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
    }
    public class HideUntilViewModelLoaded : Catel.Windows.Interactivity.BehaviorBase<System.Windows.FrameworkElement>
    {
        public HideUntilViewModelLoaded() { }
        protected override void Initialize() { }
        protected override void Uninitialize() { }
    }
    public interface IBehavior { }
    public interface ITrigger { }
    public class KeyPressToCommand : Catel.Windows.Interactivity.CommandBehaviorBase<System.Windows.FrameworkElement>
    {
        public static readonly System.Windows.DependencyProperty KeyProperty;
        public KeyPressToCommand() { }
        public System.Windows.Input.Key Key { get; set; }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
    }
    public class MouseInfo : Catel.Windows.Interactivity.BehaviorBase<System.Windows.FrameworkElement>
    {
        public static readonly System.Windows.DependencyProperty IsMouseOverProperty;
        public MouseInfo() { }
        public bool IsMouseOver { get; set; }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
    }
    public class Navigate : Microsoft.Xaml.Behaviors.Behavior<System.Windows.Documents.Hyperlink>
    {
        public Navigate() { }
        protected override void OnAttached() { }
        protected override void OnDetaching() { }
    }
    public class NumericTextBox : Catel.Windows.Interactivity.BehaviorBase<System.Windows.Controls.TextBox>
    {
        public static readonly System.Windows.DependencyProperty IsDecimalAllowedProperty;
        public static readonly System.Windows.DependencyProperty IsNegativeAllowedProperty;
        public static readonly System.Windows.DependencyProperty UpdateBindingOnTextChangedProperty;
        public NumericTextBox() { }
        public bool IsDecimalAllowed { get; set; }
        public bool IsNegativeAllowed { get; set; }
        public bool UpdateBindingOnTextChanged { get; set; }
        protected override void Initialize() { }
        protected override void Uninitialize() { }
    }
    public class RoutedEventTrigger : Catel.Windows.Interactivity.EventTriggerBase<System.Windows.FrameworkElement>
    {
        public RoutedEventTrigger() { }
        public System.Windows.RoutedEvent RoutedEvent { get; set; }
        protected override string GetEventName() { }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
        protected override void ValidateRequiredProperties() { }
    }
    public enum ScrollDirection
    {
        Top = 0,
        Bottom = 1,
    }
    public class SelectTextOnFocus : Catel.Windows.Interactivity.BehaviorBase<System.Windows.Controls.Control>
    {
        public SelectTextOnFocus() { }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
    }
    public abstract class TriggerActionBase<T> : Microsoft.Xaml.Behaviors.TriggerAction<T>
        where T : System.Windows.FrameworkElement
    {
        protected TriggerActionBase() { }
        public bool IsAssociatedObjectLoaded { get; }
        protected bool IsInDesignMode { get; }
        protected virtual void Initialize() { }
        protected virtual void OnAssociatedObjectLoaded() { }
        protected virtual void OnAssociatedObjectUnloaded() { }
        protected override void OnAttached() { }
        protected override void OnDetaching() { }
        protected virtual void Uninitialize() { }
        protected virtual void ValidateRequiredProperties() { }
    }
    public abstract class TriggerBase<T> : Microsoft.Xaml.Behaviors.TriggerBase<T>, Catel.Windows.Interactivity.ITrigger
        where T : System.Windows.FrameworkElement
    {
        public static readonly System.Windows.DependencyProperty IsEnabledProperty;
        protected TriggerBase() { }
        public bool IsAssociatedObjectLoaded { get; }
        public bool IsEnabled { get; set; }
        protected bool IsInDesignMode { get; }
        protected virtual void Initialize() { }
        protected virtual void OnAssociatedObjectLoaded() { }
        protected virtual void OnAssociatedObjectUnloaded() { }
        protected override void OnAttached() { }
        protected override void OnDetaching() { }
        protected virtual void OnIsEnabledChanged() { }
        protected virtual void Uninitialize() { }
        protected virtual void ValidateRequiredProperties() { }
    }
    public class TriggerEventArgs : System.EventArgs
    {
        public TriggerEventArgs(Catel.Windows.Interactivity.ITrigger trigger) { }
        public Catel.Windows.Interactivity.ITrigger Trigger { get; }
    }
    public class UpdateBindingBehaviorBase<T> : Catel.Windows.Interactivity.BehaviorBase<T>
        where T : System.Windows.FrameworkElement
    {
        public UpdateBindingBehaviorBase(string dependencyPropertyName) { }
        protected System.Windows.DependencyProperty DependencyProperty { get; }
        protected string DependencyPropertyName { get; }
        protected virtual void UpdateBinding() { }
    }
    public class UpdateBindingOnPasswordChanged : Catel.Windows.Interactivity.BehaviorBase<System.Windows.Controls.PasswordBox>
    {
        public static readonly System.Windows.DependencyProperty PasswordProperty;
        public UpdateBindingOnPasswordChanged() { }
        public string Password { get; set; }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
    }
    public class UpdateBindingOnTextChanged : Catel.Windows.Interactivity.UpdateBindingBehaviorBase<System.Windows.Controls.TextBox>
    {
        public UpdateBindingOnTextChanged() { }
        public int UpdateDelay { get; set; }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
    }
    public class WindowEventToCommand : Catel.Windows.Interactivity.CommandBehaviorBase<System.Windows.FrameworkElement>
    {
        public WindowEventToCommand() { }
        public WindowEventToCommand(System.Action<System.Windows.Window> action) { }
        public string EventName { get; set; }
        protected override void ExecuteCommand() { }
        protected override void ExecuteCommand(object parameter) { }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
        public void OnEventOccurred() { }
        protected void RegisterEventHandler(System.Windows.Window window) { }
        protected void UnregisterEventHandler(System.Windows.Window window) { }
    }
}
namespace Catel.Windows.Markup
{
    public class CommandManagerBinding : Catel.Windows.Markup.UpdatableMarkupExtension
    {
        public CommandManagerBinding() { }
        public CommandManagerBinding(string commandName) { }
        [System.Windows.Markup.ConstructorArgument("commandName")]
        public string CommandName { get; set; }
        protected override void OnTargetObjectLoaded() { }
        protected override void OnTargetObjectUnloaded() { }
        protected override object ProvideDynamicValue(System.IServiceProvider serviceProvider) { }
    }
    public class LanguageBinding : Catel.Windows.Markup.UpdatableMarkupExtension
    {
        public LanguageBinding() { }
        public LanguageBinding(string resourceName) { }
        public System.Globalization.CultureInfo Culture { get; set; }
        public bool HideDesignTimeMessages { get; set; }
        [System.Windows.Markup.ConstructorArgument("resourceName")]
        public string ResourceName { get; set; }
        public void OnLanguageUpdated(object sender, System.EventArgs e) { }
        protected override void OnTargetObjectLoaded() { }
        protected override void OnTargetObjectUnloaded() { }
        protected override object ProvideDynamicValue(System.IServiceProvider serviceProvider) { }
    }
    public class ServiceDependencyExtension : System.Windows.Markup.MarkupExtension
    {
        public ServiceDependencyExtension() { }
        public ServiceDependencyExtension(System.Type type) { }
        public object Tag { get; set; }
        [System.Windows.Markup.ConstructorArgument("type")]
        public System.Type Type { get; set; }
        public override object ProvideValue(System.IServiceProvider serviceProvider) { }
    }
    public abstract class UpdatableMarkupExtension : System.Windows.Markup.MarkupExtension, System.ComponentModel.INotifyPropertyChanged
    {
        protected UpdatableMarkupExtension() { }
        protected bool AllowUpdatableStyleSetters { get; set; }
        protected object TargetObject { get; }
        protected object TargetProperty { get; }
        public object Value { get; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnTargetObjectLoaded() { }
        protected virtual void OnTargetObjectUnloaded() { }
        protected virtual object ProvideDynamicValue(System.IServiceProvider serviceProvider) { }
        public override sealed object ProvideValue(System.IServiceProvider serviceProvider) { }
        protected void RaisePropertyChanged(string propertyName) { }
        protected void UpdateValue() { }
    }
}
namespace Catel.Windows.Threading
{
    public static class DispatcherExtensions
    {
        public static System.Windows.Threading.DispatcherOperation BeginInvoke(this System.Windows.Threading.Dispatcher dispatcher, System.Action action) { }
        public static System.Windows.Threading.DispatcherOperation BeginInvoke(this System.Windows.Threading.Dispatcher dispatcher, System.Action action, bool onlyBeginInvokeWhenNoAccess) { }
        public static System.Windows.Threading.DispatcherOperation BeginInvoke(this System.Windows.Threading.Dispatcher dispatcher, System.Action action, System.Windows.Threading.DispatcherPriority priority) { }
        public static System.Windows.Threading.DispatcherOperation BeginInvoke(this System.Windows.Threading.Dispatcher dispatcher, System.Delegate method, params object[] args) { }
        public static System.Windows.Threading.DispatcherOperation BeginInvoke(this System.Windows.Threading.Dispatcher dispatcher, System.Action action, System.Windows.Threading.DispatcherPriority priority, bool onlyBeginInvokeWhenNoAccess) { }
        public static System.Windows.Threading.DispatcherOperation BeginInvoke(this System.Windows.Threading.Dispatcher dispatcher, System.Delegate method, System.Windows.Threading.DispatcherPriority priority, params object[] args) { }
        public static System.Threading.Tasks.Task BeginInvokeAsync(this System.Windows.Threading.Dispatcher dispatcher, System.Func<System.Threading.Tasks.Task> func) { }
        public static System.Windows.Threading.DispatcherOperation BeginInvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, System.Action action) { }
        public static System.Windows.Threading.DispatcherOperation BeginInvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, System.Action action, System.Windows.Threading.DispatcherPriority priority) { }
        public static System.Windows.Threading.DispatcherOperation BeginInvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, System.Delegate method, params object[] args) { }
        public static System.Windows.Threading.DispatcherOperation BeginInvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, System.Delegate method, System.Windows.Threading.DispatcherPriority priority, params object[] args) { }
        public static int GetThreadId(this System.Windows.Threading.Dispatcher dispatcher) { }
        public static void Invoke(this System.Windows.Threading.Dispatcher dispatcher, System.Action action) { }
        public static void Invoke(this System.Windows.Threading.Dispatcher dispatcher, System.Action action, bool onlyBeginInvokeWhenNoAccess) { }
        public static void Invoke(this System.Windows.Threading.Dispatcher dispatcher, System.Action action, System.Windows.Threading.DispatcherPriority priority) { }
        public static void Invoke(this System.Windows.Threading.Dispatcher dispatcher, System.Delegate method, params object[] args) { }
        public static void Invoke(this System.Windows.Threading.Dispatcher dispatcher, System.Action action, System.Windows.Threading.DispatcherPriority priority, bool onlyInvokeWhenNoAccess) { }
        public static void Invoke(this System.Windows.Threading.Dispatcher dispatcher, System.Delegate method, System.Windows.Threading.DispatcherPriority priority, params object[] args) { }
        public static System.Threading.Tasks.Task InvokeAsync(this System.Windows.Threading.Dispatcher dispatcher, System.Func<System.Threading.Tasks.Task> actionAsync) { }
        public static System.Threading.Tasks.Task InvokeAsync(this System.Windows.Threading.Dispatcher dispatcher, System.Delegate method, params object[] args) { }
        public static System.Threading.Tasks.Task InvokeAsync(this System.Windows.Threading.Dispatcher dispatcher, System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task> actionAsync, System.Threading.CancellationToken cancellationToken) { }
        public static System.Threading.Tasks.Task InvokeAsync(this System.Windows.Threading.Dispatcher dispatcher, System.Delegate method, System.Windows.Threading.DispatcherPriority priority, params object[] args) { }
        public static System.Threading.Tasks.Task<T> InvokeAsync<T>(this System.Windows.Threading.Dispatcher dispatcher, System.Func<System.Threading.Tasks.Task<T>> funcAsync) { }
        public static System.Threading.Tasks.Task<T> InvokeAsync<T>(this System.Windows.Threading.Dispatcher dispatcher, System.Func<T> func) { }
        public static System.Threading.Tasks.Task<T> InvokeAsync<T>(this System.Windows.Threading.Dispatcher dispatcher, System.Func<T> func, System.Windows.Threading.DispatcherPriority priority) { }
        public static System.Threading.Tasks.Task<T> InvokeAsync<T>(this System.Windows.Threading.Dispatcher dispatcher, System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<T>> funcAsync, System.Threading.CancellationToken cancellationToken) { }
        public static void InvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, System.Action action) { }
        public static void InvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, System.Action action, System.Windows.Threading.DispatcherPriority priority) { }
        public static void InvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, System.Delegate method, params object[] args) { }
        public static void InvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, System.Delegate method, System.Windows.Threading.DispatcherPriority priority, params object[] args) { }
    }
}