namespace Catel
{
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Catel.Logging;

    /// <summary>
    /// Resource helper class to read resource files.
    /// </summary>
    public static class ResourceHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Extracts the embedded resource and reads it as a string.
        /// </summary>
        /// <param name="assembly">The assembly to read the resource from.</param>
        /// <param name="relativeResourceName">The relative name of the resource, the assembly name will automatically be added.</param>
        /// <returns>The embedded resource as a string.</returns>
        public static string? ExtractEmbeddedResource(this Assembly assembly, string relativeResourceName)
        {
            using (var memoryStream = new MemoryStream())
            {
                ExtractEmbeddedResource(assembly, relativeResourceName, memoryStream);

                if (memoryStream.Length == 0)
                {
                    return null;
                }

                memoryStream.Position = 0L;

                using (var streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Extracts the embedded resource and writes it to the target stream.
        /// </summary>
        /// <param name="assembly">The assembly to read the resource from.</param>
        /// <param name="relativeResourceName">The relative name of the resource, the assembly name will automatically be added.</param>
        /// <param name="targetStream">The target stream to write the resource to.</param>
        public static void ExtractEmbeddedResource(this Assembly assembly, string relativeResourceName, Stream targetStream)
        {
            ExtractEmbeddedResource(assembly, assembly.GetName().Name ?? "unknown", relativeResourceName, targetStream);
        }

        /// <summary>
        /// Extracts the embedded resource and writes it to the target stream.
        /// </summary>
        /// <param name="assembly">The assembly to read the resource from.</param>
        /// <param name="assemblyName">The assembly name to prefix the resource with.</param>
        /// <param name="relativeResourceName">The relative name of the resource, the assembly name will automatically be added.</param>
        /// <param name="targetStream">The target stream to write the resource to.</param>
        public static void ExtractEmbeddedResource(this Assembly assembly, string assemblyName, string relativeResourceName, Stream targetStream)
        {
            Log.Debug($"Extracting embedded resource '{relativeResourceName}' from assembly '{assembly.FullName}'");

            var finalResourceName = relativeResourceName;
            if (!string.IsNullOrWhiteSpace(assemblyName))
            {
                finalResourceName = $"{assemblyName}.{finalResourceName}";
            }

            using (var resource = assembly.GetManifestResourceStream(finalResourceName))
            {
                if (resource is null)
                {
                    var warning = new StringBuilder();
                    warning.AppendLine($"Failed to extract embedded resource '{finalResourceName}', possible names:");

                    foreach (var name in assembly.GetManifestResourceNames())
                    {
                        warning.AppendLine($"  * {name}");
                    }

                    Log.Warning(warning.ToString());
                    return;
                }

                resource.CopyTo(targetStream);
            }
        }
    }
}
