namespace Catel.Services
{
    /// <summary>
    /// Service to retrieve the navigation root in the application.
    /// </summary>
    public interface INavigationRootService
    {
        /// <summary>
        /// Gets the navigation root.
        /// </summary>
        /// <returns>System.Object.</returns>
        object GetNavigationRoot();
    }
}
