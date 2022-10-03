// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBaseFacts.mappings.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.MVVM.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Catel.Data;
    using Catel.MVVM;
    using NUnit.Framework;
    using TestClasses;

    public partial class ViewModelBaseFacts
    {
        [TestCase]
        public void ViewModelBase_ActionsTimeout_SetsDefaultValue()
        {
            var vm = new TestViewModelWithActionTimeout();

            Assert.AreEqual(IViewModelExtensions.ViewModelActionAwaitTimeoutInMilliseconds, vm.ViewModelActionAwaitTimeoutInMilliseconds);
        }

        [Test]
        public async Task ViewModelBase_ActionsTimeout_ExpectedException_SaveAsync()
        {
            var vm = new TestViewModelWithActionTimeout();

            await vm.InitializeViewModelAsync();

            vm.ViewModelActionAwaitTimeoutInMilliseconds = 70;
            vm.ActionDuration = 1000;

            // Need duplicate call
            _ = vm.SaveAndCloseViewModelAsync();
            Assert.ThrowsAsync<TimeoutException>(async () => await vm.SaveAndCloseViewModelAsync());
        }

        [TestCase]
        public async Task ViewModelBase_ActionsTimeout_SaveAsync()
        {
            var vm = new TestViewModelWithActionTimeout();

            await vm.InitializeViewModelAsync();

            vm.ViewModelActionAwaitTimeoutInMilliseconds = 70;
            vm.ActionDuration = 40;

            // Need duplicate call
            _ = vm.SaveAndCloseViewModelAsync();
            await vm.SaveAndCloseViewModelAsync();
        }

        [Test]
        public async Task ViewModelBase_ActionsTimeout_ExpectedException_CancelAsync()
        {
            var vm = new TestViewModelWithActionTimeout();

            await vm.InitializeViewModelAsync();

            vm.ViewModelActionAwaitTimeoutInMilliseconds = 70;
            vm.ActionDuration = 1000;

            // Need duplicate call
            _ = vm.CancelAndCloseViewModelAsync();
            Assert.ThrowsAsync<TimeoutException>(async () => await vm.CancelAndCloseViewModelAsync());
        }

        [TestCase]
        public async Task ViewModelBase_ActionsTimeout_CancelAsync()
        {
            var vm = new TestViewModelWithActionTimeout();

            await vm.InitializeViewModelAsync();

            vm.ViewModelActionAwaitTimeoutInMilliseconds = 70;
            vm.ActionDuration = 50;

            // Need duplicate call
            _ = vm.CancelAndCloseViewModelAsync();
            await vm.CancelAndCloseViewModelAsync();
        }
    }
}
