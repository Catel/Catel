// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditingManagerTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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

            Assert.AreEqual(1, AuditingManager.RegisteredAuditorsCount);

            AuditingManager.Clear();

            Assert.AreEqual(0, AuditingManager.RegisteredAuditorsCount);
        }

        [TestCase]
        public void RegisterAuditor_Null()
        {
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => AuditingManager.RegisterAuditor(null));
        }

        [TestCase]
        public void RegisterAuditor_ValidAuditor()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            Assert.AreEqual(1, AuditingManager.RegisteredAuditorsCount);
        }

        [TestCase]
        public void RegisterAuditor_SameAuditorTwice()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);
            AuditingManager.RegisterAuditor(auditor);

            Assert.AreEqual(1, AuditingManager.RegisteredAuditorsCount);
        }

        [TestCase]
        public void UnregisterAuditor_Null()
        {
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => AuditingManager.UnregisterAuditor(null));
        }

        [TestCase]
        public void UnregisterAuditor_UnregisterExisting()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            Assert.AreEqual(1, AuditingManager.RegisteredAuditorsCount);

            AuditingManager.UnregisterAuditor(auditor);

            Assert.AreEqual(0, AuditingManager.RegisteredAuditorsCount);
        }

        [TestCase]
        public void UnregisterAuditor_UnregisterNotExisting()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.UnregisterAuditor(auditor);

            Assert.AreEqual(0, AuditingManager.RegisteredAuditorsCount);
        }

        [TestCase]
        public void UnregisterAuditor_UnregisterNotExistingWithAnotherRegistered()
        {
            AuditingManager.Clear();

            var auditor1 = new TestAuditor();
            var auditor2 = new TestAuditor();

            AuditingManager.RegisterAuditor(auditor1);

            Assert.AreEqual(1, AuditingManager.RegisteredAuditorsCount);

            AuditingManager.UnregisterAuditor(auditor2);

            Assert.AreEqual(1, AuditingManager.RegisteredAuditorsCount, "Count should still be 1");
        }
    }
}