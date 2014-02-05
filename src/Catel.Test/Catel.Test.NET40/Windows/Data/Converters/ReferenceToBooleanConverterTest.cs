// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceToBooleanConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Windows.Data.Converters
{
    using System.Globalization;
    using Catel.Windows.Data.Converters;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    /// <summary>
    ///This is a test class for ReferenceToBooleanConverterTest and is intended
    ///to contain all ReferenceToBooleanConverterTest Unit Tests
    ///</summary>
    [TestClass]
    public class ReferenceToBooleanConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_Null()
        {
            var converter = new ReferenceToBooleanConverter();
            Assert.AreEqual(false, converter.Convert(null, typeof (bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Null_ParameterTrue()
        {
            var converter = new ReferenceToBooleanConverter();
            Assert.AreEqual(true, converter.Convert(null, typeof (bool), true, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Null_ParameterFalse()
        {
            var converter = new ReferenceToBooleanConverter();
            Assert.AreEqual(false, converter.Convert(null, typeof (bool), false, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ValueType()
        {
            var converter = new ReferenceToBooleanConverter();
            Assert.AreEqual(true, converter.Convert(1, typeof (bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ValueType_ParameterTrue()
        {
            var converter = new ReferenceToBooleanConverter();
            Assert.AreEqual(false, converter.Convert(1, typeof (bool), true, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ValueType_ParameterFalse()
        {
            var converter = new ReferenceToBooleanConverter();
            Assert.AreEqual(true, converter.Convert(1, typeof (bool), false, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ReferenceType()
        {
            var converter = new ReferenceToBooleanConverter();
            Assert.AreEqual(true, converter.Convert(new object(), typeof (bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ReferenceType_ParameterTrue()
        {
            var converter = new ReferenceToBooleanConverter();
            Assert.AreEqual(false, converter.Convert(new object(), typeof (bool), true, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ReferenceType_ParameterFalse()
        {
            var converter = new ReferenceToBooleanConverter();
            Assert.AreEqual(true, converter.Convert(new object(), typeof (bool), false, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack()
        {
            var converter = new ReferenceToBooleanConverter();
            Assert.AreEqual(ConverterHelper.DoNothingBindingValue, converter.ConvertBack(null, typeof (object), null, (CultureInfo)null));
        }
        #endregion
    }
}