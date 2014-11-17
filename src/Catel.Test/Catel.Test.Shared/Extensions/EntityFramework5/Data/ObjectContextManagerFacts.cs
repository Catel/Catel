// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectContextManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.EntityFramework5.Data
{
    using Catel.Data;
    using NUnit.Framework;
    using Test.EntityFramework5.ObjectContextTest;

    public class ObjectContextManagerFacts
    {
        [TestFixture]
        public class ScopingTest
        {
            [TestCase]
            public void SingleLevelScoping()
            {
                ObjectContextManager<TestObjectContextContainer> manager = null;

                using (manager = ObjectContextManager<TestObjectContextContainer>.GetManager())
                {
                    Assert.AreEqual(1, manager.RefCount);
                }

                Assert.AreEqual(0, manager.RefCount);
            }

            [TestCase]
            public void MultipleLevelScoping()
            {
                ObjectContextManager<TestObjectContextContainer> manager = null;

                using (manager = ObjectContextManager<TestObjectContextContainer>.GetManager())
                {
                    Assert.AreEqual(1, manager.RefCount);

                    using (ObjectContextManager<TestObjectContextContainer>.GetManager())
                    {
                        Assert.AreEqual(2, manager.RefCount);

                        using (ObjectContextManager<TestObjectContextContainer>.GetManager())
                        {
                            Assert.AreEqual(3, manager.RefCount);
                        }

                        Assert.AreEqual(2, manager.RefCount);
                    }

                    Assert.AreEqual(1, manager.RefCount);
                }

                Assert.AreEqual(0, manager.RefCount);
            }
        }
    }
}

#endif