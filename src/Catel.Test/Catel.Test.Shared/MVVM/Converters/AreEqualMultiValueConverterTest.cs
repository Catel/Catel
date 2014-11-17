// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AreEqualMultiValueConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.MVVM.Converters
{
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    [TestFixture]
    public class AreEqualMultiValueConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_NotEnoughValues()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.AreEqual(false, converter.Convert(new object[] {1}, typeof (bool), null, null));
        }

        [TestCase]
        public void Convert_TooManyValues()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.AreEqual(false, converter.Convert(new object[] {1, 1, 1}, typeof (bool), null, null));
        }

        [TestCase]
        public void Convert_Equal_Null()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.AreEqual(true, converter.Convert(new object[] {null, null}, typeof (bool), null, null));
        }

        [TestCase]
        public void Convert_Equal_Integer()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.AreEqual(true, converter.Convert(new object[] {1, 1}, typeof (bool), null, null));
        }

        [TestCase]
        public void Convert_NonEqual()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.AreEqual(false, converter.Convert(new object[] {1, 2}, typeof (bool), null, null));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.AreEqual(null, converter.ConvertBack(null, new[] {typeof (object)}, null, null));
        }
        #endregion
    }
}

#endif