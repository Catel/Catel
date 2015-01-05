// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToTextConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;
    using NUnit.Framework;

    [TestFixture]
    public class BooleanToTextConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual(string.Empty, converter.Convert(null, typeof (string), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_NonBoolean()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual(string.Empty, converter.Convert("string", typeof (string), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual("Yes", converter.Convert(true, typeof (string), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_False()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual("No", converter.Convert(false, typeof (string), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True_YesNoAsText()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual("Yes", converter.Convert(true, typeof (string), "YesNo", (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True_YesNoAsValue()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual("Yes", converter.Convert(true, typeof (string), BooleanToTextConverterMode.YesNo, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True_XAsText()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual("x", converter.Convert(true, typeof (string), "X", (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True_XAsValue()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual("x", converter.Convert(true, typeof (string), BooleanToTextConverterMode.X, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_False_YesNoAsText()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual("No", converter.Convert(false, typeof (string), "YesNo", (CultureInfo)null));
        }

        [TestCase]
        public void Convert_False_YesNoAsValue()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual("No", converter.Convert(false, typeof (string), BooleanToTextConverterMode.YesNo, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_False_XAsText()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual(string.Empty, converter.Convert(false, typeof (string), "X", (CultureInfo)null));
        }

        [TestCase]
        public void Convert_False_XAsValue()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual(string.Empty, converter.Convert(false, typeof (string), BooleanToTextConverterMode.X, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof (bool), null, (CultureInfo)null));
        }
        #endregion

        //[TestCase]
        //public void ConvertBack_Null()
        //{
        //    Assert.Fail("Need to write unit test");
        //}

        //[TestCase]
        //public void ConvertBack_YesNo_Yes()
        //{
        //    Assert.Fail("Need to write unit test");
        //}

        //[TestCase]
        //public void ConvertBack_YesNo_No()
        //{
        //    Assert.Fail("Need to write unit test");
        //}

        //[TestCase]
        //public void ConvertBack_X_X()
        //{
        //    Assert.Fail("Need to write unit test");
        //}

        //[TestCase]
        //public void ConvertBack_X_EmptyString()
        //{
        //    Assert.Fail("Need to write unit test");
        //}
    }
}