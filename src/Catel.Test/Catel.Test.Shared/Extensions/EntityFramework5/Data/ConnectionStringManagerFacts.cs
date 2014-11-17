// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.EntityFramework5.Data
{
    using Catel.Data;
    using NUnit.Framework;
    using Test.EntityFramework5.DbContextTest;

    public class ConnectionStringManagerFacts
    {
        [TestFixture]
        public class TheGetConnectionStringMethod
        {
            [TestCase]
            public void ReturnsNullByDefault()
            {
                var connectionStringManager = new ConnectionStringManager();

                Assert.AreEqual(null, connectionStringManager.GetConnectionString(typeof(TestDbContextContainer), null, null));
            }
        }
    }
}

#endif