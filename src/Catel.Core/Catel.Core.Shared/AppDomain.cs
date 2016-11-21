// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDomain.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE || PCL

namespace System
{
    using Catel.Logging;

    using Collections.Generic;
    using Reflection;
    using System.Linq;

#if NETFX_CORE
    using global::Windows.ApplicationModel;
    using global::Windows.Storage.Search;
    using Catel;
#endif

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
        public Assembly[] GetAssemblies()
        {
            lock (_lock)
            {
                if (_loadedAssemblies == null)
                {
                    var loadedAssemblies = new List<Assembly>();

#if NETFX_CORE
                    var folder = Package.Current.InstalledLocation;

                    // Note: normally it's bad practice to use task.Wait(), but GetAssemblies must be blocking to cache it all

                    var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, new[] { ".dll", ".exe" });
                    var queryResult = folder.CreateFileQueryWithOptions(queryOptions);

                    var task = queryResult.GetFilesAsync().AsTask();
                    task.Wait();

                    var files = task.Result.ToList();

                    var arrayToIgnore = KnownPrefixesToIgnore.ToArray();

                    foreach (var file in files)
                    {
                        try
                        {
                            if (file.Name.StartsWithAnyIgnoreCase(arrayToIgnore))
                            {
                                continue;
                            }

                            var filename = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
                            var name = new AssemblyName
                            {
                                Name = filename
                            };

                            var assembly = Assembly.Load(name);
                            loadedAssemblies.Add(assembly);
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(ex, $"Failed to load assembly '{file.Name}'");
                        }
                    }
#else
                    var currentdomain = typeof(string).GetTypeInfo().Assembly.GetType("System.AppDomain").GetRuntimeProperty("CurrentDomain").GetMethod.Invoke(null, new object[] { });
                    var method = currentdomain.GetType().GetRuntimeMethod("GetAssemblies", new Type[] { });
                    var assemblies = method.Invoke(currentdomain, new object[] { }) as Assembly[];
                    loadedAssemblies.AddRange(assemblies);
#endif

                    _loadedAssemblies = loadedAssemblies.ToArray();
                }

                return _loadedAssemblies;
            }
        }
        #endregion
    }
}

#endif