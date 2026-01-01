namespace Catel.Tests.MVVM.Auditing
{
    using System;
    using Catel.MVVM.Auditing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class AuditingManagerTest
    {
        [TestCase]
        public void Clear_ValidAuditor()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            Assert.That(auditingManager.RegisteredAuditorsCount, Is.EqualTo(1));

            auditingManager.Clear();

            Assert.That(auditingManager.RegisteredAuditorsCount, Is.EqualTo(0));
        }

        [TestCase]
        public void RegisterAuditor_Null()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            Assert.Throws<ArgumentNullException>(() => auditingManager.RegisterAuditor(null));
        }

        [TestCase]
        public void RegisterAuditor_ValidAuditor()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            Assert.That(auditingManager.RegisteredAuditorsCount, Is.EqualTo(1));
        }

        [TestCase]
        public void RegisterAuditor_SameAuditorTwice()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);
            auditingManager.RegisterAuditor(auditor);

            Assert.That(auditingManager.RegisteredAuditorsCount, Is.EqualTo(1));
        }

        [TestCase]
        public void UnregisterAuditor_Null()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            Assert.Throws<ArgumentNullException>(() => auditingManager.UnregisterAuditor(null));
        }

        [TestCase]
        public void UnregisterAuditor_UnregisterExisting()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.RegisterAuditor(auditor);

            Assert.That(auditingManager.RegisteredAuditorsCount, Is.EqualTo(1));

            auditingManager.UnregisterAuditor(auditor);

            Assert.That(auditingManager.RegisteredAuditorsCount, Is.EqualTo(0));
        }

        [TestCase]
        public void UnregisterAuditor_UnregisterNotExisting()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor = new TestAuditor();
            auditingManager.UnregisterAuditor(auditor);

            Assert.That(auditingManager.RegisteredAuditorsCount, Is.EqualTo(0));
        }

        [TestCase]
        public void UnregisterAuditor_UnregisterNotExistingWithAnotherRegistered()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var auditingManager = new AuditingManager(serviceProvider);

            var auditor1 = new TestAuditor();
            var auditor2 = new TestAuditor();

            auditingManager.RegisterAuditor(auditor1);

            Assert.That(auditingManager.RegisteredAuditorsCount, Is.EqualTo(1));

            auditingManager.UnregisterAuditor(auditor2);

            Assert.That(auditingManager.RegisteredAuditorsCount, Is.EqualTo(1), "Count should still be 1");
        }
    }
}
