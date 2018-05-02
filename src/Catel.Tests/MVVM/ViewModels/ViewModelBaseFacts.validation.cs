// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBaseFacts.validation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.MVVM.ViewModels
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using NUnit.Framework;
    using TestClasses;

    public partial class ViewModelBaseFacts
    {
        [TestCase]
        public async Task CanSaveViewModelWithSuspendedValidationAsync()
        {
            var person = new Person();
            var vm = new TestViewModel(person);

            vm.Validate();

            Assert.IsTrue(vm.HasErrors);

            using (vm.SuspendValidations())
            {
                var hasSaved = await vm.SaveViewModelAsync();

                Assert.IsTrue(hasSaved);
            }
        }

        //[TestCase]
        //public void DeferredValidation()
        //{
        //    var viewModel = new TestViewModelWithDeferredValidation();
        //    viewModel.FirstName = null;

        //    Assert.IsTrue(viewModel.HasErrors);
        //    Assert.AreEqual(string.Empty, ((IDataErrorInfo)viewModel)["FirstName"]);
        //    Assert.IsFalse(viewModel.SaveViewModel());
        //    Assert.IsTrue(viewModel.HasErrors);
        //    Assert.AreNotEqual(string.Empty, ((IDataErrorInfo)viewModel)["FirstName"]);
        //}

        [TestCase]
        public void ModelValidation_NotifyDataErrorInfo_FieldErrors()
        {
            var testViewModel = new TestViewModel();

            Assert.IsFalse(testViewModel.HasErrors);

            testViewModel.SpecialValidationModel = new SpecialValidationModel();

            Assert.IsFalse(testViewModel.HasErrors);

            testViewModel.SpecialValidationModel.FieldErrorWhenEmpty = string.Empty;

            Assert.IsTrue(testViewModel.HasErrors);
            Assert.AreNotEqual(string.Empty, ((IDataErrorInfo) testViewModel)["FieldErrorWhenEmpty"]);

            testViewModel.SpecialValidationModel.FieldErrorWhenEmpty = "no error";

            Assert.IsFalse(testViewModel.HasErrors);
        }

        [TestCase]
        public void ModelValidation_NotifyDataErrorInfo_BusinessErrors()
        {
            var testViewModel = new TestViewModel();

            Assert.IsFalse(testViewModel.HasErrors);

            testViewModel.SpecialValidationModel = new SpecialValidationModel();

            Assert.IsFalse(testViewModel.HasErrors);

            testViewModel.SpecialValidationModel.BusinessRuleErrorWhenEmpty = string.Empty;

            Assert.IsTrue(testViewModel.HasErrors);
            Assert.AreNotEqual(string.Empty, ((IDataErrorInfo) testViewModel).Error);

            testViewModel.SpecialValidationModel.BusinessRuleErrorWhenEmpty = "no error";

            Assert.IsFalse(testViewModel.HasErrors);
        }

        [TestCase]
        public void ModelValidation_NotifyDataWarningInfo_FieldWarnings()
        {
            var testViewModel = new TestViewModel();
            var validation = testViewModel;

            Assert.IsFalse(validation.HasWarnings);

            testViewModel.SpecialValidationModel = new SpecialValidationModel();

            Assert.IsFalse(validation.HasWarnings);

            testViewModel.SpecialValidationModel.FieldWarningWhenEmpty = string.Empty;

            Assert.IsTrue(validation.HasWarnings);
            Assert.AreNotEqual(string.Empty, ((IDataWarningInfo) testViewModel)["FieldWarningWhenEmpty"]);

            testViewModel.SpecialValidationModel.FieldWarningWhenEmpty = "no warning";

            Assert.IsFalse(validation.HasWarnings);
        }

        [TestCase]
        public void ModelValidation_NotifyDataWarningInfo_BusinessWarnings()
        {
            var testViewModel = new TestViewModel();
            var validation = testViewModel;

            Assert.IsFalse(validation.HasWarnings);

            testViewModel.SpecialValidationModel = new SpecialValidationModel();

            Assert.IsFalse(validation.HasWarnings);

            testViewModel.SpecialValidationModel.BusinessRuleWarningWhenEmpty = string.Empty;

            Assert.IsTrue(validation.HasWarnings);
            Assert.AreNotEqual(string.Empty, ((IDataWarningInfo) testViewModel).Warning);

            testViewModel.SpecialValidationModel.BusinessRuleWarningWhenEmpty = "no warning";

            Assert.IsFalse(validation.HasWarnings);
        }

        [TestCase]
        public void GetValidationSummary_WithoutTagFiltering()
        {
            var viewModel = new TestViewModelWithValidationTags();
            viewModel.Validate();

            var summary = viewModel.GetValidationSummary(true);

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsNotNull(summary);
            Assert.AreEqual(2, summary.FieldErrors.Count);
        }

        [TestCase]
        public void GetValidationSummary_NullTag()
        {
            var viewModel = new TestViewModelWithValidationTags();
            viewModel.Validate();

            var summary = viewModel.GetValidationSummary(true, null);

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsNotNull(summary);
            Assert.AreEqual(0, summary.FieldErrors.Count);
        }

        [TestCase]
        public void GetValidationSummary_NonExistingTag()
        {
            var viewModel = new TestViewModelWithValidationTags();
            viewModel.Validate();

            var summary = viewModel.GetValidationSummary(true, "NonExistingTag");

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsNotNull(summary);
            Assert.AreEqual(0, summary.FieldErrors.Count);
        }

        [TestCase]
        public void GetValidationSummary_ExistingTag()
        {
            var viewModel = new TestViewModelWithValidationTags();
            viewModel.Validate();

            var summary = viewModel.GetValidationSummary(true, "PersonValidation");

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsNotNull(summary);
            Assert.AreEqual(2, summary.FieldErrors.Count);
        }
    }
}