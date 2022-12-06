namespace Catel.Reflection
{
    using System.Reflection;

    /// <summary>
    /// Allows custom logic to be injected into <see cref="AssemblyHelper.GetEntryAssembly"/>.
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
