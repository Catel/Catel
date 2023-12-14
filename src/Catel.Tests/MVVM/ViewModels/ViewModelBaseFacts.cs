namespace Catel.Tests.MVVM.ViewModels
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Auditing;
    using Catel.Data;
    using Catel.MVVM;
    using Catel.MVVM.Auditing;
    using NUnit.Framework;
    using TestViewModel = TestClasses.TestViewModel;

    [TestFixture]
    public partial class ViewModelBaseFacts
    {
        [TestCase]
        public async Task IsNotDirtyAtStartupAsync()
        {
            var vm = new TestViewModel();

            Assert.That(vm.IsDirty, Is.False);
        }

        [TestCase]
        public async Task ProtectPropertiesAfterClosingAsync()
        {
            var vm = new TestViewModel();
            var freezable = (IFreezable)vm;

            await vm.InitializeViewModelAsync();

            vm.FirstName = "John";
            Assert.That(vm.FirstName, Is.EqualTo("John"));
            Assert.That(freezable.IsFrozen, Is.False);

            await vm.SaveAndCloseViewModelAsync();

            vm.FirstName = "Jane";
            Assert.That(vm.FirstName, Is.EqualTo("John"));
            Assert.That(freezable.IsFrozen, Is.True);

            await vm.InitializeViewModelAsync();

            vm.FirstName = "Jane";
            Assert.That(vm.FirstName, Is.EqualTo("Jane"));
            Assert.That(freezable.IsFrozen, Is.False);
        }

        [TestCase]
        public async Task CancelAfterCloseProtectionAsync()
        {
            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var vm = new TestViewModel();

            Assert.That(auditor.OnViewModelCanceledCalled, Is.EqualTo(false));
            Assert.That(auditor.OnViewModelClosedCalled, Is.EqualTo(false));

            await vm.CancelAndCloseViewModelAsync();

            Assert.That(auditor.OnViewModelCanceledCalled, Is.EqualTo(true));
            Assert.That(auditor.OnViewModelClosedCalled, Is.EqualTo(true));

            auditor.OnViewModelCanceledCalled = false;
            auditor.OnViewModelClosedCalled = false;

            await vm.CancelAndCloseViewModelAsync();

            Assert.That(auditor.OnViewModelCanceledCalled, Is.EqualTo(false));
            Assert.That(auditor.OnViewModelClosedCalled, Is.EqualTo(false));
        }

        [TestCase]
        public async Task SaveAfterCloseProtectionAsync()
        {
            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var vm = new TestViewModel();

            Assert.That(auditor.OnViewModelSavedCalled, Is.EqualTo(false));
            Assert.That(auditor.OnViewModelClosedCalled, Is.EqualTo(false));

            await vm.SaveAndCloseViewModelAsync();

            Assert.That(auditor.OnViewModelSavedCalled, Is.EqualTo(true));
            Assert.That(auditor.OnViewModelClosedCalled, Is.EqualTo(true));

            auditor.OnViewModelSavedCalled = false;
            auditor.OnViewModelClosedCalled = false;

            await vm.SaveAndCloseViewModelAsync();

            Assert.That(auditor.OnViewModelSavedCalled, Is.EqualTo(false));
            Assert.That(auditor.OnViewModelClosedCalled, Is.EqualTo(false));
        }

        [TestCase]
        public async Task CloseAfterCloseProtectionAsync()
        {
            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var vm = new TestViewModel();

            Assert.That(auditor.OnViewModelClosedCalled, Is.EqualTo(false));

            await vm.CloseViewModelAsync(null);

            Assert.That(auditor.OnViewModelClosedCalled, Is.EqualTo(true));

            auditor.OnViewModelClosedCalled = false;

            await vm.CloseViewModelAsync(null);

            Assert.That(auditor.OnViewModelClosedCalled, Is.EqualTo(false));
        }

        [Test]
        public async Task MultipleViewModelsCanBeCreatedConcurrentlyAsync()
        {
            const int personsPerThread = 50;
            const int threadAmount = 10;
            var threads = new Thread[threadAmount];

            var allViewModels = new TestViewModel[threadAmount][];

            for (var i = 0; i < threadAmount; i++)
            {
                threads[i] = new Thread(index =>
                {
                    var localViewModels = new TestViewModel[personsPerThread];
                    for (var j = 0; j < personsPerThread; j++)
                    {
                        localViewModels[j] = new TestViewModel();
                    }

                    lock (allViewModels)
                    {
                        allViewModels[(int)index] = localViewModels;
                    }
                });
            }

            for (var i = 0; i < threadAmount; i++)
            {
                threads[i].Start(i);
            }

            await Task.Delay(1000);

            for (var i = 0; i < threadAmount; i++)
            {
                threads[i].Join();
            }

            var flatenSortedIdentifiers =
                allViewModels
                    .SelectMany(o => o)
                    .Select(o => o.UniqueIdentifier)
                    .OrderBy(o => o)
                    .ToArray();

            var firstId = flatenSortedIdentifiers[0];

            Assert.That(flatenSortedIdentifiers, Is.EquivalentTo(Enumerable.Range(firstId, personsPerThread * threadAmount)));
        }

        [Test]
        public async Task PropertiesCanBeSetConcurrentlyWithObjectCreationAsync()
        {
            const int personsPerThread = 50;
            const int threadAmount = 10;
            var threads = new Thread[threadAmount];

            var allViewModels = new TestViewModel[threadAmount][];

            for (int i = 0; i < threadAmount; i++)
            {
                threads[i] = new Thread((index) =>
                {
                    var localViewModels = new TestViewModel[personsPerThread];

                    for (int j = 0; j < personsPerThread; j++)
                    {
                        var viewModel = new TestViewModel
                        {
                            Age = 18
                        };

                        viewModel.Age = 19;

                        localViewModels[j] = viewModel;
                    }

                    allViewModels[(int)index] = localViewModels;
                });
            }

            for (int i = 0; i < threadAmount; i++)
            {
                threads[i].Start(i);
            }

            await Task.Delay(1000);

            for (int i = 0; i < threadAmount; i++)
            {
                threads[i].Join();
            }

            var flatenSortedIdentifiers =
                allViewModels
                    .SelectMany(o => o)
                    .Select(o => o.UniqueIdentifier)
                    .OrderBy(o => o)
                    .ToArray();

            var firstId = flatenSortedIdentifiers[0];

            Assert.That(flatenSortedIdentifiers, Is.EquivalentTo(Enumerable.Range(firstId, personsPerThread * threadAmount)));
        }

        [Test]
        public async Task CommandsCanBeCalledConcurrentlyWithObjectCreationAsync()
        {
            const int personsPerThread = 50;
            const int threadAmount = 10;
            var threads = new Thread[threadAmount];

            var allViewModels = new TestViewModel[threadAmount][];

            for (int i = 0; i < threadAmount; i++)
            {
                threads[i] = new Thread((index) =>
                {
                    var localViewModels = new TestViewModel[personsPerThread];

                    for (int j = 0; j < personsPerThread; j++)
                    {
                        var viewModel = new TestViewModel();
                        viewModel.GenerateData.Execute();
                        localViewModels[j] = viewModel;
                    }

                    allViewModels[(int)index] = localViewModels;
                });
            }

            for (int i = 0; i < threadAmount; i++)
            {
                threads[i].Start(i);
            }

            await Task.Delay(1000);

            for (int i = 0; i < threadAmount; i++)
            {
                threads[i].Join();
            }

            var flattenSortedIdentifiers =
                allViewModels
                    .SelectMany(o => o)
                    .Select(o => o.UniqueIdentifier)
                    .OrderBy(o => o)
                    .ToArray();

            var firstId = flattenSortedIdentifiers[0];

            Assert.That(flattenSortedIdentifiers, Is.EquivalentTo(Enumerable.Range(firstId, personsPerThread * threadAmount)));
        }
    }
}
