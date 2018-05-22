// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBaseFacts.mappings-validation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.MVVM.ViewModels
{
    using Catel.MVVM;
    using NUnit.Framework;
    using TestClasses;

    [TestFixture]
    public partial class ViewModelBaseFacts
    {
        [Test]
        public void UpdatesMappedValidationAfterChangingMappedViewModelProperty()
        {
            var vm = new TestViewModelWithMappings(new Person());
            vm.DeferValidationUntilFirstSaveCallWrapper = false;

            vm.FirstNameAsTwoWay = "John";
            vm.LastName = "Doe";

            Assert.IsFalse(vm.HasErrors);
        }

        [Test]
        public void HasErrors_Returns_False_If_Model_Contains_Errors_But_Model_Validation_Is_Supended()
        {
            var vm = new TestViewModelWithSuspendedModelValidation(new Person());

            vm.Validate();

            Assert.IsFalse(vm.HasErrors);
        }

        [Test]
        public void HasErrors_Returns_True_If_Model_Contains_Errors_But_Model_Validation_Is_Supended()
        {
            var vm = new TestViewModelWithEnabledModelValidation(new Person());

            vm.Validate();

            Assert.True(vm.HasErrors);
        }

        [Test]
        public void HasWarnings_Returns_False_If_Model_Contains_Errors_But_Model_Validation_Is_Supended()
        {
            var vm = new TestViewModelWithSuspendedModelValidation(new Person());

            vm.Validate();

            Assert.IsFalse(vm.HasWarnings);
        }

        [Test]
        public void HasWarnings_Returns_True_If_Model_Contains_Errors_But_Model_Validation_Is_Supended()
        {
            var vm = new TestViewModelWithEnabledModelValidation(new Person());

            vm.Validate();

            Assert.True(vm.HasWarnings);
        }
    }
}