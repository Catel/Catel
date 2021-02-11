namespace Catel.Services
{
    public interface IDispatcherProviderService
    {
        object GetApplicationDispatcher();
        object GetCurrentDispatcher();
    }
}
