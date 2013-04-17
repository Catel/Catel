// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Extensions.EntityFramework5.Data
{
    using Catel.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Test.EntityFramework5.DbContextTest;

    public class ConnectionStringManagerFacts
    {
        [TestClass]
        public class TheGetConnectionStringMethod
        {
            [TestMethod]
            public void ReturnsNullByDefault()
            {
                var connectionStringManager = new ConnectionStringManager();

                Assert.AreEqual(null, connectionStringManager.GetConnectionString(typeof(TestDbContextContainer), null, null));
            }
        }
    }
}