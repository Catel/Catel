// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Scoping
{
    using System;
    using Catel.Scoping;
    using NUnit.Framework;

    internal class ScopeManagerFacts
    {
        [TestFixture]
        public class TheScopeExistsMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullScopeName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ScopeManager<string>.ScopeExists(null));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingScope()
            {
                Assert.IsFalse(ScopeManager<string>.ScopeExists());
            }

            [TestCase]
            public void ReturnsTrueForExistignScope()
            {
                Assert.IsFalse(ScopeManager<string>.ScopeExists());

                using (var scopeManager = ScopeManager<string>.GetScopeManager())
                {
                    Assert.IsTrue(ScopeManager<string>.ScopeExists());    
                }

                Assert.IsFalse(ScopeManager<string>.ScopeExists());
            }
        }

        #region Nested type: ScopingTest
        [TestFixture]
        public class ScopingTest
        {
            #region Methods
            [TestCase]
            public void SingleLevelScoping()
            {
                ScopeManager<object> scopeManager = null;

                using (scopeManager = ScopeManager<object>.GetScopeManager("object"))
                {
                    Assert.AreEqual(1, scopeManager.RefCount);
                }

                Assert.AreEqual(0, scopeManager.RefCount);
            }

            [TestCase]
            public void MultipleLevelScoping()
            {
                ScopeManager<object> scopeManager = null;

                using (scopeManager = ScopeManager<object>.GetScopeManager("object"))
                {
                    Assert.AreEqual(1, scopeManager.RefCount);

                    using (ScopeManager<object>.GetScopeManager("object"))
                    {
                        Assert.AreEqual(2, scopeManager.RefCount);

                        using (ScopeManager<object>.GetScopeManager("object"))
                        {
                            Assert.AreEqual(3, scopeManager.RefCount);
                        }

                        Assert.AreEqual(2, scopeManager.RefCount);
                    }

                    Assert.AreEqual(1, scopeManager.RefCount);
                }

                Assert.AreEqual(0, scopeManager.RefCount);
            }

            [TestCase]
            public void CustomScopeCreationTest()
            {
                var obj1 = "15";
                var obj2 = "16";

                using (var scope1Manager = ScopeManager<string>.GetScopeManager(createScopeFunction: () => obj1))
                {
                    using (var scope2Manager = ScopeManager<string>.GetScopeManager(createScopeFunction: () => obj2))
                    {
                        Assert.AreEqual(obj1, scope2Manager.ScopeObject);
                        Assert.AreEqual(2, scope2Manager.RefCount);
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}