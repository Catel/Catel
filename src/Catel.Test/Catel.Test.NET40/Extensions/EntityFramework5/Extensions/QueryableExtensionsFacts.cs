// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryableExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Extensions.EntityFramework5.Extensions
{
    using System;
    using System.Linq;
    using Catel.Data;
    using Catel.Test.EntityFramework5.DbContextTest;
    using Catel.Test.EntityFramework5.DbContextTest.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class QueryableExtensionsFacts
    {
        [TestClass]
        public class TheIncludeMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullQueryable()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => QueryableExtensions.Include<DbContextCustomer>(null, x => x.DbContextOrders));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullExpression()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => repository.GetAll().Include(null));
                    }
                }
            }

            [TestMethod]
            public void IncludesEntitiesUsingExpression()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(42);

                        var existingCustomer = repository.GetAll().Include(x => x.DbContextOrders).FirstOrDefault();

                        Assert.IsNotNull(existingCustomer);
                    }
                }
            }
        }
    }
}