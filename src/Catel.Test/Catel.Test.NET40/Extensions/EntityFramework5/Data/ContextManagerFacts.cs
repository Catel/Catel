// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Extensions.EntityFramework5.Data
{
    using Catel.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Test.EntityFramework5.DbContextTest;
    using Test.EntityFramework5.ObjectContextTest;

    public class ContextManagerFacts
    {
        [TestClass]
        public class TheTypeInstantiation
        {
            [TestMethod]
            public void WorksForDbContext()
            {
                using (var manager = DbContextManager<TestDbContextContainer>.GetManager())
                {
                    Assert.IsNotNull(manager);
                }
            }

            [TestMethod]
            public void WorksForObjectContext()
            {
                using (var manager = ObjectContextManager<TestObjectContextContainer>.GetManager())
                {
                    Assert.IsNotNull(manager);
                }
            }
        }
    }
}