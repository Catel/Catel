namespace Catel
{
    using System;
    using System.Linq;
    using Reflection;

    /// <summary>
    /// Design time helper.
    /// </summary>
    public static class DesignTimeHelper
    {
        private static readonly object _lock = new object();
        private static int _lastLoadedAssembliesCount;

        /// <summary>
        /// Initializes the designer options.
        /// </summary>
        public static void InitializeDesignTime()
        {
            if (CatelEnvironment.GetIsInDesignMode(false))
            {
                lock (_lock)
                {
                    var loadedAssemblies = AssemblyHelper.GetLoadedAssemblies();
                    if (loadedAssemblies.Count != _lastLoadedAssembliesCount)
                    {
                        _lastLoadedAssembliesCount = loadedAssemblies.Count;

                        foreach (var assembly in loadedAssemblies)
                        {
                            // Part 1: support attributes
                            try
                            {
                                var attributes = assembly.GetCustomAttributesEx(typeof(DesignTimeCodeAttribute));
                                foreach (var attribute in attributes)
                                {
                                    // No need to do anything
                                }
                            }
                            catch (Exception)
                            {
                                // Never kill the designer
                            }

                            // Part 2: support types deriving from custom attributes
                            var initializerTypes = assembly.GetAllTypesSafely().Where(x => typeof(DesignTimeInitializer).IsAssignableFromEx(x));
                            foreach (var initializerType in initializerTypes)
                            {
                                try
                                {
                                    // Note: instantiating is sufficient
                                    var initializer = Activator.CreateInstance(initializerType);
                                }
                                catch (Exception)
                                {
                                    // Never kill the designer
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
