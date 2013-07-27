// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsolationHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Extensions.EntityFramework5
{
    using System;
    using System.Data;
    using Catel.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class IsolationHelperFacts
    {
        #region Nested type: TheTranslateTransactionLevelToSqlMethod
        [TestClass]
        public class TheTranslateTransactionLevelToSqlMethod
        {
            #region Methods
            [TestMethod]
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