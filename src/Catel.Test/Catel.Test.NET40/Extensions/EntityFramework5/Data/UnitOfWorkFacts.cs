// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitOfWorkFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Extensions.EntityFramework5.Data
{
    using System;

    using Catel.Data;
    using Catel.Test.EntityFramework5.DbContextTest;
    using Catel.Test.EntityFramework5.DbContextTest.Repositories;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class UnitOfWorkFacts
    {
        [TestClass]
        public class TheIsInTransactionProperty
        {
            [TestMethod]
            public void ReturnsTrueWhenInTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    Assert.IsFalse(uow.IsInTransaction);

                    uow.BeginTransaction();

                    Assert.IsTrue(uow.IsInTransaction);

                    uow.CommitTransaction();

                    Assert.IsFalse(uow.IsInTransaction);
                }
            }
        }

        [TestClass]
        public class TheBeginTransactionMethod
        {
            [TestMethod]
            public void ThrowsInvalidOperationExceptionWhenCalledWhenAlreadyInTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    uow.BeginTransaction();

                    ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => uow.BeginTransaction());
                }
            }
        }

        [TestClass]
        public class TheRollbackTransactionMethod
        {
            [TestMethod]
            public void ThrowsInvalidOperationExceptionWhenCalledWhenNotInTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => uow.RollBackTransaction());
                }
            }

            // TODO: Check if this item can correctly rollback transactions
        }

        [TestClass]
        public class TheCommitTransactionMethod
        {
            [TestMethod]
            public void ThrowsInvalidOperationExceptionWhenCalledWhenNotInTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => uow.CommitTransaction());
                }
            }

            [TestMethod]
            public void CorrectlyCommitsTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    var customerRepository = uow.GetRepository<IDbContextCustomerRepository>();
                    var productRepository = uow.GetRepository<IDbContextProductRepository>();
                    var orderRepository = uow.GetRepository<IDbContextOrderRepository>();

                    uow.BeginTransaction();

                    var customer = EFTestHelper.CreateCustomer(451);
                    customerRepository.Add(customer);

                    var product = EFTestHelper.CreateProduct(451);
                    productRepository.Add(product);

                    var order = new DbContextOrder { OrderCreated = DateTime.Now, Amount = 1, CustomerId = 451, ProductId = 451 };
                    orderRepository.Add(order);

                    uow.CommitTransaction();
                }

                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    var customerRepository = uow.GetRepository<IDbContextCustomerRepository>();
                    var productRepository = uow.GetRepository<IDbContextProductRepository>();
                    var orderRepository = uow.GetRepository<IDbContextOrderRepository>();

                    var customer = customerRepository.GetByKey(451);
                    Assert.IsNotNull(customer);

                    var product = productRepository.GetByKey(451);
                    Assert.IsNotNull(product);

                    var order = orderRepository.FirstOrDefault(x => x.CustomerId == 451 && x.ProductId == 451);
                    Assert.IsNotNull(order);
                }
            }

            [TestMethod]
            public void CorrectlyRollbacksTransactionWhenAnErrorOccursWhileSaving()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    var orderRepository = uow.GetRepository<IDbContextOrderRepository>();

                    uow.BeginTransaction();

                    var order = new DbContextOrder { Amount = 1, CustomerId = 999, ProductId = 999 };
                    orderRepository.Add(order);

                    try
                    {
                        uow.CommitTransaction();

                        Assert.Fail("Expected an exception");
                    }
                    catch (Exception)
                    {
                        Assert.IsFalse(uow.IsInTransaction);
                    }
                }
            }
        }

        [TestClass]
        public class TheSaveChangesMethod
        {
            [TestMethod]
            public void ThrowsInvalidOperationExceptionWhenCalledInsideTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    uow.BeginTransaction();

                    ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => uow.SaveChanges());
                }
            }

            [TestMethod]
            public void CorrectlySavesChangesWhenNotInTransaction()
            {
                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    var customerRepository = uow.GetRepository<IDbContextCustomerRepository>();

                    var customer = EFTestHelper.CreateCustomer(401);
                    customerRepository.Add(customer);

                    uow.SaveChanges();
                }

                using (var uow = new UnitOfWork<TestDbContextContainer>())
                {
                    var customerRepository = uow.GetRepository<IDbContextCustomerRepository>();

                    var customer = customerRepository.GetByKey(401);

                    Assert.IsNotNull(customer);
                }
            }
        }
    }
}