namespace Catel.Core
{
    using System.Runtime.CompilerServices;
    /// <summary>
    /// Class that gets called as soon as the module is loaded.
    /// </summary>
    /// <remarks>
    /// This is made possible thanks to Fody.
    /// </remarks>
    public static class ModuleInitializer
    {

        [ModuleInitializer]
        public static void Initialize()
        {
            // Empty by design
        }
    }
}
