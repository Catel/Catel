// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET && !SKIP_EF

namespace Catel.Test.Extensions.EntityFramework5.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Objects;
    using Catel.Data;
    using NUnit.Framework;
    using Test.EntityFramework5.DbContextTest;
    using Test.EntityFramework5.ObjectContextTest;

    public class ConnectionStringHelperFacts
    {
        [TestFixture]
        public class TheDbContextSetConnectionStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ConnectionStringHelper.SetConnectionString((DbContext)null, "dummy"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullOrWhitespaceConnectionString()
            {
                var dbContext = new TestDbContextContainer();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ConnectionStringHelper.SetConnectionString(dbContext, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ConnectionStringHelper.SetConnectionString(dbContext, string.Empty));
            }

            [TestCase]
            public void SetsConnectionString()
            {
                var dbContext = new TestDbContextContainer();

                dbContext.SetConnectionString(TestConnectionStrings.DbContextModified);

                Assert.AreEqual(TestConnectionStrings.DbContextModified, dbContext.Database.Connection.ConnectionString);
            }
        }

        [TestFixture]
        public class TheObjectContextSetConnectionStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ConnectionStringHelper.SetConnectionString((ObjectContext)null, "dummy"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullOrWhitespaceConnectionString()
            {
                var connectionString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextDefault);
                var objectContext = new TestObjectContextContainer(connectionString);

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ConnectionStringHelper.SetConnectionString(objectContext, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ConnectionStringHelper.SetConnectionString(objectContext, string.Empty));
            }

            [TestCase]
            public void SetsConnectionString()
            {
                var connectionString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextDefault);
                var objectContext = new TestObjectContextContainer(connectionString);

                objectContext.SetConnectionString(TestConnectionStrings.ObjectContextModified);

                var expectedConnectionString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextModified);
                Assert.AreEqual(expectedConnectionString, objectContext.Connection.ConnectionString);
            }
        }

        [TestFixture]
        public class TheDbContextGetConnectionStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDbContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ConnectionStringHelper.SetConnectionString((DbContext)null, "dummy"));
            }

            [TestCase]
            public void ReturnsNamedConnectionString()
            {
                var context = new TestDbContextContainer();

                string expectedString = string.Format("{0};Application Name=EntityFrameworkMUE", TestConnectionStrings.DbContextDefault);

                var connectionString = context.GetConnectionString();

                Assert.IsTrue(string.Equals(expectedString, connectionString, StringComparison.OrdinalIgnoreCase));
            }

            [TestCase]
            public void ReturnsRealConnectionString()
            {
                var context = new TestDbContextContainer();

                string expectedString = TestConnectionStrings.DbContextModified;

                context.SetConnectionString(TestConnectionStrings.DbContextModified);
                var connectionString = context.GetConnectionString();

                Assert.IsTrue(string.Equals(expectedString, connectionString, StringComparison.OrdinalIgnoreCase));
            }
        }

        [TestFixture]
        public class TheObjectContextGetConnectionStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullObjectContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ConnectionStringHelper.SetConnectionString((ObjectContext)null, "dummy"));
            }

            [TestCase]
            public void ReturnsNamedConnectionString()
            {
                var context = new TestObjectContextContainer();

                string expectedString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextDefault);

                var connectionString = context.GetConnectionString();

                Assert.IsTrue(string.Equals(expectedString, connectionString, StringComparison.OrdinalIgnoreCase));
            }

            [TestCase]
            public void ReturnsRealConnectionString()
            {
                var context = new TestObjectContextContainer();

                string expectedString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextModified);

                context.SetConnectionString(TestConnectionStrings.ObjectContextModified);
                var connectionString = context.GetConnectionString();

                Assert.IsTrue(string.Equals(expectedString, connectionString, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}

#endif