namespace Catel.Tests.MVVM.ViewModels
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
            var viewModel = new ManagedViewModel(typeof(TestViewModel));
            Assert.That(viewModel.ViewModelType, Is.EqualTo(typeof(TestViewModel)));
        }

        [TestCase]
        public void AddViewModelInstance_Null()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof(TestViewModel));

            Assert.Throws<ArgumentNullException>(() => viewModel.AddViewModelInstance(null));
        }

        [TestCase]
        public void AddViewModelInstance_WrongType()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof(TestViewModel));

            try
            {
                viewModel.AddViewModelInstance(new TestViewModelWithDeferredValidation());

                Assert.Fail("Expected WrongViewModelTypeException");
            }
            catch (WrongViewModelTypeException ex)
            {
                Assert.That(ex.ActualType, Is.EqualTo(typeof(TestViewModelWithDeferredValidation)));
                Assert.That(ex.ExpectedType, Is.EqualTo(typeof(TestViewModel)));
            }
        }

        [TestCase]
        public void AddViewModelInstance_NewInstance()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof(TestViewModel));
            viewModel.AddViewModelInstance(new TestViewModel());
        }

        [TestCase]
        public void RemoveViewModelInstance_Null()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof(TestViewModel));

            Assert.Throws<ArgumentNullException>(() => viewModel.RemoveViewModelInstance(null));
        }

        [TestCase]
        public void RemoveViewModelInstance_NotRegisteredViewModel()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof(TestViewModel));
            viewModel.RemoveViewModelInstance(new TestViewModel());
        }

        [TestCase]
        public void RemoveViewModelInstance_RegisteredViewModel()
        {
            ViewModelManager.ClearAll();

            var viewModel = new ManagedViewModel(typeof(TestViewModel));

            var interestingViewModel = new TestViewModel();
            viewModel.AddViewModelInstance(interestingViewModel);
            viewModel.RemoveViewModelInstance(interestingViewModel);
        }
        #endregion
    }
}
