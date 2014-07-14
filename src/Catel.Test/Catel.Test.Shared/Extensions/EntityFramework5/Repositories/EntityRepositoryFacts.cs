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

    using NUnit.Framework;

    public class EntityRepositoryFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new DbContextOrderRepository(null));
            }
        }

        [TestFixture]
        public class TheGetByKeyMethod
        {
            [TestCase]
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

            [TestCase]
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

        [TestFixture]
        public class TheSingleMethod
        {
            [TestCase]
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

            [TestCase]
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

        [TestFixture]
        public class TheSingleOrDefaultMethod
        {
            [TestCase]
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

            [TestCase]
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

        [TestFixture]
        public class TheFirstMethod
        {
            [TestCase]
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

            [TestCase]
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

        [TestFixture]
        public class TheFirstOrDefaultMethod
        {
            [TestCase]
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

            [TestCase]
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

        [TestFixture]
        public class TheAddMethod
        {
            [TestCase]
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

            [TestCase]
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

        [TestFixture]
        public class TheAttachMethod
        {
            [TestCase]
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

            [TestCase]
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

        [TestFixture]
        public class TheDeleteMethod
        {
            [TestCase]
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

            [TestCase]
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

            [TestCase]
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

            [TestCase]
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

        [TestFixture]
        public class TheUpdateMethod
        {
            [TestCase]
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

            [TestCase]
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

        [TestFixture]
        public class TheFindMethod
        {
            [TestCase]
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

        [TestFixture]
        public class TheGetAllMethod
        {
            [TestCase]
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

        [TestFixture]
        public class TheCountMethod
        {
            [TestCase]
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