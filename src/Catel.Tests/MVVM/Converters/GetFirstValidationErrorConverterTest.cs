namespace Catel.Tests.MVVM.Converters
{
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    [TestFixture]
    public class GetFirstValidationErrorConverterTest
    {
        [TestCase]
        public void Convert_Null()
        {
            var converter = new GetFirstValidationErrorConverter();
            Assert.AreEqual(string.Empty, converter.Convert(null, typeof (string), null, null));
        }

        //[TestCase]
        //public void Convert_ObjectWithError()
        //{
        //    List<ValidationError> errors = new List<ValidationError>();
        //    errors.Add(new ValidationError(new ValidationRule(ValidationStep.CommittedValue, false), null));

        //    var converter = new GetFirstValidationErrorConverter();
        //    Assert.AreEqual(string.Empty, converter.Convert(null, typeof(object), null, null));
        //}

        //[TestCase]
        //public void Convert_ObjectWithoutErrors()
        //{
        //    Assert.Fail("Need to write unit test");
        //}

        [TestCase]
        public void ConvertBack()
        {
            var converter = new GetFirstValidationErrorConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof (object), null, null));
        }
    }
}
