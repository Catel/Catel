namespace Catel.ThirdPartyNotices;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Catel;

public class ResourceBasedThirdPartyNotice : ThirdPartyNotice
{
    protected ResourceBasedThirdPartyNotice()
    {
        // Only here to allow derived classes
    }

    public ResourceBasedThirdPartyNotice(string title, string url, string assemblyName, string relativeResourceName)
    {
        var assembly = Catel.Reflection.AssemblyHelper.GetLoadedAssemblies().First(x => (x.GetName().Name ?? string.Empty).EqualsIgnoreCase(assemblyName));

        Initialize(title, url, assembly, assembly.GetName().Name ?? string.Empty, relativeResourceName);
    }

    public ResourceBasedThirdPartyNotice(string title, string url, string assemblyName, string rootNamespace, string relativeResourceName)
    {
        var assembly = Catel.Reflection.AssemblyHelper.GetLoadedAssemblies().First(x => (x.GetName().Name ?? string.Empty).EqualsIgnoreCase(assemblyName));

        Initialize(title, url, assembly, rootNamespace, relativeResourceName);
    }

    public ResourceBasedThirdPartyNotice(string title, string url, Assembly assembly, string relativeResourceName)
    {
        Initialize(title, url, assembly, assembly.GetName().Name ?? string.Empty, relativeResourceName);
    }

    public ResourceBasedThirdPartyNotice(string title, string url, Assembly assembly, string rootNamespace, string relativeResourceName)
    {
        Initialize(title, url, assembly, rootNamespace, relativeResourceName);
    }

    protected void Initialize(string title, string url, Assembly assembly, string rootNamespace, string relativeResourceName)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(assembly);

        Title = title;
        Url = url;

        using (var memoryStream = new MemoryStream())
        {
            ResourceHelper.ExtractEmbeddedResource(assembly, rootNamespace, relativeResourceName, memoryStream);

            memoryStream.Position = 0L;

            using (var textReader = new StreamReader(memoryStream))
            {
                Content = "[failed to load resources]";

                var content = textReader.ReadToEnd();
                if (!string.IsNullOrEmpty(content))
                {
                    Content = content;
                }
            }
        }
    }
}
