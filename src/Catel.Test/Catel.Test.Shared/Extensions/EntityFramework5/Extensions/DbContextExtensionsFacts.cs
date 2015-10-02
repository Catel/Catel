// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET && !SKIP_EF

namespace Catel.Test.Extensions.EntityFramework5
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Objects;

    using Catel.Data;
    using NUnit.Framework;
    using Test.EntityFramework5.DbContextTest;

    public class DbContextExtensionsFacts
    {
        [TestFixture]
        public class TheGetObjectContextMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetObjectContext(null));
            }

            [TestCase]
            public void ReturnsObjectContextForDbContext()
            {
                var dbContext = new TestDbContextContainer();

                var objectContext = dbContext.GetObjectContext();

                Assert.IsNotNull(objectContext);
            }
        }

        [TestFixture]
        public class TheGetEntityKeyMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetEntityKey(null, typeof(DbContextProduct), 1));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNulEntityType()
            {
                var dbContext = new TestDbContextContainer();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dbContext.GetEntityKey(null, 1));
            }

            [TestCase]
            public void ReturnsCorrectKeyValue()
            {
                var dbContext = new TestDbContextContainer();

                var keyValue = dbContext.GetEntityKey(typeof(DbContextProduct), 1);

                Assert.AreEqual("Id", keyValue.EntityKeyValues[0].Key);
                Assert.AreEqual(1, keyValue.EntityKeyValues[0].Value);
            }
        }

        [TestFixture]
        public class TheGetEntitySetNameMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetEntitySetName(null, typeof(DbContextProduct)));
            }

            [TestCase]
            public void ReturnsCorrectEntitySetName()
            {
                var dbContext = new TestDbContextContainer();

                var entitySetName = dbContext.GetEntitySetName(typeof(DbContextProduct));

                Assert.AreEqual("DbContextProducts", entitySetName);
            }
        }

        [TestFixture]
        public class TheGetFullEntitySetNameMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetFullEntitySetName(null, typeof(DbContextProduct)));
            }

            [TestCase]
            public void ReturnsCorrectEntitySetName()
            {
                var dbContext = new TestDbContextContainer();

                var entitySetName = dbContext.GetFullEntitySetName(typeof(DbContextProduct));

                Assert.AreEqual("TestDbContextContainer.DbContextProducts", entitySetName);
            }   
        }

        [TestFixture]
        public class TheGetTableNameMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.GetTableName((ObjectContext)null, typeof(DbContextProduct)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var dbContext = new TestDbContextContainer();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dbContext.GetTableName(null));
            }

            [TestCase]
            public void ReturnsTableNameIncludingSchemaForType()
            {
                var dbContext = new TestDbContextContainer();
                var tableName = dbContext.GetTableName<DbContextOrder>();

                Assert.AreEqual("[dbo].[DbContextOrder]", tableName);
            }
        }

        [TestFixture]
        public class TheSetTransactionLevelMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => DbContextExtensions.SetTransactionLevel(null, IsolationLevel.ReadUncommitted));
            }
        }
    }
}

#endif