// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagedViewModelTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels
{
    using System;
    using Catel.MVVM;

    using TestClasses;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class ManagedViewModelTest
    {
        #region Methods
        [TestMethod]
        public void Constructor()
        {
            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));
            Assert.AreEqual(typeof (InterestingViewModel), viewModel.ViewModelType);
        }

        [TestMethod]
        public void AddViewModelInstance_Null()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModel.AddViewModelInstance(null));
        }

        [TestMethod]
        public void AddViewModelInstance_WrongType()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            try
            {
                viewModel.AddViewModelInstance(new InterestedViewModel());

                Assert.Fail("Expected WrongViewModelTypeException");
            }
            catch (WrongViewModelTypeException ex)
            {
                Assert.AreEqual(ex.ActualType, typeof (InterestedViewModel));
                Assert.AreEqual(ex.ExpectedType, typeof (InterestingViewModel));
            }
        }

        [TestMethod]
        public void AddViewModelInstance_NewInstance()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));
            viewModel.AddViewModelInstance(new InterestingViewModel());
        }

        [TestMethod]
        public void RemoveViewModelInstance_Null()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModel.RemoveViewModelInstance(null));
        }

        [TestMethod]
        public void RemoveViewModelInstance_NotRegisteredViewModel()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));
            viewModel.RemoveViewModelInstance(new InterestingViewModel());
        }

        [TestMethod]
        public void RemoveViewModelInstance_RegisteredViewModel()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            var interestingViewModel = new InterestingViewModel();
            viewModel.AddViewModelInstance(interestingViewModel);
            viewModel.RemoveViewModelInstance(interestingViewModel);
        }

        [TestMethod]
        public void AddInterestedViewModel_Null()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModel.AddInterestedViewModel(null));
        }

        [TestMethod]
        public void RemoveInterestedViewModel_Null()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModel.RemoveInterestedViewModel(null));
        }

        [TestMethod]
        public void InterestingViewModelPropertyChanged()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            viewModel.AddViewModelInstance(interestingViewModel);
            viewModel.AddInterestedViewModel(interestedViewModel);

            interestingViewModel.InterestingValue = "new value";
            Assert.AreEqual("new value", interestedViewModel.InterestedValue);

            interestingViewModel.CloseViewModel(false);
            interestedViewModel.CloseViewModel(false);
        }

        [TestMethod]
        public void InterestingViewModelCommandExecuted()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            viewModel.AddViewModelInstance(interestingViewModel);
            viewModel.AddInterestedViewModel(interestedViewModel);

            interestingViewModel.TestCommand.Execute(null);

            Assert.AreEqual(true, interestedViewModel.CommandHasBeenExecuted);
            Assert.AreEqual(false, interestedViewModel.CommandHasBeenExecutedWithParameter);

            interestingViewModel.CloseViewModel(false);
            interestedViewModel.CloseViewModel(false);
        }

        [TestMethod]
        public void InterestingViewModelCommandExecutedWithCommandParameter()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            viewModel.AddViewModelInstance(interestingViewModel);
            viewModel.AddInterestedViewModel(interestedViewModel);

            interestingViewModel.TestCommand.Execute("parameter");
            Assert.AreEqual(true, interestedViewModel.CommandHasBeenExecuted);
            Assert.AreEqual(true, interestedViewModel.CommandHasBeenExecutedWithParameter);

            interestingViewModel.CloseViewModel(false);
            interestedViewModel.CloseViewModel(false);
        }

        [TestMethod]
        public void InterestedViewModelAutomaticallyBeingRemovedWhenClosed()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            viewModel.AddViewModelInstance(interestingViewModel);
            viewModel.AddInterestedViewModel(interestedViewModel);

            interestingViewModel.InterestingValue = "new value";
            Assert.AreEqual("new value", interestedViewModel.InterestedValue);

            interestedViewModel.CloseViewModel(null);

            interestingViewModel.InterestingValue = "new value which has changed";
            Assert.AreNotEqual("new value which has changed", interestedViewModel.InterestedValue);
            Assert.AreEqual("new value", interestedViewModel.InterestedValue);

            interestingViewModel.CloseViewModel(false);
            interestedViewModel.CloseViewModel(false);
        }

        [TestMethod]
        public void InterestingViewModel_Event_SavingAndSaved()
        {
            ViewModelManager.ClearAll();

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            interestingViewModel.SaveViewModel();

            Assert.AreEqual(2, interestedViewModel.ViewModelEvents.Count);
            Assert.AreEqual(ViewModelEvent.Saving, interestedViewModel.ViewModelEvents[0]);
            Assert.AreEqual(ViewModelEvent.Saved, interestedViewModel.ViewModelEvents[1]);

            interestingViewModel.CloseViewModel(false);
            interestedViewModel.CloseViewModel(false);
        }

        [TestMethod]
        public void InterestingViewModel_Event_CancelingAndCanceled()
        {
            ViewModelManager.ClearAll();

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            interestingViewModel.CancelViewModel();

            Assert.AreEqual(2, interestedViewModel.ViewModelEvents.Count);
            Assert.AreEqual(ViewModelEvent.Canceling, interestedViewModel.ViewModelEvents[0]);
            Assert.AreEqual(ViewModelEvent.Canceled, interestedViewModel.ViewModelEvents[1]);

            interestingViewModel.CloseViewModel(false);
            interestedViewModel.CloseViewModel(false);
        }

        [TestMethod]
        public void InterestingViewModel_Event_Closed()
        {
            ViewModelManager.ClearAll();

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            interestingViewModel.CloseViewModel(null);

            Assert.AreEqual(1, interestedViewModel.ViewModelEvents.Count);
            Assert.AreEqual(ViewModelEvent.Closed, interestedViewModel.ViewModelEvents[0]);

            interestingViewModel.CloseViewModel(false);
            interestedViewModel.CloseViewModel(false);
        }
        #endregion
    }
}