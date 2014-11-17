// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToLowerCaseConverterFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    public class TextToLowerCaseConverterFacts
    {
        #region Nested type: TheConvertMethod
        [TestFixture]
        public class TheConvertMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsLowerCaseString()
            {
                var converter = new TextToLowerCaseConverter();

                Assert.AreEqual("lowercase", converter.Convert("LoWeRcAsE", typeof (string), null, (CultureInfo) null));
            }

            [TestCase]
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