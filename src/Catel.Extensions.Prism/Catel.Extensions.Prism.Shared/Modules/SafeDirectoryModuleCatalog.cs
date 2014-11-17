// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SafeDirectoryModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Modules
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;
    using Catel.Logging;
    using Microsoft.Practices.Prism;
    using Microsoft.Practices.Prism.Modularity;
    using System;

    /// <summary>
    /// Safe implementation of the <see cref="DirectoryModuleCatalog"/> which does not crash when
    /// the .NET runtime tries to load different versions of an assembly.
    /// <remarks>
    /// This code originally comes from the official Prism implementation.
    /// </remarks>
    /// </summary>
    public class SafeDirectoryModuleCatalog : ModuleCatalog
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Directory containing modules to search for.
        /// </summary>
        public string ModulePath { get; set; }

        /// <summary>
        /// Drives the main logic of building the child domain and searching for the assemblies.
        /// </summary>
        protected override void InnerLoad()
        {
            Argument.IsNotNullOrWhitespace(ModulePath, "ModulePath");

            if (!Directory.Exists(ModulePath))
            {
                string error = string.Format("Directory '{0}' not found", ModulePath);
                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            var childDomain = BuildChildDomain(AppDomain.CurrentDomain);

            try
            {
                var loadedAssemblies = new List<string>();

                var assemblies = (from Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
                                  where !(assembly is System.Reflection.Emit.AssemblyBuilder)
                                     && assembly.GetType().FullName != "System.Reflection.Emit.InternalAssemblyBuilder"
                                     && !String.IsNullOrEmpty(assembly.Location)
                                  select assembly.Location);

                loadedAssemblies.AddRange(assemblies);

                var loaderType = typeof(InnerModuleInfoLoader);

                if (loaderType.Assembly != null)
                {
                    var loader = (InnerModuleInfoLoader)childDomain.CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName).Unwrap();
                    loader.LoadAssemblies(loadedAssemblies);
                    Items.AddRange(loader.GetModuleInfos(ModulePath));
                }
            }
            finally
            {
                AppDomain.Unload(childDomain);
            }
        }


        /// <summary>
        /// Creates a new child domain and copies the evidence from a parent domain.
        /// </summary>
        /// <param name="parentDomain">The parent domain.</param>
        /// <returns>The new child domain.</returns>
        /// <remarks>
        /// Grabs the <paramref name="parentDomain"/> evidence and uses it to construct the new
        /// <see cref="AppDomain"/> because in a ClickOnce execution environment, creating an
        /// <see cref="AppDomain"/> will by default pick up the partial trust environment of 
        /// the AppLaunch.exe, which was the root executable. The AppLaunch.exe does a 
        /// create domain and applies the evidence from the ClickOnce manifests to 
        /// create the domain that the application is actually executing in. This will 
        /// need to be Full Trust for Composite Application Library applications.
        /// </remarks>
        /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if <paramref name="parentDomain"/> is null.</exception>
        protected virtual AppDomain BuildChildDomain(AppDomain parentDomain)
        {
            Argument.IsNotNull(() => parentDomain);

            var evidence = new Evidence(parentDomain.Evidence);
            var setup = parentDomain.SetupInformation;
            return AppDomain.CreateDomain("DiscoveryRegion", evidence, setup);
        }

        private class InnerModuleInfoLoader : MarshalByRefObject
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            internal ModuleInfo[] GetModuleInfos(string path)
            {
                var directory = new DirectoryInfo(path);

                ResolveEventHandler resolveEventHandler = (sender, args) => OnReflectionOnlyResolve(args, directory);

                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += resolveEventHandler;

                var moduleReflectionOnlyAssembly = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().First(asm => asm.FullName == typeof(IModule).Assembly.FullName);
                var moduleType = moduleReflectionOnlyAssembly.GetType(typeof(IModule).FullName);

                var modules = GetNotAllreadyLoadedModuleInfos(directory, moduleType);

                var array = modules.ToArray();
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= resolveEventHandler;

                return array;
            }

            private static IEnumerable<ModuleInfo> GetNotAllreadyLoadedModuleInfos(DirectoryInfo directory, Type moduleType)
            {
                var validAssemblies = new List<FileInfo>();
                var alreadyLoadedAssemblies = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();

                var fileInfos = directory.GetFiles("*.dll").Where(file => alreadyLoadedAssemblies.FirstOrDefault(assembly => String.Compare(Path.GetFileName(assembly.Location), file.Name, StringComparison.OrdinalIgnoreCase) == 0) == null);

                foreach (var fileInfo in fileInfos)
                {
                    try
                    {
                        if (Assembly.ReflectionOnlyLoadFrom(fileInfo.FullName) != null)
                        {
                            validAssemblies.Add(fileInfo);
                        }
                    }
                    catch (BadImageFormatException)
                    {
                        Log.Warning("Skipping assembly '{0}', not a .NET dll", fileInfo.FullName);   
                    }
                }

                var moduleInfoList = new List<ModuleInfo>();
                foreach (var validAssembly in validAssemblies)
                {
                    Log.Info("Loading module from '{0}'", validAssembly.FullName);

                    try
                    {
                        var moduleInfo = Assembly.ReflectionOnlyLoadFrom(validAssembly.FullName)
                            .GetExportedTypes()
                            .Where(moduleType.IsAssignableFrom)
                            .Where(t => t != moduleType)
                            .Where(t => !t.IsAbstract)
                            .Select(CreateModuleInfo).FirstOrDefault();

                        if (moduleInfo != null)
                        {
                            moduleInfoList.Add(moduleInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to load assembly '{0}', cannot determine module info", validAssembly.FullName);
                    }
                }

                return moduleInfoList;
            }

            private static Assembly OnReflectionOnlyResolve(ResolveEventArgs args, DirectoryInfo directory)
            {
                var loadedAssembly = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().FirstOrDefault(asm => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase));
                if (loadedAssembly != null)
                {
                    return loadedAssembly;
                }

                var assemblyName = new AssemblyName(args.Name);
                string dependentAssemblyFilename = Path.Combine(directory.FullName, assemblyName.Name + ".dll");
                if (File.Exists(dependentAssemblyFilename))
                {
                    try
                    {
                        return Assembly.ReflectionOnlyLoadFrom(dependentAssemblyFilename);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to load assembly from location '{0}'", dependentAssemblyFilename);
                    }
                }

                try
                {
                    return Assembly.ReflectionOnlyLoad(args.Name);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to load assembly by name '{0}'", args.Name);
                }

                return null;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            internal void LoadAssemblies(IEnumerable<string> assemblies)
            {
                foreach (string assemblyPath in assemblies)
                {
                    try
                    {
                        Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                    }
                    catch (FileNotFoundException ex)
                    {
                        Log.Warning(ex, "Failed to load assembly '{0}', skipping...", assemblyPath);
                    }
                }
            }

            private static ModuleInfo CreateModuleInfo(Type type)
            {
                string moduleName = type.Name;
                var dependsOn = new List<string>();
                bool onDemand = false;
                var moduleAttribute = CustomAttributeData.GetCustomAttributes(type).FirstOrDefault(cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleAttribute).FullName);

                if (moduleAttribute != null)
                {
                    foreach (CustomAttributeNamedArgument argument in moduleAttribute.NamedArguments)
                    {
                        string argumentName = argument.MemberInfo.Name;
                        switch (argumentName)
                        {
                            case "ModuleName":
                                moduleName = (string)argument.TypedValue.Value;
                                break;

                            case "OnDemand":
                                onDemand = (bool)argument.TypedValue.Value;
                                break;

                            case "StartupLoaded":
                                onDemand = !((bool)argument.TypedValue.Value);
                                break;
                        }
                    }
                }

                var moduleDependencyAttributes = CustomAttributeData.GetCustomAttributes(type).Where(cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleDependencyAttribute).FullName);

                foreach (CustomAttributeData cad in moduleDependencyAttributes)
                {
                    dependsOn.Add((string)cad.ConstructorArguments[0].Value);
                }

                var moduleInfo = new ModuleInfo(moduleName, type.AssemblyQualifiedName)
                {
                    InitializationMode = onDemand ? InitializationMode.OnDemand : InitializationMode.WhenAvailable,
                    Ref = type.Assembly.CodeBase,
                };

                moduleInfo.DependsOn.AddRange(dependsOn);

                return moduleInfo;
            }
        }
    }
}

#endif