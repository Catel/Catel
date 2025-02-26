namespace Catel.Reflection
{
    using System.Reflection;

    /// <summary>
    /// Determines the entry assembly.
    /// </summary>
    public interface IEntryAssemblyResolver
    {
        /// <summary>
        /// Resolves the entry assembly.
        /// </summary>
        /// <returns>Assembly.</returns>
        Assembly Resolve();
    }
}
