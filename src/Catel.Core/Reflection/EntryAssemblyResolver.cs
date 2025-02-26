namespace Catel.Reflection
{
    using System.Reflection;

    public class EntryAssemblyResolver : IEntryAssemblyResolver
    {
        public Assembly Resolve()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly is null)
            {
                throw new CatelException("Could not determine entry assembly");
            }

            return assembly;
        }
    }
}
