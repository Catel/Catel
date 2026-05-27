namespace Catel.Tests.MVVM.ViewModels;

using System.ComponentModel;
using System.Threading.Tasks;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestClasses;

public partial class ViewModelBaseFacts
{
    [TestCase]
    public async Task CanSaveViewModelWithSuspendedValidationAsync()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var person = new Person();
        var vm = new TestFeaturedViewModel(person, serviceProvider);

        vm.Validate();

        Assert.That(vm.HasErrors, Is.True);

        using (vm.SuspendValidations())
        {
            var hasSaved = await vm.SaveViewModelAsync();

            Assert.That(hasSaved, Is.True);
        }
    }

    [TestCase]
    public async Task DelaysValidationUntilInitializeViewModelAsyncIsCalledAsync()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModel = new TestViewModelWithValidationTags(serviceProvider);

        Assert.That(viewModel.HasErrors, Is.False);

        await viewModel.InitializeViewModelAsync();

        Assert.That(viewModel.HasErrors, Is.True);
    }

    [TestCase]
    public void ModelValidation_NotifyDataErrorInfo_FieldErrors()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var testViewModel = new TestFeaturedViewModel(serviceProvider);

        Assert.That(testViewModel.HasErrors, Is.False);

        testViewModel.SpecialValidationModel = new SpecialValidationModel();

        Assert.That(testViewModel.HasErrors, Is.False);

        testViewModel.SpecialValidationModel.FieldErrorWhenEmpty = string.Empty;

        Assert.That(testViewModel.HasErrors, Is.True);
        Assert.That(((IDataErrorInfo)testViewModel)["FieldErrorWhenEmpty"], Is.Not.EqualTo(string.Empty));

        testViewModel.SpecialValidationModel.FieldErrorWhenEmpty = "no error";

        Assert.That(testViewModel.HasErrors, Is.False);
    }

    [TestCase]
    public void ModelValidation_NotifyDataErrorInfo_BusinessErrors()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var testViewModel = new TestFeaturedViewModel(serviceProvider);

        Assert.That(testViewModel.HasErrors, Is.False);

        testViewModel.SpecialValidationModel = new SpecialValidationModel();

        Assert.That(testViewModel.HasErrors, Is.False);

        testViewModel.SpecialValidationModel.BusinessRuleErrorWhenEmpty = string.Empty;

        Assert.That(testViewModel.HasErrors, Is.True);
        Assert.That(((IDataErrorInfo)testViewModel).Error, Is.Not.EqualTo(string.Empty));

        testViewModel.SpecialValidationModel.BusinessRuleErrorWhenEmpty = "no error";

        Assert.That(testViewModel.HasErrors, Is.False);
    }

    [TestCase]
    public void ModelValidation_NotifyDataWarningInfo_FieldWarnings()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var testViewModel = new TestFeaturedViewModel(serviceProvider);
        var validation = testViewModel;

        Assert.That(validation.HasWarnings, Is.False);

        testViewModel.SpecialValidationModel = new SpecialValidationModel();

        Assert.That(validation.HasWarnings, Is.False);

        testViewModel.SpecialValidationModel.FieldWarningWhenEmpty = string.Empty;

        Assert.That(validation.HasWarnings, Is.True);
        Assert.That(((IDataWarningInfo)testViewModel)["FieldWarningWhenEmpty"], Is.Not.EqualTo(string.Empty));

        testViewModel.SpecialValidationModel.FieldWarningWhenEmpty = "no warning";

        Assert.That(validation.HasWarnings, Is.False);
    }

    [TestCase]
    public void ModelValidation_NotifyDataWarningInfo_BusinessWarnings()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var testViewModel = new TestFeaturedViewModel(serviceProvider);
        var validation = testViewModel;

        Assert.That(validation.HasWarnings, Is.False);

        testViewModel.SpecialValidationModel = new SpecialValidationModel();

        Assert.That(validation.HasWarnings, Is.False);

        testViewModel.SpecialValidationModel.BusinessRuleWarningWhenEmpty = string.Empty;

        Assert.That(validation.HasWarnings, Is.True);
        Assert.That(((IDataWarningInfo)testViewModel).Warning, Is.Not.EqualTo(string.Empty));

        testViewModel.SpecialValidationModel.BusinessRuleWarningWhenEmpty = "no warning";

        Assert.That(validation.HasWarnings, Is.False);
    }

    [TestCase]
    public void GetValidationSummary_WithoutTagFiltering()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModel = new TestViewModelWithValidationTags(serviceProvider);
        viewModel.Validate();

        var summary = viewModel.GetValidationSummary(true);

        Assert.That(viewModel.HasErrors, Is.True);
        Assert.That(summary, Is.Not.Null);
        Assert.That(summary.FieldErrors.Count, Is.EqualTo(2));
    }

    [TestCase]
    public void GetValidationSummary_NullTag()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModel = new TestViewModelWithValidationTags(serviceProvider);
        viewModel.Validate();

        var summary = viewModel.GetValidationSummary(true, null);

        Assert.That(viewModel.HasErrors, Is.True);
        Assert.That(summary, Is.Not.Null);
        Assert.That(summary.FieldErrors.Count, Is.EqualTo(0));
    }

    [TestCase]
    public void GetValidationSummary_NonExistingTag()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModel = new TestViewModelWithValidationTags(serviceProvider);
        viewModel.Validate();

        var summary = viewModel.GetValidationSummary(true, "NonExistingTag");

        Assert.That(viewModel.HasErrors, Is.True);
        Assert.That(summary, Is.Not.Null);
        Assert.That(summary.FieldErrors.Count, Is.EqualTo(0));
    }

    [TestCase]
    public void GetValidationSummary_ExistingTag()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModel = new TestViewModelWithValidationTags(serviceProvider);
        viewModel.Validate();

        var summary = viewModel.GetValidationSummary(true, "PersonValidation");

        Assert.That(viewModel.HasErrors, Is.True);
        Assert.That(summary, Is.Not.Null);
        Assert.That(summary.FieldErrors.Count, Is.EqualTo(2));
    }
}
