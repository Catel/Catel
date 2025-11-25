namespace Catel.Core
{
    using System.Runtime.CompilerServices;
    using Catel.IoC;

    /// <summary>
    /// Class that gets called as soon as the module is loaded.
    /// </summary>
    /// <remarks>
    /// This is made possible thanks to Fody.
    /// </remarks>
    public static class ModuleInitializer
    {
        /// <summary>
        /// Initializes the module.
        /// </summary>
#pragma warning disable CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
        [ModuleInitializer]
#pragma warning restore CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
        public static void Initialize()
        {
            var serviceLocator = ServiceLocator.Default;
            var module = new SerializationJsonModule();
            module.Initialize(serviceLocator);
        }
    }
}
