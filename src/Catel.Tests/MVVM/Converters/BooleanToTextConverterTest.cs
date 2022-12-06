namespace Catel.Tests.MVVM.Converters
{
    using System.Collections.Generic;
    using System.Globalization;
    using Catel.MVVM.Converters;
    using NUnit.Framework;

    [TestFixture]
    public class BooleanToTextConverterTest
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(null, null).Returns(ConverterHelper.UnsetValue).SetName("Convert_Null");
                yield return new TestCaseData("string", null).Returns(ConverterHelper.UnsetValue).SetName("Convert_NonBoolean");
                yield return new TestCaseData(true, null).Returns("Yes").SetName("Convert_True");
                yield return new TestCaseData(true, "YesNo").Returns("Yes").SetName("Convert_True_YesNoAsText");
                yield return new TestCaseData(true, BooleanToTextConverterMode.YesNo).Returns("Yes").SetName("Convert_True_YesNoAsValue");
                yield return new TestCaseData(true, "X").Returns("x").SetName("Convert_True_XAsText");
                yield return new TestCaseData(true, BooleanToTextConverterMode.X).Returns("x").SetName("Convert_True_XAsValue");
                yield return new TestCaseData(false, null).Returns("No").SetName("Convert_False");
                yield return new TestCaseData(false, "YesNo").Returns("No").SetName("Convert_False_YesNoAsText");
                yield return new TestCaseData(false, BooleanToTextConverterMode.YesNo).Returns("No").SetName("Convert_False_YesNoAsValue");
                yield return new TestCaseData(false, "X").Returns(string.Empty).SetName("Convert_False_XAsText");
                yield return new TestCaseData(false, BooleanToTextConverterMode.X).Returns(string.Empty).SetName("Convert_False_XAsValue");
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public object Convert(object value, object parameter)
        {
            var converter = new BooleanToTextConverter();
            return converter.Convert(value, typeof(string), parameter, null);
        }

        [Test]
        public void ConvertBack()
        {
            var converter = new BooleanToTextConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof(bool), null, (CultureInfo)null));
        }

        //[Test]
        //public void ConvertBack_Null()
        //{
        //    Assert.Fail("Need to write unit test");
        //}

        //[Test]
        //public void ConvertBack_YesNo_Yes()
        //{
        //    Assert.Fail("Need to write unit test");
        //}

        //[Test]
        //public void ConvertBack_YesNo_No()
        //{
        //    Assert.Fail("Need to write unit test");
        //}

        //[Test]
        //public void ConvertBack_X_X()
        //{
        //    Assert.Fail("Need to write unit test");
        //}

        //[Test]
        //public void ConvertBack_X_EmptyString()
        //{
        //    Assert.Fail("Need to write unit test");
        //}
    }
}
