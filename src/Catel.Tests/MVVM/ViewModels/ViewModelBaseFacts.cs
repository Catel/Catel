// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBaseFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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

            Assert.IsFalse(vm.IsDirty);
        }

        [TestCase]
        public async Task ProtectPropertiesAfterClosingAsync()
        {
            var vm = new TestViewModel();
            var freezable = (IFreezable)vm;

            await vm.InitializeViewModelAsync();

            vm.FirstName = "John";
            Assert.AreEqual("John", vm.FirstName);
            Assert.IsFalse(freezable.IsFrozen);

            await vm.SaveAndCloseViewModelAsync();

            vm.FirstName = "Jane";
            Assert.AreEqual("John", vm.FirstName);
            Assert.IsTrue(freezable.IsFrozen);

            await vm.InitializeViewModelAsync();

            vm.FirstName = "Jane";
            Assert.AreEqual("Jane", vm.FirstName);
            Assert.IsFalse(freezable.IsFrozen);
        }

        [TestCase]
        public async Task CancelAfterCloseProtection()
        {
            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var vm = new TestViewModel();

            Assert.AreEqual(false, auditor.OnViewModelCanceledCalled);
            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);

            await vm.CancelAndCloseViewModelAsync();

            Assert.AreEqual(true, auditor.OnViewModelCanceledCalled);
            Assert.AreEqual(true, auditor.OnViewModelClosedCalled);

            auditor.OnViewModelCanceledCalled = false;
            auditor.OnViewModelClosedCalled = false;

            await vm.CancelAndCloseViewModelAsync();

            Assert.AreEqual(false, auditor.OnViewModelCanceledCalled);
            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);
        }

        [TestCase]
        public async Task SaveAfterCloseProtection()
        {
            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var vm = new TestViewModel();

            Assert.AreEqual(false, auditor.OnViewModelSavedCalled);
            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);

            await vm.SaveAndCloseViewModelAsync();

            Assert.AreEqual(true, auditor.OnViewModelSavedCalled);
            Assert.AreEqual(true, auditor.OnViewModelClosedCalled);

            auditor.OnViewModelSavedCalled = false;
            auditor.OnViewModelClosedCalled = false;

            await vm.SaveAndCloseViewModelAsync();

            Assert.AreEqual(false, auditor.OnViewModelSavedCalled);
            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);
        }

        [TestCase]
        public async Task CloseAfterCloseProtection()
        {
            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var vm = new TestViewModel();

            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);

            await vm.CloseViewModelAsync(null);

            Assert.AreEqual(true, auditor.OnViewModelClosedCalled);

            auditor.OnViewModelClosedCalled = false;

            await vm.CloseViewModelAsync(null);

            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);
        }

        [Test]
        public async Task MultipleViewModelsCanBeCreatedConcurrently()
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
        public async Task PropertiesCanBeSetConcurrentlyWithObjectCreation()
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
        public async Task CommandsCanBeCalledConcurrentlyWithObjectCreation()
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
