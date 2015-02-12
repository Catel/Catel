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

    public class TextToUpperCaseConverterFacts
    {
        #region Nested type: TheConvertMethod
        [TestFixture]
        public class TheConvertMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsUpperCaseString()
            {
                var converter = new TextToUpperCaseConverter();

                Assert.AreEqual("UPPERCASE", converter.Convert("UpPeRcAsE", typeof(string), null, (CultureInfo)null));
            }

            [TestCase]
            public void ReturnsNullForNullValue()
            {
                var converter = new TextToUpperCaseConverter();

                Assert.AreEqual(null, converter.Convert(null, typeof(string), null, (CultureInfo)null));
            }
            #endregion
        }
        #endregion
    }
}