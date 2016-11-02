// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Windows.Markup;
    using Logging;

#if PRISM6
    using Prism.Modularity;
#else
    using Microsoft.Practices.Prism;
    using Microsoft.Practices.Prism.Modularity;
#endif

    using Threading;

    /// <summary>
    /// The <see cref="ModuleCatalog"/> holds information about the modules that can be used by the 
    /// application. Each module is described in a <see cref="ModuleInfo"/> class, that records the 
    /// name, type and location of the module. 
    /// 
    /// It also verifies that the <see cref="ModuleCatalog"/> is internally valid. That means that
    /// it does not have:
    /// <list>
    ///     <item>Circular dependencies</item>
    ///     <item>Missing dependencies</item>
    ///     <item>
    ///         Invalid dependencies, such as a Module that's loaded at startup that depends on a module 
    ///         that might need to be retrieved.
    ///     </item>
    /// </list>
    /// The <see cref="ModuleCatalog"/> also serves as a baseclass for more specialized Catalogs .
    /// </summary>
    /// <remarks>
    /// This implementation is actually based on the original source of Prism. But is thread safe and actually allow inherits from it with correctness.
    /// </remarks>
    [ContentProperty("Items")]
    public class ModuleCatalog : IModuleCatalog
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        /// <summary>
        /// The items.
        /// </summary>
        private readonly ModuleCatalogItemCollection _items;

        /// <summary>
        /// The synchronization context.
        /// </summary>
        protected readonly SynchronizationContext _synchronizationContext = new SynchronizationContext();

        /// <summary>
        /// The is loaded.
        /// </summary>
        private bool _isLoaded;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleCatalog"/> class.
        /// </summary>
        public ModuleCatalog()
        {
            _items = new ModuleCatalogItemCollection();
            _items.BeginCollectionChanged += (sender, args) => _synchronizationContext.Acquire();
            _items.EndCollectionChanged += (sender, args) => _synchronizationContext.Release();
            _items.CollectionChanged += ItemsCollectionChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleCatalog" /> class while providing an
        /// initial list of <see cref="ModuleInfo" />s.
        /// </summary>
        /// <param name="modules">The initial list of modules.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="modules" /> is <c>null</c>.</exception>
        public ModuleCatalog(IEnumerable<ModuleInfo> modules)
            : this()
        {
            Argument.IsNotNull("modules", modules);

            foreach (var moduleInfo in modules)
            {
                AddModule(moduleInfo);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the items in the <see cref="ModuleCatalog" />. This property is mainly used to add <see cref="ModuleInfoGroup" />s or
        /// <see cref="ModuleInfo" />s through XAML.
        /// </summary>
        /// <value>The items in the catalog.</value>
        public Collection<IModuleCatalogItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets the <see cref="ModuleInfoGroup"/>s that have been added to the <see cref="ModuleCatalog"/>. 
        /// </summary>
        /// <value>The groups.</value>
        public IEnumerable<ModuleInfoGroup> Groups
        {
            get { return QueryItems.OfType<ModuleInfoGroup>(); }
        }

        /// <summary>
        /// Gets a readonly access to Items.
        /// </summary>
        /// <remarks>Override this property to enumerate the module catalog items</remarks>
        public virtual IEnumerable<IModuleCatalogItem> QueryItems
        {
            get { return Items; }
        }

        /// <summary>
        /// Gets or sets a value that remembers whether the <see cref="ModuleCatalog"/> has been validated already. 
        /// </summary>
        protected bool Validated { get; set; }

        /// <summary>
        /// Returns the list of <see cref="ModuleInfo"/>s that are not contained within any <see cref="ModuleInfoGroup"/>. 
        /// </summary>
        /// <value>The groupless modules.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Groupless")]
        protected IEnumerable<ModuleInfo> GrouplessModules
        {
            get { return QueryItems.OfType<ModuleInfo>(); }
        }
        #endregion

        #region IModuleCatalog Members
        /// <summary>
        /// Gets all the <see cref="ModuleInfo"/> classes that are in the <see cref="ModuleCatalog"/>, regardless 
        /// if they are within a <see cref="ModuleInfoGroup"/> or not. 
        /// </summary>
        /// <value>The modules.</value>
        public virtual IEnumerable<ModuleInfo> Modules
        {
            get { return GrouplessModules.Union(Groups.SelectMany(g => g)); }
        }

        /// <summary>
        /// Return the list of <see cref="ModuleInfo" />s that <paramref name="moduleInfo" /> depends on.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo" /> to get the</param>
        /// <returns>An enumeration of <see cref="ModuleInfo" /> that <paramref name="moduleInfo" /> depends on.</returns>
        /// <remarks>If  the <see cref="ModuleCatalog" /> was not yet validated, this method will call <see cref="Validate" />.</remarks>
        public virtual IEnumerable<ModuleInfo> GetDependentModules(ModuleInfo moduleInfo)
        {
            EnsureCatalogValidated();

            return GetDependentModulesInner(moduleInfo);
        }

        /// <summary>
        /// Returns a list of <see cref="ModuleInfo" />s that contain both the <see cref="ModuleInfo" />s in
        /// <paramref name="modules" />, but also all the modules they depend on.
        /// </summary>
        /// <param name="modules">The modules to get the dependencies for.</param>
        /// <returns>A list of <see cref="ModuleInfo" /> that contains both all <see cref="ModuleInfo" />s in <paramref name="modules" />
        /// but also all the <see cref="ModuleInfo" /> they depend on.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="modules" /> is <c>null</c>.</exception>
        public virtual IEnumerable<ModuleInfo> CompleteListWithDependencies(IEnumerable<ModuleInfo> modules)
        {
            Argument.IsNotNull("modules", modules);

            EnsureCatalogValidated();

            var completeList = new List<ModuleInfo>();
            List<ModuleInfo> pendingList = modules.ToList();
            while (pendingList.Count > 0)
            {
                ModuleInfo moduleInfo = pendingList[0];

                foreach (ModuleInfo dependency in GetDependentModules(moduleInfo))
                {
                    if (!completeList.Contains(dependency) && !pendingList.Contains(dependency))
                    {
                        pendingList.Add(dependency);
                    }
                }

                pendingList.RemoveAt(0);
                completeList.Add(moduleInfo);
            }

            IEnumerable<ModuleInfo> sortedList = Sort(completeList);
            return sortedList;
        }

        /// <summary>
        /// Adds a <see cref="ModuleInfo" /> to the <see cref="ModuleCatalog" />.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo" /> to add.</param>
        /// <returns>The <see cref="T:Microsoft.Practices.Prism.Modularity.ModuleCatalog" /> for easily adding multiple modules.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo" /> is <c>null</c>.</exception>
        public virtual void AddModule(ModuleInfo moduleInfo)
        {
            Argument.IsNotNull("moduleInfo", moduleInfo);

            Log.Debug("Adding a module {0} to a module catalog", moduleInfo.ModuleName);

            if (!Items.Contains(moduleInfo))
            {
                Items.Add(moduleInfo);
            }
        }

        /// <summary>
        /// Initializes the catalog, which may load and validate the modules.
        /// </summary>
        /// <exception cref="ModularityException">When validation of the <see cref="ModuleCatalog"/> fails, because this method calls <see cref="Validate"/>.</exception>
        public virtual void Initialize()
        {
            if (!_isLoaded)
            {
                Load();
            }

            Validate();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a <see cref="ModuleCatalog" /> from XAML.
        /// </summary>
        /// <param name="xamlStream"><see cref="Stream" /> that contains the XAML declaration of the catalog.</param>
        /// <returns>An instance of <see cref="ModuleCatalog" /> built from the XAML.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="xamlStream" /> is <c>null</c>.</exception>
        public static ModuleCatalog CreateFromXaml(Stream xamlStream)
        {
            Argument.IsNotNull("xamlStream", xamlStream);

            // Note: create custom module catalog that users can use prism example xaml code as well
            var moduleCatalog = new ModuleCatalog();

            var temporaryModuleCatalog = XamlReader.Load(xamlStream) as IModuleCatalog;
            if (temporaryModuleCatalog != null)
            {
                foreach (var module in temporaryModuleCatalog.Modules)
                {
                    moduleCatalog.AddModule(module);
                }
            }

            return moduleCatalog;
        }

        /// <summary>
        /// Creates a <see cref="ModuleCatalog" /> from a XAML included as an Application Resource.
        /// </summary>
        /// <param name="builderResourceUri">Relative <see cref="Uri" /> that identifies the XAML included as an Application Resource.</param>
        /// <returns>An instance of <see cref="ModuleCatalog" /> build from the XAML.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="builderResourceUri" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="builderResourceUri" /> points to an url, which must be downloaded asynchronously.</exception>
        public static ModuleCatalog CreateFromXaml(Uri builderResourceUri)
        {
            Argument.IsNotNull("builderResourceUri", builderResourceUri);

            if (builderResourceUri.ToString().StartsWith("http"))
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("Url '{0}' is an http url. Use CreateFromXamlAsync instead", builderResourceUri);
            }

            var streamInfo = System.Windows.Application.GetResourceStream(builderResourceUri);

            if ((streamInfo != null) && (streamInfo.Stream != null))
            {
                return CreateFromXaml(streamInfo.Stream);
            }

            return null;
        }

        /// <summary>
        /// Loads the catalog if necessary.
        /// </summary>
        public void Load()
        {
            _isLoaded = true;
            InnerLoad();
        }

        /// <summary>
        /// Validates the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <exception cref="ModularityException">When validation of the <see cref="ModuleCatalog"/> fails.</exception>
        public virtual void Validate()
        {
            ValidateUniqueModules();
            ValidateDependencyGraph();
            ValidateCrossGroupDependencies();
            ValidateDependenciesInitializationMode();

            Validated = true;
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo" /> to the catalog.
        /// </summary>
        /// <param name="moduleType"><see cref="Type" /> of the module to be added.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName" />) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog" /> instance with the added module.</returns>
        public ModuleCatalog AddModule(Type moduleType, params string[] dependsOn)
        {
            return AddModule(moduleType, InitializationMode.WhenAvailable, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo" /> to the catalog.
        /// </summary>
        /// <param name="moduleType"><see cref="Type" /> of the module to be added.</param>
        /// <param name="initializationMode">Stage on which the module to be added will be initialized.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName" />) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog" /> instance with the added module.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleType" /> is <c>null</c>.</exception>
        public ModuleCatalog AddModule(Type moduleType, InitializationMode initializationMode, params string[] dependsOn)
        {
            Argument.IsNotNull("moduleType", moduleType);

            return AddModule(moduleType.Name, moduleType.AssemblyQualifiedName, initializationMode, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo" /> to the catalog.
        /// </summary>
        /// <param name="moduleName">Name of the module to be added.</param>
        /// <param name="moduleType"><see cref="Type" /> of the module to be added.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName" />) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog" /> instance with the added module.</returns>
        public ModuleCatalog AddModule(string moduleName, string moduleType, params string[] dependsOn)
        {
            return AddModule(moduleName, moduleType, InitializationMode.WhenAvailable, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo" /> to the catalog.
        /// </summary>
        /// <param name="moduleName">Name of the module to be added.</param>
        /// <param name="moduleType"><see cref="Type" /> of the module to be added.</param>
        /// <param name="initializationMode">Stage on which the module to be added will be initialized.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName" />) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog" /> instance with the added module.</returns>
        public ModuleCatalog AddModule(string moduleName, string moduleType, InitializationMode initializationMode, params string[] dependsOn)
        {
            return AddModule(moduleName, moduleType, null, initializationMode, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo" /> to the catalog.
        /// </summary>
        /// <param name="moduleName">Name of the module to be added.</param>
        /// <param name="moduleType"><see cref="Type" /> of the module to be added.</param>
        /// <param name="refValue">Reference to the location of the module to be added assembly.</param>
        /// <param name="initializationMode">Stage on which the module to be added will be initialized.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName" />) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog" /> instance with the added module.</returns>
        /// <exception cref="System.ArgumentException">The <paramref name="moduleName" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="moduleType" /> is <c>null</c> or whitespace.</exception>
        public ModuleCatalog AddModule(string moduleName, string moduleType, string refValue, InitializationMode initializationMode, params string[] dependsOn)
        {
            Argument.IsNotNullOrWhitespace("moduleName", moduleName);
            Argument.IsNotNullOrWhitespace("moduleType", moduleType);

            var moduleInfo = new ModuleInfo(moduleName, moduleType);
            moduleInfo.DependsOn.AddRange(dependsOn);
            moduleInfo.InitializationMode = initializationMode;
            moduleInfo.Ref = refValue;
            Items.Add(moduleInfo);

            return this;
        }

        /// <summary>
        /// Creates and adds a <see cref="ModuleInfoGroup" /> to the catalog.
        /// </summary>
        /// <param name="initializationMode">Stage on which the module group to be added will be initialized.</param>
        /// <param name="refValue">Reference to the location of the module group to be added.</param>
        /// <param name="moduleInfos">Collection of <see cref="ModuleInfo" /> included in the group.</param>
        /// <returns><see cref="ModuleCatalog" /> with the added module group.</returns>
        /// <exception cref="System.ArgumentException">The <paramref name="moduleInfos" /> is <c>null</c> or an empty array.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos")]
        public virtual ModuleCatalog AddGroup(InitializationMode initializationMode, string refValue, params ModuleInfo[] moduleInfos)
        {
            Argument.IsNotNullOrEmptyArray("moduleInfos", moduleInfos);

            Log.Debug("Initializing module group");
            var newGroup = new ModuleInfoGroup { InitializationMode = initializationMode, Ref = refValue };
            foreach (var info in moduleInfos)
            {
                Log.Debug("Adding a module {0} to module group", info.ModuleName);

                newGroup.Add(info);
            }

            Items.Add(newGroup);

            return this;
        }

        /// <summary>
        /// Checks for cyclic dependencies, by calling the dependencysolver.
        /// </summary>
        /// <param name="modules">the.</param>
        /// <returns>The System.String[].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="modules" /> is <c>null</c>.</exception>
        protected static string[] SolveDependencies(IEnumerable<ModuleInfo> modules)
        {
            Argument.IsNotNull("modules", modules);

            var solver = new ModuleDependencySolver();

            foreach (var data in modules)
            {
                solver.AddModule(data.ModuleName);

                if (data.DependsOn != null)
                {
                    foreach (string dependency in data.DependsOn)
                    {
                        solver.AddDependency(data.ModuleName, dependency);
                    }
                }
            }

            if (solver.ModuleCount > 0)
            {
                return solver.Solve();
            }

            return new string[0];
        }

        /// <summary>
        /// Ensures that all the dependencies within <paramref name="modules" /> refer to <see cref="ModuleInfo" />s
        /// within that list.
        /// </summary>
        /// <param name="modules">The modules to validate modules for.</param>
        /// <exception cref="ModularityException">Throws if a <see cref="ModuleInfo" /> in <paramref name="modules" /> depends on a module that's
        /// not in <paramref name="modules" />.</exception>
        /// <exception cref="System.ArgumentNullException">Throws if <paramref name="modules" /> is <see langword="null" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="modules" /> is <c>null</c>.</exception>
        protected static void ValidateDependencies(IEnumerable<ModuleInfo> modules)
        {
            Argument.IsNotNull("modules", modules);

            var moduleNames = modules.Select(m => m.ModuleName).ToList();
            foreach (ModuleInfo moduleInfo in modules)
            {
                if (moduleInfo.DependsOn != null && moduleInfo.DependsOn.Except(moduleNames).Any())
                {
                    var exception = new ModularityException(moduleInfo.ModuleName, string.Format(CultureInfo.CurrentCulture, "Module {0} depends on other modules that don't belong to the same group.", moduleInfo.ModuleName));

                    Log.Error(exception);

                    throw exception;
                }
            }
        }

        /// <summary>
        /// Does the actual work of loading the catalog.
        /// <para />
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void InnerLoad()
        {
        }

        /// <summary>
        /// Gets a <see cref="ModuleInfo" /> by a module name.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns>The <see cref="ModuleInfo" /> or <c>null</c> if the module could not be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="moduleName" /> is <c>null</c> or whitespace.</exception>
        protected ModuleInfo GetModuleInfoByName(string moduleName)
        {
            Argument.IsNotNullOrWhitespace("moduleName", moduleName);

            return (from moduleInfo in Modules
                    where string.Equals(moduleInfo.ModuleName, moduleName)
                    select moduleInfo).FirstOrDefault();
        }

        /// <summary>
        /// Sorts a list of <see cref="ModuleInfo" />s. This method is called by <see cref="CompleteListWithDependencies" />
        /// to return a sorted list.
        /// </summary>
        /// <param name="modules">The <see cref="ModuleInfo" />s to sort.</param>
        /// <returns>Sorted list of <see cref="ModuleInfo" />s</returns>
        protected virtual IEnumerable<ModuleInfo> Sort(IEnumerable<ModuleInfo> modules)
        {
            return SolveDependencies(modules).Select(moduleName => modules.First(m => m.ModuleName == moduleName));
        }

        /// <summary>
        /// Makes sure all modules have an Unique name.
        /// </summary>
        /// <exception cref="DuplicateModuleException">Thrown if the names of one or more modules are not unique.</exception>
        protected virtual void ValidateUniqueModules()
        {
            Log.Debug("Validating uniquely usage of module names");

            List<string> moduleNames = _synchronizationContext.Execute(() => Modules.Select(m => m.ModuleName).ToList());

            string duplicateModule = moduleNames.FirstOrDefault(m => moduleNames.Count(m2 => m2 == m) > 1);

            if (duplicateModule != null)
            {
                var exception = new DuplicateModuleException(duplicateModule, string.Format(CultureInfo.CurrentCulture, "A duplicated module with name {0} has been found by the loader.", duplicateModule));

                Log.Error(exception);

                throw exception;
            }
        }

        /// <summary>
        /// Ensures that there are no cyclic dependencies. 
        /// </summary>
        protected virtual void ValidateDependencyGraph()
        {
            SolveDependencies(Modules);
        }

        /// <summary>
        /// Ensures that there are no dependencies between modules on different groups.
        /// </summary>
        /// <remarks>
        /// A groupless module can only depend on other groupless modules.
        /// A module within a group can depend on other modules within the same group and/or on groupless modules.
        /// </remarks>
        protected virtual void ValidateCrossGroupDependencies()
        {
            Log.Debug("Validating cross group dependencies");

            _synchronizationContext.Execute(() =>
                                                {
                                                    ValidateDependencies(GrouplessModules);
                                                    foreach (ModuleInfoGroup group in Groups)
                                                    {
                                                        ValidateDependencies(GrouplessModules.Union(group));
                                                    }
                                                });
        }

        /// <summary>
        /// Ensures that there are no modules marked to be loaded <see cref="InitializationMode.WhenAvailable"/>
        /// depending on modules loaded <see cref="InitializationMode.OnDemand"/>
        /// </summary>
        protected virtual void ValidateDependenciesInitializationMode()
        {
            Log.Debug("Validating Dependencies initialization mode");

            ModuleInfo moduleInfo = _synchronizationContext.Execute(() => Modules.FirstOrDefault(m => m.InitializationMode == InitializationMode.WhenAvailable && GetDependentModulesInner(m).Any(dependency => dependency.InitializationMode == InitializationMode.OnDemand)));

            if (moduleInfo != null)
            {
                var exception = new ModularityException(moduleInfo.ModuleName, string.Format(CultureInfo.CurrentCulture, "Module {0} is marked for automatic initialization when the application starts, but it depends on modules that are marked as OnDemand initialization. To fix this error, mark the dependency modules for InitializationMode=WhenAvailable, or remove this validation by extending the ModuleCatalog class.", moduleInfo.ModuleName));

                Log.Error(exception);

                throw exception;
            }
        }

        /// <summary>
        /// Returns the <see cref="ModuleInfo" /> on which the received module dependens on.
        /// </summary>
        /// <param name="moduleInfo">Module whose dependant modules are requested.</param>
        /// <returns>Collection of <see cref="ModuleInfo" /> dependants of <paramref name="moduleInfo" />.</returns>
        protected virtual IEnumerable<ModuleInfo> GetDependentModulesInner(ModuleInfo moduleInfo)
        {
            return _synchronizationContext.Execute(() => Modules.Where(dependantModule => moduleInfo.DependsOn.Contains(dependantModule.ModuleName)));
        }

        /// <summary>
        /// Ensures that the catalog is validated.
        /// </summary>
        protected virtual void EnsureCatalogValidated()
        {
            if (!Validated)
            {
                Validate();
            }
        }

        /// <summary>
        /// The items collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Validated)
            {
                EnsureCatalogValidated();
            }
        }
        #endregion

        #region Nested type: ModuleCatalogItemCollection
        /// <summary>
        /// The module catalog item collection.
        /// </summary>
        private class ModuleCatalogItemCollection : Collection<IModuleCatalogItem>, INotifyCollectionChanged
        {
            #region INotifyCollectionChanged Members
            /// <summary>
            /// The collection changed.
            /// </summary>
            public event NotifyCollectionChangedEventHandler CollectionChanged;
            #endregion

            #region Methods
            /// <summary>
            /// The insert item.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="item">The item.</param>
            protected override void InsertItem(int index, IModuleCatalogItem item)
            {
                OnBeginCollectionChanged(EventArgs.Empty);
                base.InsertItem(index, item);
                OnEndCollectionChanged(EventArgs.Empty);

                OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }

            /// <summary>
            /// The on notify collection changed.
            /// </summary>
            /// <param name="eventArgs">The event args.</param>
            private void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
            {
                if (CollectionChanged != null)
                {
                    CollectionChanged(this, eventArgs);
                }
            }

            /// <summary>
            /// The on begin collection changed.
            /// </summary>
            /// <param name="e">The event arg.</param>
            private void OnBeginCollectionChanged(EventArgs e)
            {
                EventHandler handler = BeginCollectionChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            /// <summary>
            /// The on end collection changed.
            /// </summary>
            /// <param name="e">The event arg.</param>
            private void OnEndCollectionChanged(EventArgs e)
            {
                EventHandler handler = EndCollectionChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            /// <summary>
            /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1" />.
            /// </summary>
            /// <param name="index">The zero-based index of the element to remove.</param>
            /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is less than zero.-or-<paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count" />.</exception>
            protected override void RemoveItem(int index)
            {
                OnBeginCollectionChanged(EventArgs.Empty);
                base.RemoveItem(index);
                OnEndCollectionChanged(EventArgs.Empty);

                OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, this[index], index));
            }
            #endregion

            /// <summary>
            /// The begin collection changed.
            /// </summary>
            public event EventHandler BeginCollectionChanged;

            /// <summary>
            /// The end collection changed.
            /// </summary>
            public event EventHandler EndCollectionChanged;
        }
        #endregion
    }
}