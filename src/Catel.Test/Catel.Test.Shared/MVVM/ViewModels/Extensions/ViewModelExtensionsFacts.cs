// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.MVVM.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Catel.Data;
    using Catel.MVVM;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ViewModelExtensionsFacts
    {
        [TestClass]
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

            [TestMethod]
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

            [TestMethod]
            public void ReturnsTrueForOutdatedValidationContext()
            {
                var vm = new ValidatingViewModel();

                vm.FirstName = "some value";

                var lastUpdated = vm.ValidationContext.LastModifiedTicks;

                vm.FirstName = null;

                Assert.IsTrue(vm.IsValidationSummaryOutdated(lastUpdated, true));
            }
        }
    }
}