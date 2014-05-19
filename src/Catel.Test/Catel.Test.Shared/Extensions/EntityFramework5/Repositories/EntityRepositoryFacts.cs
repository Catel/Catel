// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepositoryFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.EntityFramework5.Repositories
{
    using System;
    using System.Linq;

    using Catel.Test.EntityFramework5.DbContextTest;
    using Catel.Test.EntityFramework5.DbContextTest.Repositories;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class EntityRepositoryFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new DbContextOrderRepository(null));
            }
        }

        [TestClass]
        public class TheGetByKeyMethod
        {
            [TestMethod]
            public void ReturnsNullIfKeyIsInvalid()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        Assert.IsNull(repository.GetByKey(12345));
                    }
                }
            }

            [TestMethod]
            public void ReturnsEntityIfKeyIsValid()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(42);

                        var existingCustomer = repository.GetByKey(42);

                        Assert.IsNotNull(existingCustomer);
                    }
                }
            }
        }

        [TestClass]
        public class TheSingleMethod
        {
            [TestMethod]
            public void ThrowsExceptionWhenTableDoesNotContainEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => repository.Single(x => x.Id == 999));
                    }
                }
            }

            [TestMethod]
            public void ReturnsEntityWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(1);

                        var customer = repository.Single(x => x.Id == 1);

                        Assert.IsNotNull(customer);
                        Assert.AreEqual(1, customer.Id);
                    }
                }
            }
        }

        [TestClass]
        public class TheSingleOrDefaultMethod
        {
            [TestMethod]
            public void ReturnsNullWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        var customer = repository.SingleOrDefault(x => x.Id == 999);
                        Assert.IsNull(customer);
                    }
                }
            }

            [TestMethod]
            public void ReturnsEntityWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(1);

                        var customer = repository.SingleOrDefault(x => x.Id == 1);

                        Assert.IsNotNull(customer);
                        Assert.AreEqual(1, customer.Id);
                    }
                }
            }
        }

        [TestClass]
        public class TheFirstMethod
        {
            [TestMethod]
            public void ThrowsExceptionWhenTableDoesNotContainEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => repository.First(x => x.Id == 999));
                    }
                }
            }

            [TestMethod]
            public void ReturnsEntityWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(1);

                        var customer = repository.First();

                        Assert.IsNotNull(customer);
                        Assert.AreEqual(1, customer.Id);
                    }
                }
            }
        }

        [TestClass]
        public class TheFirstOrDefaultMethod
        {
            [TestMethod]
            public void ReturnsNullWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        var customer = repository.FirstOrDefault(x => x.Id == 999);
                        Assert.IsNull(customer);
                    }
                }
            }

            [TestMethod]
            public void ReturnsEntityWhenTableContainsEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(1);

                        var customer = repository.FirstOrDefault();

                        Assert.IsNotNull(customer);
                        Assert.AreEqual(1, customer.Id);
                    }
                }
            }
        }

        [TestClass]
        public class TheAddMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => repository.Add(null));
                    }
                }  
            }

            [TestMethod]
            public void AddsNonExistingEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        var customer = EFTestHelper.CreateCustomer(1234);

                        repository.Add(customer);

                        dbContext.SaveChanges();

                        var fetchedCustomer = repository.GetByKey(1234);
                        Assert.AreEqual(customer, fetchedCustomer);
                    }
                }  
            }
        }

        [TestClass]
        public class TheAttachMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => repository.Attach(null));
                    }
                }
            }

            [TestMethod]
            public void AddsNonExistingEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        var customer = EFTestHelper.CreateCustomer(1235);

                        repository.Attach(customer);

                        dbContext.SaveChanges();

                        var fetchedCustomer = repository.GetByKey(1235);
                        Assert.AreEqual(customer, fetchedCustomer);
                    }
                }
            }
        }

        [TestClass]
        public class TheDeleteMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => repository.Delete((DbContextCustomer)null));
                    }
                }
            }

            [TestMethod]
            public void DeletesSpecificEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(201);

                        var customer1 = repository.GetByKey(201);

                        Assert.IsNotNull(customer1);

                        repository.Delete(customer1);

                        dbContext.SaveChanges();

                        var customer2 = repository.GetByKey(201);

                        Assert.IsNull(customer2);
                    }
                }
            }

            [TestMethod]
            public void SucceedsWhenNoEntitiesMatchFilter()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        repository.Delete(x => x.Id == 999);

                        dbContext.SaveChanges();
                    }
                }
            }

            [TestMethod]
            public void SucceedsWhenEntitiesMatchFilter()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(201);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(202);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(203);

                        repository.Delete(x => x.Id >= 201 && x.Id <= 203);

                        dbContext.SaveChanges();

                        Assert.IsNull(repository.GetByKey(201));
                        Assert.IsNull(repository.GetByKey(202));
                        Assert.IsNull(repository.GetByKey(203));
                    }
                }
            }
        }

        [TestClass]
        public class TheUpdateMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => repository.Update(null));
                    }
                }
            }

            [TestMethod]
            public void UpdatesEntity()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(301);

                        var customer1 = repository.GetByKey(301);
                        customer1.Name = "John Doe";

                        repository.Update(customer1);

                        dbContext.SaveChanges();

                        var customer2 = repository.GetByKey(301);

                        Assert.IsNotNull(customer2);
                        Assert.AreEqual("John Doe", customer2.Name);
                    }
                }
            }
        }

        [TestClass]
        public class TheFindMethod
        {
            [TestMethod]
            public void ReturnsCorrectEntities()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(100);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(101);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(102);

                        var customers = repository.Find(x => x.Id >= 100 && x.Id <= 102).ToList();

                        Assert.AreEqual(3, customers.Count);
                    }
                }
            }
        }

        [TestClass]
        public class TheGetAllMethod
        {
            [TestMethod]
            public void ReturnsCorrectEntities()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(100);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(101);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(102);

                        var customers = repository.GetAll().ToList();

                        Assert.IsTrue(customers.Count >= 3);
                    }
                }
            }
        }

        [TestClass]
        public class TheCountMethod
        {
            [TestMethod]
            public void ReturnsCorrectEntityCount()
            {
                using (var dbContext = new TestDbContextContainer())
                {
                    using (var repository = new DbContextCustomerRepository(dbContext))
                    {
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(100);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(101);
                        EFTestHelper.CreateCustomerIfNotAlreadyExists(102);

                        var customerCount = repository.Count(x => x.Id >= 100 && x.Id <= 102);

                        Assert.AreEqual(3, customerCount);
                    }
                }
            }
        }
    }
}

#endif