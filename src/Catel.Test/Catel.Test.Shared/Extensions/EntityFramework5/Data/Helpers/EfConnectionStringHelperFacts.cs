// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EfConnectionStringHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.EntityFramework5.Data
{
    using System;
    using Catel.Data;
    using NUnit.Framework;
    using Test.EntityFramework5.ObjectContextTest;

    public class EfConnectionStringHelperFacts
    {
        [TestFixture]
        public class TheGetEntityFrameworkConnectionStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullContextType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => EfConnectionStringHelper.GetEntityFrameworkConnectionString(null, TestConnectionStrings.ObjectContextDefault));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullAndEmptyConnectionString()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), string.Empty));
            }

            [TestCase]
            public void ReturnsCorrectValueForTestObjectContext()
            {
                string expectedValue = string.Format("metadata=res://*/TestObjectContext.csdl|res://*/TestObjectContext.ssdl|res://*/TestObjectContext.msl;provider=System.Data.SqlClient;provider connection string=\"{0}\"", TestConnectionStrings.ObjectContextDefault);

                var connectionString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextDefault);

                Assert.AreEqual(expectedValue, connectionString);
            }
        }
    }
}

#endif