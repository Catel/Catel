// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelCommandManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM
{
    using System;
    using Catel.MVVM;
    using ViewModels.TestClasses;
    using NUnit.Framework;

    public class ViewModelCommandManagerFacts
    {
        [TestFixture]
        public class TheCreateMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ViewModelCommandManager.Create(null));
            }

            [TestCase]
            public void ReturnsViewModelCommandManagerForViewModel()
            {
                var viewModel = new TestViewModel();
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);

                Assert.IsNotNull(viewModelCommandManager);
            }
        }

        [TestFixture]
        public class TheAddHandlerMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullHandler()
            {
                var viewModel = new TestViewModel();
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModelCommandManager.AddHandler(null));
            }            

            [TestCase]
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