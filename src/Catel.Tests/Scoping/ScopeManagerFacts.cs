namespace Catel.Tests.Scoping
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
                Assert.Throws<ArgumentNullException>(() => ScopeManager<string>.ScopeExists(null));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingScope()
            {
                Assert.That(ScopeManager<string>.ScopeExists(), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForExistingScope()
            {
                Assert.That(ScopeManager<string>.ScopeExists(), Is.False);

                using (var scopeManager = ScopeManager<string>.GetScopeManager(createScopeFunction: () => string.Empty))
                {
                    Assert.That(ScopeManager<string>.ScopeExists(), Is.True);
                }

                Assert.That(ScopeManager<string>.ScopeExists(), Is.False);
            }
        }

        [TestFixture]
        public class ScopingTest
        {
            [TestCase]
            public void SingleLevelScoping()
            {
                ScopeManager<object> scopeManager = null;

                using (scopeManager = ScopeManager<object>.GetScopeManager("object"))
                {
                    Assert.That(scopeManager.RefCount, Is.EqualTo(1));
                }

                Assert.That(scopeManager.RefCount, Is.EqualTo(0));
            }

            [TestCase]
            public void MultipleLevelScoping()
            {
                ScopeManager<object> scopeManager = null;

                using (scopeManager = ScopeManager<object>.GetScopeManager("object"))
                {
                    Assert.That(scopeManager.RefCount, Is.EqualTo(1));

                    using (ScopeManager<object>.GetScopeManager("object"))
                    {
                        Assert.That(scopeManager.RefCount, Is.EqualTo(2));

                        using (ScopeManager<object>.GetScopeManager("object"))
                        {
                            Assert.That(scopeManager.RefCount, Is.EqualTo(3));
                        }

                        Assert.That(scopeManager.RefCount, Is.EqualTo(2));
                    }

                    Assert.That(scopeManager.RefCount, Is.EqualTo(1));
                }

                Assert.That(scopeManager.RefCount, Is.EqualTo(0));
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
                        Assert.That(scope2Manager.ScopeObject, Is.EqualTo(obj1));
                        Assert.That(scope2Manager.RefCount, Is.EqualTo(2));
                    }
                }
            }
        }
    }
}
