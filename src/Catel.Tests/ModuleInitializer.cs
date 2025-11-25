using System.Runtime.CompilerServices;
namespace Catel.Tests
{
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
        [ModuleInitializer]
    public static void Initialize()
        {

        }
    }
}
