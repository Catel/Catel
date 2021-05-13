// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDomain.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if UAP_DEFAULT

namespace System
{
    using Catel.Collections;
    using Catel.Logging;

    using Collections.Generic;
    using Reflection;
    using System.Linq;
    using Catel.Reflection;
    using MethodTimer;

    using global::Windows.ApplicationModel;
    using global::Windows.Storage.Search;
    using Catel;
    using Collections.Immutable;

    /// <summary>
    /// WinRT implementation of the AppDomain class.
    /// </summary>
    public sealed class AppDomain
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly HashSet<string> KnownPrefixesToIgnore = new HashSet<string>();
        private readonly object _lock = new object();

        private Assembly[] _loadedAssemblies;

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="AppDomain" /> class.
        /// </summary>
        static AppDomain()
        {
            CurrentDomain = new AppDomain();

            KnownPrefixesToIgnore.Add("clrcompression");
            KnownPrefixesToIgnore.Add("clrjit");
            KnownPrefixesToIgnore.Add("ucrtbased");
            KnownPrefixesToIgnore.Add("methodtimer");
            KnownPrefixesToIgnore.Add("catel.fody.attributes");
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current domain.
        /// </summary>
        /// <value>The current domain.</value>
        public static AppDomain CurrentDomain { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the assemblies in the current application domain.
        /// </summary>
        /// <returns></returns>
        [Time]
        public Assembly[] GetAssemblies()
        {
            lock (_lock)
            {
                if (_loadedAssemblies is null)
                {
                    var loadedAssemblies = new List<Assembly>();

                    var folder = Package.Current.InstalledLocation;

                    // Note: normally it's bad practice to use task.Wait(), but GetAssemblies must be blocking to cache it all

                    // Note: winmd cannot be read, see https://stackoverflow.com/questions/9136683/cannot-get-types-from-winmd-file,
                    // so we create a hashset of all winmd files and ignore them
                    var winmdQueryOptions = new QueryOptions(CommonFileQuery.OrderByName, new[] { ".winmd" });
                    winmdQueryOptions.FolderDepth = FolderDepth.Shallow;

                    var winmdQueryResult = folder.CreateFileQueryWithOptions(winmdQueryOptions);
                    var winmdTask = winmdQueryResult.GetFilesAsync().AsTask();
                    winmdTask.Wait();
                    var winmdFiles = new HashSet<string>(winmdTask.Result.Select(x => x.Name.Substring(0, x.Name.Length - x.FileType.Length)), StringComparer.OrdinalIgnoreCase);

                    // Get assemblies
                    var assemblyQueryOptions = new QueryOptions(CommonFileQuery.OrderByName, new[] { ".dll", ".exe" /*, ".winmd" */ });
                    assemblyQueryOptions.FolderDepth = FolderDepth.Shallow;

                    var assemblyQueryResult = folder.CreateFileQueryWithOptions(assemblyQueryOptions);
                    var assembliesTask = assemblyQueryResult.GetFilesAsync().AsTask();
                    assembliesTask.Wait();
                    var assemblyFiles = assembliesTask.Result.ToImmutableHashSet();

                    var arrayToIgnore = KnownPrefixesToIgnore.ToArray();

                    foreach (var file in assemblyFiles)
                    {
                        try
                        {
                            if (file.Name.StartsWithAnyIgnoreCase(arrayToIgnore))
                            {
                                continue;
                            }

                            Log.Debug($"Loading assembly from file '{file.Name}'");

                            var assemblyName = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
                            if (winmdFiles.Contains(assemblyName))
                            {
                                Log.Debug($"Ignoring assembly load of '{file.Name}' because it's (most likely) not a managed assembly (because .winmd exists)");
                                continue;
                            }

                            var assembly = LoadAssemblyFromFile(file);
                            if (assembly is not null)
                            {
                                loadedAssemblies.Add(assembly);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(ex, $"Failed to load assembly '{file}'");
                        }
                    }

                    _loadedAssemblies = loadedAssemblies.ToArray();
                }

                return _loadedAssemblies;
            }
        }

        //[Time]
        private Assembly LoadAssemblyFromFile(global::Windows.Storage.StorageFile file)
        {
            try
            {
                var assemblyName = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
                var name = new AssemblyName
                {
                    Name = assemblyName
                };

                var assembly = Assembly.Load(name);

                return assembly;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Failed to load assembly '{file.Name}'");
                return null;
            }
        }
        #endregion
    }
}

#endif
