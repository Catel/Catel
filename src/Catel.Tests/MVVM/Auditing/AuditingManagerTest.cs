namespace Catel.Tests.MVVM.Auditing
{
    using System;
    using Catel.MVVM.Auditing;
    using NUnit.Framework;

    [TestFixture]
    public class AuditingManagerTest
    {
        [TestCase]
        public void Clear_ValidAuditor()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            Assert.That(AuditingManager.RegisteredAuditorsCount, Is.EqualTo(1));

            AuditingManager.Clear();

            Assert.That(AuditingManager.RegisteredAuditorsCount, Is.EqualTo(0));
        }

        [TestCase]
        public void RegisterAuditor_Null()
        {
            Assert.Throws<ArgumentNullException>(() => AuditingManager.RegisterAuditor(null));
        }

        [TestCase]
        public void RegisterAuditor_ValidAuditor()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            Assert.That(AuditingManager.RegisteredAuditorsCount, Is.EqualTo(1));
        }

        [TestCase]
        public void RegisterAuditor_SameAuditorTwice()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);
            AuditingManager.RegisterAuditor(auditor);

            Assert.That(AuditingManager.RegisteredAuditorsCount, Is.EqualTo(1));
        }

        [TestCase]
        public void UnregisterAuditor_Null()
        {
            Assert.Throws<ArgumentNullException>(() => AuditingManager.UnregisterAuditor(null));
        }

        [TestCase]
        public void UnregisterAuditor_UnregisterExisting()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            Assert.That(AuditingManager.RegisteredAuditorsCount, Is.EqualTo(1));

            AuditingManager.UnregisterAuditor(auditor);

            Assert.That(AuditingManager.RegisteredAuditorsCount, Is.EqualTo(0));
        }

        [TestCase]
        public void UnregisterAuditor_UnregisterNotExisting()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.UnregisterAuditor(auditor);

            Assert.That(AuditingManager.RegisteredAuditorsCount, Is.EqualTo(0));
        }

        [TestCase]
        public void UnregisterAuditor_UnregisterNotExistingWithAnotherRegistered()
        {
            AuditingManager.Clear();

            var auditor1 = new TestAuditor();
            var auditor2 = new TestAuditor();

            AuditingManager.RegisterAuditor(auditor1);

            Assert.That(AuditingManager.RegisteredAuditorsCount, Is.EqualTo(1));

            AuditingManager.UnregisterAuditor(auditor2);

            Assert.That(AuditingManager.RegisteredAuditorsCount, Is.EqualTo(1), "Count should still be 1");
        }
    }
}