// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Extensions.EntityFramework5
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Objects;

    using Catel.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Test.EntityFramework5.DbContextTest;

    public class DbContextExtensionsFacts
    {
        [TestClass]
        public class TheGetObjectContextMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetObjectContext(null));
            }

            [TestMethod]
            public void ReturnsObjectContextForDbContext()
            {
                var dbContext = new TestDbContextContainer();

                var objectContext = dbContext.GetObjectContext();

                Assert.IsNotNull(objectContext);
            }
        }

        [TestClass]
        public class TheGetEntityKeyMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetEntityKey(null, typeof(DbContextProduct), 1));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNulEntityType()
            {
                var dbContext = new TestDbContextContainer();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dbContext.GetEntityKey(null, 1));
            }

            [TestMethod]
            public void ReturnsCorrectKeyValue()
            {
                var dbContext = new TestDbContextContainer();

                var keyValue = dbContext.GetEntityKey(typeof(DbContextProduct), 1);

                Assert.AreEqual("Id", keyValue.EntityKeyValues[0].Key);
                Assert.AreEqual(1, keyValue.EntityKeyValues[0].Value);
            }
        }

        [TestClass]
        public class TheGetEntitySetNameMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetEntitySetName(null, typeof(DbContextProduct)));
            }

            [TestMethod]
            public void ReturnsCorrectEntitySetName()
            {
                var dbContext = new TestDbContextContainer();

                var entitySetName = dbContext.GetEntitySetName(typeof(DbContextProduct));

                Assert.AreEqual("DbContextProducts", entitySetName);
            }
        }

        [TestClass]
        public class TheGetFullEntitySetNameMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetFullEntitySetName(null, typeof(DbContextProduct)));
            }

            [TestMethod]
            public void ReturnsCorrectEntitySetName()
            {
                var dbContext = new TestDbContextContainer();

                var entitySetName = dbContext.GetFullEntitySetName(typeof(DbContextProduct));

                Assert.AreEqual("TestDbContextContainer.DbContextProducts", entitySetName);
            }   
        }

        [TestClass]
        public class TheGetTableNameMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetTableName((ObjectContext)null, typeof(DbContextProduct)));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var dbContext = new TestDbContextContainer();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dbContext.GetTableName(null));
            }

            [TestMethod]
            public void ReturnsTableNameIncludingSchemaForType()
            {
                var dbContext = new TestDbContextContainer();
                var tableName = dbContext.GetTableName<DbContextOrder>();

                Assert.AreEqual("[dbo].[DbContextOrder]", tableName);
            }
        }

        [TestClass]
        public class TheSetTransactionLevelMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.SetTransactionLevel(null, IsolationLevel.ReadUncommitted));
            }
        }
    }
}