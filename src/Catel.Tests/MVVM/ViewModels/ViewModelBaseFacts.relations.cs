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

            Assert.That(viewModel.GetParentViewModelForTest(), Is.Null);
            ((IRelationalViewModel)viewModel).SetParentViewModel(parentViewModel);
            Assert.That(viewModel.GetParentViewModelForTest(), Is.EqualTo(parentViewModel));
        }

        [TestCase]
        public void RegisterChildViewModel_Null()
        {
            var viewModel = new TestViewModel();

            Assert.Throws<ArgumentNullException>(() => ((IRelationalViewModel)viewModel).RegisterChildViewModel(null));
        }

        /// <summary>
        /// Checks whether a child view model is correctly subscribed by making sure the parent view model is also
        /// being validated. Then, it unsubscribes the child view model by closing it.
        /// </summary>
        [TestCase]
        public async Task RegisterChildViewModel_RemovedViaClosingChildViewModelAsync()
        {
            bool validationTriggered = false;
            using (var validatedEvent = new ManualResetEvent(false))
            {
                var person = new Person();
                person.FirstName = "first name";
                person.LastName = "last name";

                var viewModel = new TestViewModel();
                var childViewModel = new TestViewModel(person);

                Assert.That(childViewModel.HasErrors, Is.False);

                ((IRelationalViewModel)viewModel).RegisterChildViewModel(childViewModel);
                ((IValidatable)viewModel).Validating += delegate
                {
                    validationTriggered = true;
                    validatedEvent.Set();
                };

                childViewModel.FirstName = string.Empty;

                validatedEvent.WaitOne(2000, false);

                Assert.That(validationTriggered, Is.True, "Validating event is not triggered");

                await childViewModel.CloseViewModelAsync(null);

                validationTriggered = false;
                validatedEvent.Reset();

                validatedEvent.WaitOne(2000, false);

                Assert.That(validationTriggered, Is.False, "Validating event should not be triggered because child view model is removed");
            }
        }

        /// <summary>
        /// Checks whether a child view model is correctly subscribed by making sure the parent view model is also
        /// being validated. Then, it unsubscribes the child view model by calling UnregisterChildViewModel.
        /// </summary>
        [TestCase]
        public void RegisterChildViewModel_RemovedViaUnregisterChildViewModel()
        {
            bool validationTriggered = false;
            using (ManualResetEvent validatedEvent = new ManualResetEvent(false))
            {
                Person person = new Person();
                person.FirstName = "first_name";
                person.LastName = "last_name";

                var viewModel = new TestViewModel();
                var childViewModel = new TestViewModel(person);

                Assert.That(childViewModel.HasErrors, Is.False);

                ((IRelationalViewModel)viewModel).RegisterChildViewModel(childViewModel);
                ((IValidatable)viewModel).Validating += delegate
                {
                    validationTriggered = true;
                    validatedEvent.Set();
                };

                childViewModel.FirstName = string.Empty;

                validatedEvent.WaitOne(2000, false);

                Assert.That(validationTriggered, Is.True, "Validating event is not triggered");

                ((IRelationalViewModel)viewModel).UnregisterChildViewModel(childViewModel);

                validationTriggered = false;
                validatedEvent.Reset();

                validatedEvent.WaitOne(2000, false);

                Assert.That(validationTriggered, Is.False, "Validating event should not be triggered because child view model is removed");
            }
        }

        [TestCase]
        public void ChildViewModelUpdatesValidation()
        {
            var person = new Person();
            person.FirstName = "first_name";

            var viewModel = new TestViewModel();
            var childViewModel = new TestViewModel(person);

            ((IRelationalViewModel)viewModel).RegisterChildViewModel(childViewModel);

            Assert.That(viewModel.HasErrors, Is.True);
            Assert.That(childViewModel.HasErrors, Is.True);

            person.LastName = "last_name";

            Assert.That(viewModel.HasErrors, Is.False);
            Assert.That(childViewModel.HasErrors, Is.False);

            person.LastName = string.Empty;

            Assert.That(viewModel.HasErrors, Is.True);
            Assert.That(childViewModel.HasErrors, Is.True);
        }

        [TestFixture]
        public class The_RegisterChildViewModel_Method
        {
            [Test]
            public void Throws_ArgumentNullException_For_Null_ChildViewModel()
            {
                var viewModel = new TestViewModel();

                Assert.Throws<ArgumentNullException>(() => ((IRelationalViewModel)viewModel).RegisterChildViewModel(null));
            }

            [Test]
            public async Task Revalidates_Parent_After_Adding_ChildViewModel_Async()
            {
                var parentViewModel = new TestViewModel(new SpecialValidationModel());
                var childViewModel = new TestViewModel(new SpecialValidationModel());

                await parentViewModel.InitializeViewModelAsync();
                await childViewModel.InitializeViewModelAsync();

                childViewModel.BusinessRuleErrorWhenEmpty = null;

                Assert.That(parentViewModel.HasErrors, Is.False);
                Assert.That(childViewModel.HasErrors, Is.True);

                ((IRelationalViewModel)parentViewModel).RegisterChildViewModel(childViewModel);

                Assert.That(parentViewModel.HasErrors, Is.True);
            }
        }

        [TestFixture]
        public class The_UnregisterChildViewModel_Method
        {
            [Test]
            public void Throws_ArgumentNullException_For_Null_ChildViewModel()
            {
                var viewModel = new TestViewModel();

                Assert.Throws<ArgumentNullException>(() => ((IRelationalViewModel)viewModel).UnregisterChildViewModel(null));
            }

            [Test]
            public async Task Revalidates_Parent_After_Removing_ChildViewModel_Async()
            {
                var parentViewModel = new TestViewModel(new SpecialValidationModel());
                var childViewModel = new TestViewModel(new SpecialValidationModel());

                await parentViewModel.InitializeViewModelAsync();
                await childViewModel.InitializeViewModelAsync();

                ((IRelationalViewModel)parentViewModel).RegisterChildViewModel(childViewModel);

                Assert.That(parentViewModel.HasErrors, Is.False);

                childViewModel.BusinessRuleErrorWhenEmpty = null;

                Assert.That(parentViewModel.HasErrors, Is.True);

                ((IRelationalViewModel)parentViewModel).UnregisterChildViewModel(childViewModel);

                Assert.That(parentViewModel.HasErrors, Is.False);
            }
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

                Assert.That(parentVm.DeferValidationUntilFirstSaveValue, Is.True);
                Assert.That(childVm.DeferValidationUntilFirstSaveValue, Is.True);
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

                Assert.That(grantParentVm.DeferValidationUntilFirstSaveValue, Is.False);
                Assert.That(parentVm.DeferValidationUntilFirstSaveValue, Is.False);
                Assert.That(childVm.DeferValidationUntilFirstSaveValue, Is.False);

                parentVm.DeferValidationUntilFirstSaveValue = true;

                Assert.That(grantParentVm.DeferValidationUntilFirstSaveValue, Is.False);
                Assert.That(parentVm.DeferValidationUntilFirstSaveValue, Is.True);
                Assert.That(childVm.DeferValidationUntilFirstSaveValue, Is.True);
            }
        }
    }
}
