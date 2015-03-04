// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadingModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if SL5

namespace Catel.Modules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using IoC;
    using Logging;
    using Microsoft.Practices.Prism.Modularity;
    using Reflection;

    /// <summary>
    /// ModuleCatalog that allows the downloading of all modules dynamically based on the <see cref="ModuleInfo.Ref"/> property.
    /// </summary>
    public class DownloadingModuleCatalog : ModuleCatalog, IDownloadingModuleCatalog
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly Dictionary<string, ModuleBase> _loadedModules = new Dictionary<string, ModuleBase>();
        private readonly Dictionary<string, Queue<Action>> _pendingLoads = new Dictionary<string, Queue<Action>>();
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the module catalog starts downloading a module.
        /// </summary>
        public event EventHandler<ModuleEventArgs> ModuleDownloading;

        /// <summary>
        /// Occurs when the module catalog has finished downloading a module.
        /// </summary>
        public event EventHandler<ModuleEventArgs> ModuleDownloaded;
        #endregion

        #region Methods
        /// <summary>
        /// Loads a specific module.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="completedCallback">The completed callback.</param>
        /// <exception cref="ArgumentException">The <paramref name="moduleName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="completedCallback"/> is <c>null</c>.</exception>
        public void LoadModule(string moduleName, Action completedCallback)
        {
            Argument.IsNotNull("moduleName", moduleName);
            Argument.IsNotNull("completedCallback", completedCallback);

            var moduleInfo = GetModuleInfoByName(moduleName);
            if (moduleInfo == null)
            {
                string error = string.Format("Module '{0}' is not registered in this catalog", moduleName);
                Log.Error(error);
                throw new NotSupportedException(error);
            }

            if (_loadedModules.ContainsKey(moduleName))
            {
                Log.Debug("Module is already loaded, invoking completed callback immediately");

                completedCallback();
                return;
            }

            if (_pendingLoads.ContainsKey(moduleName))
            {
                Log.Debug("Module is already being loaded, queueing completed callback");

                _pendingLoads[moduleName].Enqueue(completedCallback);
                return;
            }

            var pendingQueue = new Queue<Action>();
            pendingQueue.Enqueue(completedCallback);

            _pendingLoads.Add(moduleName, pendingQueue);

            Log.Debug("Searching for dependent modules of module '{0}'", moduleInfo.ModuleName);

            var dependencies = GetDependentModules(moduleInfo).ToList();
            LoadModules(dependencies, () => DownloadModule(moduleInfo));
        }

        /// <summary>
        /// Creates a <see cref="ModuleCatalog"/> from a XAML that is located on the internet. This method creates the
        /// <see cref="ModuleCatalog"/> asynchronously.
        /// </summary>
        /// <param name="builderResourceUri">The builder resource URI.</param>
        /// <param name="completedCallback">The completed callback, can be <c>null</c>.</param>
        /// <returns>
        /// An empty instance of <see cref="ModuleCatalog"/>, will be populated as soon as the download is complete.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="builderResourceUri"/> is <c>null</c>.</exception>
        public static DownloadingModuleCatalog CreateFromXamlAsync(Uri builderResourceUri, Action<ModuleCatalog> completedCallback = null)
        {
            Argument.IsNotNull("builderResourceUri", builderResourceUri);

            Log.Debug("Downloading file '{0}'", builderResourceUri);

            var moduleCatalog = new DownloadingModuleCatalog();

            var webClient = new WebClient();
            webClient.DownloadStringCompleted += OnModuleCatalogXmlCompleted;
            webClient.DownloadStringAsync(builderResourceUri, new Tuple<ModuleCatalog, Action<ModuleCatalog>>(moduleCatalog, completedCallback));

            return moduleCatalog;
        }

        /// <summary>
        /// Called when the web client has finished downloading.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DownloadStringCompletedEventArgs" /> instance containing the event data.</param>
        private static void OnModuleCatalogXmlCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var webClient = (WebClient) sender;
            webClient.DownloadStringCompleted -= OnModuleCatalogXmlCompleted;

            var userState = (Tuple<ModuleCatalog, Action<ModuleCatalog>>) e.UserState;
            var moduleCatalog = userState.Item1;
            var completedCallback = userState.Item2;

            if (e.Error != null)
            {
                Log.Error(e.Error, "An error occurred when downloading the module catalog xml");
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream))
                    {
                        writer.Write(e.Result);
                        writer.Flush();

                        memoryStream.Position = 0L;

                        var tempModuleCatalog = CreateFromXaml(memoryStream);
                        foreach (var module in tempModuleCatalog.Modules)
                        {
                            moduleCatalog.AddModule(module);
                        }
                    }
                }
            }

            if (completedCallback != null)
            {
                completedCallback(moduleCatalog);
            }
        }

        /// <summary>
        /// Does the actual work of loading the catalog.
        /// <para />
        /// The base implementation does nothing.
        /// </summary>
        protected override void InnerLoad()
        {
            foreach (var module in Modules)
            {
                if (module.InitializationMode == InitializationMode.WhenAvailable)
                {
                    LoadModule(module.ModuleName, () => Log.Debug("Completed loading initial module"));
                }
            }
        }

        /// <summary>
        /// Loads a list of modules in the right order.
        /// </summary>
        /// <param name="modules">The modules.</param>
        /// <param name="completedCallback">The completed callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="modules"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="completedCallback"/> is <c>null</c>.</exception>
        private void LoadModules(List<ModuleInfo> modules, Action completedCallback)
        {
            Argument.IsNotNull("modules", modules);
            Argument.IsNotNull("completedCallback", completedCallback);

            if (modules.Count == 0)
            {
                Log.Debug("No modules to load (anymore), invoking completed callback immediately");

                completedCallback();
                return;
            }

            LoadModule(modules[0].ModuleName, () =>
            {
                modules.RemoveAt(0);
                LoadModules(modules, completedCallback);
            });
        }

        /// <summary>
        /// Downloads the module.
        /// <para />
        /// This method will automatically take care of the pending completed callbacks.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="moduleInfo"/> is <c>null</c>.</exception>
        private void DownloadModule(ModuleInfo moduleInfo)
        {
            Argument.IsNotNull("moduleInfo", moduleInfo);

            Log.Info("Downloading module '{0}' from '{1}'", moduleInfo.ModuleName, GetModuleUri(moduleInfo));

            ModuleDownloading.SafeInvoke(this, new ModuleEventArgs(moduleInfo));

            var uri = GetModuleUri(moduleInfo);

            var webClient = new WebClient();
            webClient.OpenReadCompleted += OnDownloadModuleCompleted;
            webClient.OpenReadAsync(uri, moduleInfo);
        }

        /// <summary>
        /// Gets the Uri of the specified module.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <returns>The uri of the module.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="moduleInfo"/> is <c>null</c>.</exception>
        private Uri GetModuleUri(ModuleInfo moduleInfo)
        {
            Argument.IsNotNull("moduleInfo", moduleInfo);

            Uri uri;

            if (string.IsNullOrWhiteSpace(moduleInfo.Ref))
            {
                Log.Debug("Ref property of module '{0}' is null or whitespace, automatically falling back to host", moduleInfo.ModuleName);

                var fullyQualifiedAssemblyName = TypeHelper.GetAssemblyName(moduleInfo.ModuleType);
                var assemblyName = TypeHelper.GetAssemblyNameWithoutOverhead(fullyQualifiedAssemblyName);
                var url = HostHelper.GetHostUrl(string.Format("/ClientBin/{0}.xap", assemblyName));

                Log.Debug("Falling back to host url '{0}'", url);

                uri = new Uri(url);
            }
            else
            {
                uri = new Uri(moduleInfo.Ref);
            }

            return uri;
        }

        /// <summary>
        /// Called when the download of a module has been completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="OpenReadCompletedEventArgs" /> instance containing the event data.</param>
        private void OnDownloadModuleCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            var moduleInfo = (ModuleInfo) e.UserState;

            Log.Info("Downloaded module '{0}' from '{1}'", moduleInfo.ModuleName, GetModuleUri(moduleInfo));

            var webClient = (WebClient) sender;
            webClient.OpenReadCompleted -= OnDownloadModuleCompleted;

            if (e.Error != null)
            {
                Log.Error(e.Error, "An error occurred while downloading module '{0}', cannot handle completed callbacks", moduleInfo.ModuleName);
                return;
            }

            try
            {
                var moduleAsBytes = e.Result;

                Log.Debug("Received '{0}' kb, now registering the xap file", moduleAsBytes.Length / 1000);

                AssemblyHelper.RegisterAssembliesFromXap(e.Result, true);

                Log.Info("Instantiating module '{0}'", moduleInfo.ModuleName);

                var moduleType = TypeCache.GetType(moduleInfo.ModuleType);
                var module = TypeFactory.Default.CreateInstance(moduleType) as ModuleBase;

                Log.Info("Initializing module '{0}'", moduleInfo.ModuleName);

                module.Initialize();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while instantiating or initializing module '{0}'", moduleInfo.ModuleName);
            }
            finally
            {
                var pendingQueue = _pendingLoads[moduleInfo.ModuleName];

                Log.Debug("Calling all '{0}' completed callbacks for module '{1}'", pendingQueue.Count, moduleInfo.ModuleName);

                while (pendingQueue.Count > 0)
                {
                    var completedCallback = pendingQueue.Dequeue();
                    completedCallback();
                }

                Log.Debug("Called all completed callbacks for module '{0}'", moduleInfo.ModuleName);

                _pendingLoads.Remove(moduleInfo.ModuleName);

                ModuleDownloaded.SafeInvoke(this, new ModuleEventArgs(moduleInfo));
            }
        }
        #endregion
    }
}

#endif