// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Tests.MVVM.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Catel.Data;
    using Catel.MVVM;

    using NUnit.Framework;

    public class ViewModelExtensionsFacts
    {
        [TestFixture]
        public class TheIsValidationSummaryOutdatedMethod
        {
            public class ValidatingViewModel : ViewModelBase
            {
                [Required]
                public string FirstName
                {
                    get { return GetValue<string>(FirstNameProperty); }
                    set { SetValue(FirstNameProperty, value); }
                }

                public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);
            }

            [TestCase]
            public void ReturnsFalseForNotOutdatedValidationContext()
            {
                var vm = new ValidatingViewModel();

                vm.FirstName = "some value";

                var lastUpdated = DateTime.Now.Ticks + 1;

                // Only full .NET supports reliable stopwatch, all other frameworks always assume outdated
#if NET
                Assert.IsFalse(vm.IsValidationSummaryOutdated(lastUpdated, true));
#else
                Assert.IsTrue(vm.IsValidationSummaryOutdated(lastUpdated, true));
#endif
            }

            [TestCase]
            public void ReturnsTrueForOutdatedValidationContext()
            {
                var vm = new ValidatingViewModel();

                vm.FirstName = "some value";

                var validation = (IValidatableModel)vm;
                var lastUpdated = validation.ValidationContext.LastModifiedTicks;

                vm.FirstName = null;

                Assert.IsTrue(vm.IsValidationSummaryOutdated(lastUpdated, true));
            }
        }
    }
}