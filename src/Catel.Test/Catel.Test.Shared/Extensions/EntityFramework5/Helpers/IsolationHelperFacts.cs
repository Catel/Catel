// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsolationHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.EntityFramework5
{
    using System;
    using System.Data;
    using Catel.Data;
    using NUnit.Framework;

    public class IsolationHelperFacts
    {
        #region Nested type: TheTranslateTransactionLevelToSqlMethod
        [TestFixture]
        public class TheTranslateTransactionLevelToSqlMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsCorrectValueForReadUncommitted()
            {
                var expectedValue = "SET TRANSACTION LEVEL ISOLATION LEVEL READ UNCOMMITTED;";
                var actualValue = IsolationHelper.TranslateTransactionLevelToSql(IsolationLevel.ReadUncommitted);

                Assert.AreEqual(expectedValue, actualValue);
            }
            #endregion
        }
        #endregion
    }
}

#endif