// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Extensions.EntityFramework5.Data
{
    using Catel.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Test.EntityFramework5.DbContextTest;

    public class DbContextManagerFacts
    {
        [TestClass]
        public class ScopingTest
        {
            [TestMethod]
            public void SingleLevelScoping()
            {
                DbContextManager<TestDbContextContainer> manager = null;

                using (manager = DbContextManager<TestDbContextContainer>.GetManager())
                {
                    Assert.AreEqual(1, manager.RefCount);
                }

                Assert.AreEqual(0, manager.RefCount);
            }

            [TestMethod]
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