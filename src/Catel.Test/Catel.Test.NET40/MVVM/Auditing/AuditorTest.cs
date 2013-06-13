// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditorTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Auditing
{
    using Catel.MVVM.Auditing;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class AuditorTest
    {
        [TestMethod]
        public void OnViewModelCreating()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();

            Assert.AreEqual(true, auditor.OnViewModelCreatingCalled);
            Assert.AreEqual(typeof (TestViewModel), auditor.OnViewModelCreatingType);
        }

        [TestMethod]
        public void OnViewModelCreated()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();

            Assert.AreEqual(true, auditor.OnViewModelCreatedCalled);
            Assert.AreEqual(typeof (TestViewModel), auditor.OnViewModelCreatedType);
        }

#if NET
        [TestMethod]
        public void OnPropertyChanging()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();
            viewModel.TestProperty = "test";

            Assert.AreEqual(true, auditor.OnPropertyChangingCalled);
            Assert.AreEqual(viewModel, auditor.OnPropertyChangingViewModel);
            Assert.AreEqual("TestProperty", auditor.OnPropertyChangingPropertyName);
            Assert.AreEqual("defaultvalue", auditor.OnPropertyChangingOldValue);
        }
#endif

        [TestMethod]
        public void OnPropertyChanged()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();
            viewModel.TestProperty = "test";

            Assert.AreEqual(true, auditor.OnPropertyChangedCalled);
            Assert.AreEqual(viewModel, auditor.OnPropertyChangedViewModel);
            Assert.AreEqual("TestProperty", auditor.OnPropertyChangedPropertyName);
            Assert.AreEqual("test", auditor.OnPropertyChangedNewValue);
        }

        [TestMethod]
        public void OnPropertyChanged_IgnoredProperties()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            auditor.PropertiesToIgnore.Add("TestProperty");
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();
            viewModel.TestProperty = "test";

            Assert.AreEqual(false, auditor.OnPropertyChangedCalled);
            Assert.AreEqual(null, auditor.OnPropertyChangedViewModel);
            Assert.AreEqual(null, auditor.OnPropertyChangedPropertyName);
            Assert.AreEqual(null, auditor.OnPropertyChangedNewValue);
        }

        [TestMethod]
        public void OnCommandExecuted()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();
            viewModel.TestCommand.Execute("test");

            Assert.AreEqual(true, auditor.OnCommandExecutedCalled);
            Assert.AreEqual(viewModel, auditor.OnCommandExecutedViewModel);
            Assert.AreEqual("TestCommand", auditor.OnCommandExecutedCommandName);
            Assert.AreEqual(viewModel.TestCommand, auditor.OnCommandExecutedCommand);
            Assert.AreEqual("test", auditor.OnCommandExecutedCommandParameter);
        }

        [TestMethod]
        public void OnViewModelSaving()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();
            viewModel.SaveViewModel();

            Assert.AreEqual(true, auditor.OnViewModelSavingCalled);
            Assert.AreEqual(viewModel, auditor.OnViewModelSavingViewModel);
        }

        [TestMethod]
        public void OnViewModelSaved()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();
            viewModel.SaveViewModel();

            Assert.AreEqual(true, auditor.OnViewModelSavedCalled);
            Assert.AreEqual(viewModel, auditor.OnViewModelSavedViewModel);
        }

        [TestMethod]
        public void OnViewModelCanceling()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();
            viewModel.CancelViewModel();

            Assert.AreEqual(true, auditor.OnViewModelCancelingCalled);
            Assert.AreEqual(viewModel, auditor.OnViewModelCancelingViewModel);
        }

        [TestMethod]
        public void OnViewModelCanceled()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();
            viewModel.CancelViewModel();

            Assert.AreEqual(true, auditor.OnViewModelCanceledCalled);
            Assert.AreEqual(viewModel, auditor.OnViewModelCanceledViewModel);
        }

        [TestMethod]
        public void OnViewModelClosing()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();
            viewModel.CloseViewModel(null);

            Assert.AreEqual(true, auditor.OnViewModelClosingCalled);
            Assert.AreEqual(viewModel, auditor.OnViewModelClosingViewModel);
        }

        [TestMethod]
        public void OnViewModelClosed()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var viewModel = new TestViewModel();
            viewModel.CloseViewModel(null);

            Assert.AreEqual(true, auditor.OnViewModelClosedCalled);
            Assert.AreEqual(viewModel, auditor.OnViewModelClosedViewModel);
        }
    }
}