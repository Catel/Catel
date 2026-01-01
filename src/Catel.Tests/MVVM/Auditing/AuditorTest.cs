namespace Catel.Tests.MVVM.Auditing
{
    using System.Threading.Tasks;
    using Catel.MVVM.Auditing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class AuditorTest
    {
        [TestCase]
        public void OnViewModelCreating()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);

            Assert.That(auditor.OnViewModelCreatingCalled, Is.EqualTo(true));
            Assert.That(auditor.OnViewModelCreatingType, Is.EqualTo(typeof(TestViewModel)));
        }

        [TestCase]
        public void OnViewModelCreated()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);

            Assert.That(auditor.OnViewModelCreatedCalled, Is.EqualTo(true));
            Assert.That(auditor.OnViewModelCreatedType, Is.EqualTo(typeof(TestViewModel)));
        }

        [TestCase]
        public async Task OnViewModelInitializedAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);
            await viewModel.InitializeViewModelAsync();

            Assert.That(auditor.OnViewModelInitializedCalled, Is.EqualTo(true));
            Assert.That(auditor.OnViewModelInitializedType, Is.EqualTo(typeof(TestViewModel)));
        }

        [TestCase]
        public void OnPropertyChanged()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);
            viewModel.TestProperty = "test";

            Assert.That(auditor.OnPropertyChangedCalled, Is.EqualTo(true));
            Assert.That(auditor.OnPropertyChangedViewModel, Is.EqualTo(viewModel));
            Assert.That(auditor.OnPropertyChangedPropertyName, Is.EqualTo("TestProperty"));
            Assert.That(auditor.OnPropertyChangedNewValue, Is.EqualTo("test"));
        }

        [TestCase]
        public void OnPropertyChanged_IgnoredProperties()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditor.PropertiesToIgnore.Add("TestProperty");
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);
            viewModel.TestProperty = "test";

            Assert.That(auditor.OnPropertyChangedCalled, Is.EqualTo(false));
            Assert.That(auditor.OnPropertyChangedViewModel, Is.EqualTo(null));
            Assert.That(auditor.OnPropertyChangedPropertyName, Is.EqualTo(null));
            Assert.That(auditor.OnPropertyChangedNewValue, Is.EqualTo(null));
        }

        [TestCase]
        public void OnCommandExecuted()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);
            viewModel.TestCommand.Execute("test");

            Assert.That(auditor.OnCommandExecutedCalled, Is.EqualTo(true));
            Assert.That(auditor.OnCommandExecutedViewModel, Is.EqualTo(viewModel));
            Assert.That(auditor.OnCommandExecutedCommandName, Is.EqualTo("TestCommand"));
            Assert.That(auditor.OnCommandExecutedCommand, Is.EqualTo(viewModel.TestCommand));
            Assert.That(auditor.OnCommandExecutedCommandParameter, Is.EqualTo("test"));
        }

        [TestCase]
        public async Task OnViewModelSavingAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);
            await viewModel.SaveViewModelAsync();

            Assert.That(auditor.OnViewModelSavingCalled, Is.EqualTo(true));
            Assert.That(auditor.OnViewModelSavingViewModel, Is.EqualTo(viewModel));
        }

        [TestCase]
        public async Task OnViewModelSavedAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);
            await viewModel.SaveViewModelAsync();

            Assert.That(auditor.OnViewModelSavedCalled, Is.EqualTo(true));
            Assert.That(auditor.OnViewModelSavedViewModel, Is.EqualTo(viewModel));
        }

        [TestCase]
        public async Task OnViewModelCancelingAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);
            await viewModel.CancelViewModelAsync();

            Assert.That(auditor.OnViewModelCancelingCalled, Is.EqualTo(true));
            Assert.That(auditor.OnViewModelCancelingViewModel, Is.EqualTo(viewModel));
        }

        [TestCase]
        public async Task OnViewModelCanceledAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);
            await viewModel.CancelViewModelAsync();

            Assert.That(auditor.OnViewModelCanceledCalled, Is.EqualTo(true));
            Assert.That(auditor.OnViewModelCanceledViewModel, Is.EqualTo(viewModel));
        }

        [TestCase]
        public async Task OnViewModelClosingAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);
            await viewModel.CloseViewModelAsync(null);

            Assert.That(auditor.OnViewModelClosingCalled, Is.EqualTo(true));
            Assert.That(auditor.OnViewModelClosingViewModel, Is.EqualTo(viewModel));
        }

        [TestCase]
        public async Task OnViewModelClosedAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel(serviceProvider);
            await viewModel.CloseViewModelAsync(null);

            Assert.That(auditor.OnViewModelClosedCalled, Is.EqualTo(true));
            Assert.That(auditor.OnViewModelClosedViewModel, Is.EqualTo(viewModel));
        }
    }
}
