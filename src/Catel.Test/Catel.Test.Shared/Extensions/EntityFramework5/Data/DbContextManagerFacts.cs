// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.EntityFramework5.Data
{
    using Catel.Data;
    using NUnit.Framework;
    using Test.EntityFramework5.DbContextTest;

    public class DbContextManagerFacts
    {
        [TestFixture]
        public class ScopingTest
        {
            [TestCase]
            public void SingleLevelScoping()
            {
                DbContextManager<TestDbContextContainer> manager = null;

                using (manager = DbContextManager<TestDbContextContainer>.GetManager())
                {
                    Assert.AreEqual(1, manager.RefCount);
                }

                Assert.AreEqual(0, manager.RefCount);
            }

            [TestCase]
            public void MultipleLevelScoping()
            {
                DbContextManager<TestDbContextContainer> manager = null;

                using (manager = DbContextManager<TestDbContextContainer>.GetManager())
                {
                    Assert.AreEqual(1, manager.RefCount);

                    using (DbContextManager<TestDbContextContainer>.GetManager())
                    {
                        Assert.AreEqual(2, manager.RefCount);

                        using (DbContextManager<TestDbContextContainer>.GetManager())
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