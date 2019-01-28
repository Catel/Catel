// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBaseFacts.relations.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.MVVM.ViewModels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel.Data;
    using Catel.MVVM;
    using NUnit.Framework;
    using TestClasses;

    public partial class ViewModelBaseFacts
    {
        [TestCase]
        public void SetParentviewModel()
        {
            var viewModel = new TestViewModel();
            var parentViewModel = new TestViewModel();

            Assert.IsNull(viewModel.GetParentViewModelForTest());
            ((IRelationalViewModel)viewModel).SetParentViewModel(parentViewModel);
            Assert.AreEqual(parentViewModel, viewModel.GetParentViewModelForTest());
        }

        [TestCase]
        public void RegisterChildViewModel_Null()
        {
            var viewModel = new TestViewModel();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ((IRelationalViewModel)viewModel).RegisterChildViewModel(null));
        }

        /// <summary>
        /// Checks whether a child view model is correctly subscribed by making sure the parent view model is also
        /// being validated. Then, it unsubscribes the child view model by closing it.
        /// </summary>
        [TestCase]
        public async Task RegisterChildViewModel_RemovedViaClosingChildViewModel()
        {
            bool validationTriggered = false;
            var validatedEvent = new ManualResetEvent(false);

            var person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var viewModel = new TestViewModel();
            var childViewModel = new TestViewModel(person);

            Assert.IsFalse(childViewModel.HasErrors);

            ((IRelationalViewModel)viewModel).RegisterChildViewModel(childViewModel);
            ((IValidatable)viewModel).Validating += delegate
            {
                validationTriggered = true;
                validatedEvent.Set();
            };

            childViewModel.FirstName = string.Empty;

#if NET || NETCORE
            validatedEvent.WaitOne(2000, false);
#else
            validatedEvent.WaitOne(2000);
#endif
            Assert.IsTrue(validationTriggered, "Validating event is not triggered");

            await childViewModel.CloseViewModelAsync(null);

            validationTriggered = false;
            validatedEvent.Reset();

#if NET || NETCORE
            validatedEvent.WaitOne(2000, false);
#else
            validatedEvent.WaitOne(2000);
#endif
            Assert.IsFalse(validationTriggered, "Validating event should not be triggered because child view model is removed");
        }

        /// <summary>
        /// Checks whether a child view model is correctly subscribed by making sure the parent view model is also
        /// being validated. Then, it unsubscribes the child view model by calling UnregisterChildViewModel.
        /// </summary>
        [TestCase]
        public void RegisterChildViewModel_RemovedViaUnregisterChildViewModel()
        {
            bool validationTriggered = false;
            ManualResetEvent validatedEvent = new ManualResetEvent(false);

            Person person = new Person();
            person.FirstName = "first_name";
            person.LastName = "last_name";

            var viewModel = new TestViewModel();
            var childViewModel = new TestViewModel(person);

            Assert.IsFalse(childViewModel.HasErrors);

            ((IRelationalViewModel)viewModel).RegisterChildViewModel(childViewModel);
            ((IValidatable)viewModel).Validating += delegate
            {
                validationTriggered = true;
                validatedEvent.Set();
            };

            childViewModel.FirstName = string.Empty;

#if NET || NETCORE
            validatedEvent.WaitOne(2000, false);
#else
            validatedEvent.WaitOne(2000);
#endif
            Assert.IsTrue(validationTriggered, "Validating event is not triggered");

            ((IRelationalViewModel)viewModel).UnregisterChildViewModel(childViewModel);

            validationTriggered = false;
            validatedEvent.Reset();

#if NET || NETCORE
            validatedEvent.WaitOne(2000, false);
#else
            validatedEvent.WaitOne(2000);
#endif
            Assert.IsFalse(validationTriggered, "Validating event should not be triggered because child view model is removed");
        }

        [TestCase]
        public void ChildViewModelUpdatesValidation()
        {
            var person = new Person();
            person.FirstName = "first_name";

            var viewModel = new TestViewModel();
            var childViewModel = new TestViewModel(person);

            ((IRelationalViewModel)viewModel).RegisterChildViewModel(childViewModel);

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsTrue(childViewModel.HasErrors);

            person.LastName = "last_name";

            Assert.IsFalse(viewModel.HasErrors);
            Assert.IsFalse(childViewModel.HasErrors);

            person.LastName = string.Empty;

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsTrue(childViewModel.HasErrors);
        }


        [TestFixture]
        public class DeferValidationUntilFirstSaveCallWithChildViewModels
        {
            public class DeferViewModelBase : ViewModelBase
            {
                public bool DeferValidationUntilFirstSaveValue
                {
                    get { return base.DeferValidationUntilFirstSaveCall; }
                    set { base.DeferValidationUntilFirstSaveCall = value; }
                }
            }

            public class GrantParentViewModel : DeferViewModelBase
            {
                
            }

            public class ParentViewModel : DeferViewModelBase
            {
                
            }

            public class ChildViewModel : DeferViewModelBase
            {
                
            }

            [TestCase]
            public void RetrievesFromParentWhenAttachingViewModelToTree()
            {
                var grantParentVm = new GrantParentViewModel();
                var parentVm = new ParentViewModel();
                var childVm = new ChildViewModel();

                grantParentVm.DeferValidationUntilFirstSaveValue = true;

                ((IRelationalViewModel)grantParentVm).RegisterChildViewModel(parentVm);
                ((IRelationalViewModel)parentVm).SetParentViewModel(grantParentVm);

                ((IRelationalViewModel)parentVm).RegisterChildViewModel(childVm);
                ((IRelationalViewModel)childVm).SetParentViewModel(parentVm);

                Assert.IsTrue(parentVm.DeferValidationUntilFirstSaveValue);
                Assert.IsTrue(childVm.DeferValidationUntilFirstSaveValue);
            }

            [TestCase]
            public void UpdatesChildsWhenUpdatingDeferValidationUntilFirstSave()
            {
                var grantParentVm = new GrantParentViewModel();
                var parentVm = new ParentViewModel();
                var childVm = new ChildViewModel();

                grantParentVm.DeferValidationUntilFirstSaveValue = false;
                parentVm.DeferValidationUntilFirstSaveValue = false;
                childVm.DeferValidationUntilFirstSaveValue = false;

                ((IRelationalViewModel)grantParentVm).RegisterChildViewModel(parentVm);
                ((IRelationalViewModel)parentVm).RegisterChildViewModel(childVm);

                Assert.IsFalse(grantParentVm.DeferValidationUntilFirstSaveValue);
                Assert.IsFalse(parentVm.DeferValidationUntilFirstSaveValue);
                Assert.IsFalse(childVm.DeferValidationUntilFirstSaveValue);

                parentVm.DeferValidationUntilFirstSaveValue = true;

                Assert.IsFalse(grantParentVm.DeferValidationUntilFirstSaveValue);
                Assert.IsTrue(parentVm.DeferValidationUntilFirstSaveValue);
                Assert.IsTrue(childVm.DeferValidationUntilFirstSaveValue);
            }
        }
    }
}
