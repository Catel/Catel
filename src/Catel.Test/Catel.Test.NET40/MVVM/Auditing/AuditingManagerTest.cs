// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditingManagerTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Auditing
{
    using System;
    using Catel.MVVM.Auditing;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class AuditingManagerTest
    {
        [TestMethod]
        public void Clear_ValidAuditor()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            Assert.AreEqual(1, AuditingManager.RegisteredAuditorsCount);

            AuditingManager.Clear();

            Assert.AreEqual(0, AuditingManager.RegisteredAuditorsCount);
        }

        [TestMethod]
        public void RegisterAuditor_Null()
        {
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => AuditingManager.RegisterAuditor(null));
        }

        [TestMethod]
        public void RegisterAuditor_ValidAuditor()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            Assert.AreEqual(1, AuditingManager.RegisteredAuditorsCount);
        }

        [TestMethod]
        public void RegisterAuditor_SameAuditorTwice()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);
            AuditingManager.RegisterAuditor(auditor);

            Assert.AreEqual(1, AuditingManager.RegisteredAuditorsCount);
        }

        [TestMethod]
        public void UnregisterAuditor_Null()
        {
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => AuditingManager.UnregisterAuditor(null));
        }

        [TestMethod]
        public void UnregisterAuditor_UnregisterExisting()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            Assert.AreEqual(1, AuditingManager.RegisteredAuditorsCount);

            AuditingManager.UnregisterAuditor(auditor);

            Assert.AreEqual(0, AuditingManager.RegisteredAuditorsCount);
        }

        [TestMethod]
        public void UnregisterAuditor_UnregisterNotExisting()
        {
            AuditingManager.Clear();

            var auditor = new TestAuditor();
            AuditingManager.UnregisterAuditor(auditor);

            Assert.AreEqual(0, AuditingManager.RegisteredAuditorsCount);
        }

        [TestMethod]
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