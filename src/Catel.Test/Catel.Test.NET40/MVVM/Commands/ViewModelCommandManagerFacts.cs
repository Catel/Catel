// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelCommandManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM
{
    using System;
    using Catel.MVVM;
    using ViewModels.TestClasses;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ViewModelCommandManagerFacts
    {
        [TestClass]
        public class TheCreateMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ViewModelCommandManager.Create(null));
            }

            [TestMethod]
            public void ReturnsViewModelCommandManagerForViewModel()
            {
                var viewModel = new TestViewModel();
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);

                Assert.IsNotNull(viewModelCommandManager);
            }
        }

        [TestClass]
        public class TheAddHandlerMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullHandler()
            {
                var viewModel = new TestViewModel();
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModelCommandManager.AddHandler(null));
            }            

            [TestMethod]
            public void RegisteredHandlerGetsCalled()
            {
                var viewModel = new TestViewModel();
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);
                viewModel.InitializeViewModel();

                bool called = false;

                viewModelCommandManager.AddHandler((vm, property, command, commandParameter) => called = true);
                viewModel.GenerateData.Execute();

                Assert.IsTrue(called);
            }
        }
    }
}