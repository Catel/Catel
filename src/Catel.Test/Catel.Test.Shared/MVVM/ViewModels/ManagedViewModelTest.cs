// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagedViewModelTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels
{
    using System;
    using Catel.MVVM;

    using TestClasses;

    using NUnit.Framework;

    [TestFixture]
    public class ManagedViewModelTest
    {
        #region Methods
        [TestCase]
        public void Constructor()
        {
            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));
            Assert.AreEqual(typeof (InterestingViewModel), viewModel.ViewModelType);
        }

        [TestCase]
        public void AddViewModelInstance_Null()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModel.AddViewModelInstance(null));
        }

        [TestCase]
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

        [TestCase]
        public void AddViewModelInstance_NewInstance()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));
            viewModel.AddViewModelInstance(new InterestingViewModel());
        }

        [TestCase]
        public void RemoveViewModelInstance_Null()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModel.RemoveViewModelInstance(null));
        }

        [TestCase]
        public void RemoveViewModelInstance_NotRegisteredViewModel()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));
            viewModel.RemoveViewModelInstance(new InterestingViewModel());
        }

        [TestCase]
        public void RemoveViewModelInstance_RegisteredViewModel()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            var interestingViewModel = new InterestingViewModel();
            viewModel.AddViewModelInstance(interestingViewModel);
            viewModel.RemoveViewModelInstance(interestingViewModel);
        }

        [TestCase]
        public void AddInterestedViewModel_Null()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModel.AddInterestedViewModel(null));
        }

        [TestCase]
        public void RemoveInterestedViewModel_Null()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModel.RemoveInterestedViewModel(null));
        }

        [TestCase]
        public async void InterestingViewModelPropertyChanged()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            viewModel.AddViewModelInstance(interestingViewModel);
            viewModel.AddInterestedViewModel(interestedViewModel);

            interestingViewModel.InterestingValue = "new value";
            Assert.AreEqual("new value", interestedViewModel.InterestedValue);

            await interestingViewModel.CloseViewModel(false);
            await interestedViewModel.CloseViewModel(false);
        }

        [TestCase]
        public async void InterestingViewModelCommandExecuted()
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

            await interestingViewModel.CloseViewModel(false);
            await interestedViewModel.CloseViewModel(false);
        }

        [TestCase]
        public async void InterestingViewModelCommandExecutedWithCommandParameter()
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

            await interestingViewModel.CloseViewModel(false);
            await interestedViewModel.CloseViewModel(false);
        }

        [TestCase]
        public async void InterestedViewModelAutomaticallyBeingRemovedWhenClosed()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof (InterestingViewModel));

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            viewModel.AddViewModelInstance(interestingViewModel);
            viewModel.AddInterestedViewModel(interestedViewModel);

            interestingViewModel.InterestingValue = "new value";
            Assert.AreEqual("new value", interestedViewModel.InterestedValue);

            await interestedViewModel.CloseViewModel(null);

            interestingViewModel.InterestingValue = "new value which has changed";
            Assert.AreNotEqual("new value which has changed", interestedViewModel.InterestedValue);
            Assert.AreEqual("new value", interestedViewModel.InterestedValue);

            await interestingViewModel.CloseViewModel(false);
            await interestedViewModel.CloseViewModel(false);
        }

        [TestCase]
        public async void InterestingViewModel_Event_SavingAndSaved()
        {
            ViewModelManager.ClearAll();

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            await interestingViewModel.SaveViewModel();

            Assert.AreEqual(2, interestedViewModel.ViewModelEvents.Count);
            Assert.AreEqual(ViewModelEvent.Saving, interestedViewModel.ViewModelEvents[0]);
            Assert.AreEqual(ViewModelEvent.Saved, interestedViewModel.ViewModelEvents[1]);

            await interestingViewModel.CloseViewModel(false);
            await interestedViewModel.CloseViewModel(false);
        }

        [TestCase]
        public async void InterestingViewModel_Event_CancelingAndCanceled()
        {
            ViewModelManager.ClearAll();

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            await interestingViewModel.CancelViewModel();

            Assert.AreEqual(2, interestedViewModel.ViewModelEvents.Count);
            Assert.AreEqual(ViewModelEvent.Canceling, interestedViewModel.ViewModelEvents[0]);
            Assert.AreEqual(ViewModelEvent.Canceled, interestedViewModel.ViewModelEvents[1]);

            await interestingViewModel.CloseViewModel(false);
            await interestedViewModel.CloseViewModel(false);
        }

        [TestCase]
        public async void InterestingViewModel_Event_Closed()
        {
            ViewModelManager.ClearAll();

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            await interestingViewModel.CloseViewModel(null);

            Assert.AreEqual(1, interestedViewModel.ViewModelEvents.Count);
            Assert.AreEqual(ViewModelEvent.Closed, interestedViewModel.ViewModelEvents[0]);

            await interestingViewModel.CloseViewModel(false);
            await interestedViewModel.CloseViewModel(false);
        }
        #endregion
    }
}