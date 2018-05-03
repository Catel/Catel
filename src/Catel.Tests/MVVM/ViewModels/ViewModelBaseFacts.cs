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
        public void MultipleViewModelsCanBeCreatedConcurrently()
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
                        allViewModels[(int) index] = localViewModels;
                    }
                });
            }

            for (var i = 0; i < threadAmount; i++)
            {
                threads[i].Start(i);
            }

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
        public void PropertiesCanBeSetConcurrentlyWithObjectCreation()
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
                        viewModel.Age = 18;
                        viewModel.Age = 19;
                        localViewModels[j] = viewModel;
                    }
                    allViewModels[(int) index] = localViewModels;
                });
            }

            for (int i = 0; i < threadAmount; i++)
            {
                threads[i].Start(i);
            }

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
        public void CommandsCanBeCalledConcurrentlyWithObjectCreation()
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
                    allViewModels[(int) index] = localViewModels;
                });
            }

            for (int i = 0; i < threadAmount; i++)
            {
                threads[i].Start(i);
            }

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
    }
}