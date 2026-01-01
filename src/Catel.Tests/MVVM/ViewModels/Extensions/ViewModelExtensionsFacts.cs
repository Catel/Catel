namespace Catel.Tests.MVVM.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Catel.Data;
    using Catel.MVVM;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    public class ViewModelExtensionsFacts
    {
        [TestFixture]
        public class TheIsValidationSummaryOutdatedMethod
        {
            public class ValidatingViewModel : ViewModelBase
            {
                public ValidatingViewModel(IServiceProvider serviceProvider) 
                    : base(serviceProvider)
                {
                }

                [Required]
                public string FirstName
                {
                    get { return GetValue<string>(FirstNameProperty); }
                    set { SetValue(FirstNameProperty, value); }
                }

                public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", string.Empty);
            }

            [TestCase]
            public void Returns_False_For_Not_Outdated_ValidationContext()
            {
                var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var vm = new ValidatingViewModel(serviceProvider);

                vm.FirstName = "some value";

                var lastUpdated = DateTime.Now.Ticks + 1;

                // Only full .NET supports reliable stopwatch, all other frameworks always assume outdated
                Assert.That(vm.IsValidationSummaryOutdated(lastUpdated, true), Is.False);
            }

            [TestCase]
            public void Returns_True_For_Outdated_ValidationContext()
            {
                var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var vm = new ValidatingViewModel(serviceProvider);

                vm.FirstName = "some value";

                var validation = (IValidatableModel)vm;
                var lastUpdated = validation.ValidationContext.LastModifiedTicks;

                vm.FirstName = null;

                Assert.That(vm.IsValidationSummaryOutdated(lastUpdated, true), Is.True);
            }
        }
    }
}
