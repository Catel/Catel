// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToLowerCaseConverterFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

    public class TextToLowerCaseConverterFacts
    {
        #region Nested type: TheConvertMethod
        [TestClass]
        public class TheConvertMethod
        {
            #region Methods
            [TestMethod]
            public void ReturnsLowerCaseString()
            {
                var converter = new TextToLowerCaseConverter();

                Assert.AreEqual("lowercase", converter.Convert("LoWeRcAsE", typeof (string), null, (CultureInfo) null));
            }

            [TestMethod]
            public void ReturnsNullForNullValue()
            {
                var converter = new TextToLowerCaseConverter();

                Assert.AreEqual(null, converter.Convert(null, typeof (string), null, (CultureInfo) null));
            }
            #endregion
        }
        #endregion
    }
}