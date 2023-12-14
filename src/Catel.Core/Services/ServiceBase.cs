namespace Catel.Services
{
    /// <summary>
    /// Base class for services.
    /// </summary>
    public abstract class ServiceBase : IService
    {
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        public virtual string Name
        {
            get { return GetType().Name; }
        }
    }
}
