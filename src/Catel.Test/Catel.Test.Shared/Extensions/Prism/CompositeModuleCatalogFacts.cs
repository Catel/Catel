// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeModuleCatalogFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.Prism
{
    using System;

    using Catel.Modules;

    using Microsoft.Practices.Prism.Modularity;
    using NUnit.Framework;

    using ModuleCatalog = Catel.Modules.ModuleCatalog;

    /// <summary>
    /// The composite module catalog tests.
    /// </summary>
    public class CompositeModuleCatalogFacts
    {
        /*
        #region Nested type: AddModuleMethodOrItemsProperty
  
        /// <summary>
        /// The add module method or items property.
        /// </summary>
        [TestFixture]
        public class AddModuleMethod
        {
            #region Methods

            /// <summary>
            /// The are actualy synchronized.
            /// </summary>
            [TestCase]
            public void IsSynchronizedWithTheItemsPropertyIfItemsPropertyIsAccessedFromTheCompositeCatalogType()
            {
                var catalog = new CompositeModuleCatalog();
                var moduleInfoA = new ModuleInfo("ModuleA", typeof(ModuleA).FullName);
                var moduleInfoC = new ModuleInfo("ModuleC", typeof(ModuleC).FullName);
                var moduleInfoB = new ModuleInfo("ModuleB", typeof(ModuleB).FullName);

                catalog.Items.Add(moduleInfoB);
                catalog.Items.Add(moduleInfoC);
                catalog.AddModule(moduleInfoA);

                Assert.AreEqual(catalog.Items.Count, 3);
                Assert.IsTrue(catalog.Items.Contains(moduleInfoA));
                Assert.IsTrue(catalog.Items.Contains(moduleInfoB));
                Assert.IsTrue(catalog.Items.Contains(moduleInfoC));
            }

            #endregion
        }
        #endregion
        */
        #region Nested type: FooModuleCatalog

        /// <summary>
        /// The foo module catalog.
        /// </summary>
        public class FooModuleCatalog : ModuleCatalog
        {
            #region Properties

            /// <summary>
            /// Gets a value indicating whether Initialized.
            /// </summary>
            public bool Initialized { get; private set; }
            #endregion

            #region Methods

            /// <summary>
            /// Initializes the catalog, which may load and validate the modules.
            /// </summary>
            /// <exception cref="T:Microsoft.Practices.Prism.Modularity.ModularityException">
            /// When validation of the <see cref="T:Microsoft.Practices.Prism.Modularity.ModuleCatalog"/> fails, because this method calls <see cref="M:Microsoft.Practices.Prism.Modularity.ModuleCatalog.Validate"/>.
            /// </exception>
            public override void Initialize()
            {
                base.Initialize();
                Initialized = true;
            }

            #endregion
        }
        #endregion

        #region Nested type: ModuleA

        /// <summary>
        /// The module a.
        /// </summary>
        public class ModuleA : IModule
        {
            #region IModule Members

            /// <summary>
            /// The initialize.
            /// </summary>
            public void Initialize()
            {
            }

            #endregion
        }
        #endregion

        #region Nested type: ModuleB

        /// <summary>
        /// The module b.
        /// </summary>
        public class ModuleB : IModule
        {
            #region IModule Members

            /// <summary>
            /// The initialize.
            /// </summary>
            public void Initialize()
            {
            }

            #endregion
        }
        #endregion

        #region Nested type: ModuleC

        /// <summary>
        /// The module b.
        /// </summary>
        public class ModuleC : IModule
        {
            #region IModule Members

            /// <summary>
            /// The initialize.
            /// </summary>
            public void Initialize()
            {
            }

            #endregion
        }
        #endregion

        #region Nested type: TheAddMethod

        /// <summary>
        /// The the add method.
        /// </summary>
        [TestFixture]
        public class TheAddMethod
        {
            #region Methods

            /// <summary>
            /// The must throw argument null exception if is called with null argument.
            /// </summary>
            [TestCase]
            public void MustThrowArgumentNullExceptionIfIsCalledWithNullArgument()
            {
                var catalog = new CompositeModuleCatalog();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => catalog.Add(null));
            }

            #endregion
        }
        #endregion

        #region Nested type: TheInitializeMethod

        /// <summary>
        /// The the initialize method.
        /// </summary>
        [TestFixture]
        public class TheInitializeMethod
        {
            #region Methods

            /// <summary>
            /// The add duplicate module.
            /// </summary>
            [TestCase]
            public void MustThrowDuplicateModuleExceptionIfTheModuleIsRegisteredInMoreThanOnceCatalog()
            {
                var catalog = new CompositeModuleCatalog();
                var firstCatalog = new ModuleCatalog();
                firstCatalog.AddModule(new ModuleInfo("ModuleA", typeof(ModuleA).FullName));
                catalog.Add(firstCatalog);
                var secondCatalog = new ModuleCatalog();
                secondCatalog.AddModule(new ModuleInfo("ModuleA", typeof(ModuleA).FullName));
                catalog.Add(secondCatalog);
                ExceptionTester.CallMethodAndExpectException<DuplicateModuleException>(() => catalog.Initialize());
            }

            /// <summary>
            /// The if.
            /// </summary>
            [TestCase]
            public void CallsTheInitializedMethodOfAllRegisteredCatalogs()
            {
                var catalog = new CompositeModuleCatalog();

                var firstCatalog = new FooModuleCatalog();
                firstCatalog.AddModule(new ModuleInfo("ModuleA", typeof(ModuleA).FullName));
                catalog.Add(firstCatalog);

                var secondCatalog = new FooModuleCatalog();
                secondCatalog.AddModule(new ModuleInfo("ModuleB", typeof(ModuleB).FullName));
                catalog.Add(secondCatalog);
                catalog.Initialize();

                Assert.IsTrue(firstCatalog.Initialized);
                Assert.IsTrue(secondCatalog.Initialized);
            }

            #endregion
        }
        #endregion
    }
}

#endif