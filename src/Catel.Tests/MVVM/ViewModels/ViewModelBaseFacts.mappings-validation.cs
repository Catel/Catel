namespace Catel.Tests.MVVM.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestClasses;

[TestFixture]
public partial class ViewModelBaseFacts
{
    [Test]
    public void UpdatesMappedValidationAfterChangingMappedViewModelProperty()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var vm = new TestViewModelWithMappings(new Person(), serviceProvider);
        vm.DeferValidationUntilFirstSaveCallWrapper = false;

        vm.FirstNameAsTwoWay = "John";
        vm.LastName = "Doe";

        Assert.That(vm.HasErrors, Is.False);
    }

    [Test]
    public void Exposes_Validation_For_Validated_Model()
    {
        // Test case for https://github.com/Catel/Catel/issues/1615

        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var model = new Person();
        model.Validate();

        Assert.That(model.HasErrors, Is.True);

        var vm = new TestViewModelWithMappings(model, serviceProvider);
        vm.DeferValidationUntilFirstSaveCallWrapper = false;

        Assert.That(vm.HasErrors, Is.True);

        vm.FirstNameAsTwoWay = "John";
        vm.LastName = "Doe";

        Assert.That(vm.HasErrors, Is.False);
    }

    [Test]
    public void HasErrors_Returns_False_If_Model_Contains_Errors_But_Model_Validation_Is_Suspended()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var vm = new TestViewModelWithSuspendedModelValidation(new Person(), serviceProvider);

        vm.Validate();

        Assert.That(vm.HasErrors, Is.False);
    }

    [Test]
    public void HasErrors_Returns_True_If_Model_Contains_Errors_But_Model_Validation_Is_Suspended()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var vm = new TestViewModelWithEnabledModelValidation(new Person(), serviceProvider);

        vm.Validate();

        Assert.That(vm.HasErrors, Is.True);
    }

    [Test]
    public void HasWarnings_Returns_False_If_Model_Contains_Errors_But_Model_Validation_Is_Suspended()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var vm = new TestViewModelWithSuspendedModelValidation(new Person(), serviceProvider);

        vm.Validate();

        Assert.That(vm.HasWarnings, Is.False);
    }

    [Test]
    public void HasWarnings_Returns_True_If_Model_Contains_Errors_But_Model_Validation_Is_Suspended()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var vm = new TestViewModelWithEnabledModelValidation(new Person(), serviceProvider);

        vm.Validate();

        Assert.That(vm.HasWarnings, Is.True);
    }
}
