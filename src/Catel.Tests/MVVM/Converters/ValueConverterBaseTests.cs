namespace Catel.Tests.MVVM.Converters
{
    using System;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    [TestFixture]
    public class ValueConverterBaseTest
    {
        #region Methods
        [TestCase]
        public void LinkConversion_ValidSetup()
        {
            var converterA = new ConverterA();
            var converterB = new ConverterB();
            converterB.Link = converterA;

            var result = converterB.Convert(new A(), typeof(C), null, null);

            Assert.That(result, Is.InstanceOf<C>());
        }

        [TestCase]
        public void LinkConversion_InvalidSetup()
        {
            var converterA = new ConverterA();
            var converterB = new ConverterB();
            converterA.Link = converterB;

            var result = converterA.Convert(new A(), typeof(C), null, null);

            Assert.That(ConverterHelper.UnsetValue, Is.EqualTo(result));
        }
        
        [TestCase]
        public void LinkConversion_NoLink()
        {
            var converterB = new ConverterB();

            var result = converterB.Convert(new A(), typeof(C), null, null);

            Assert.That(ConverterHelper.UnsetValue, Is.EqualTo(result));
        }
        #endregion

        #region Helper Classes
        private class A { }
        private class B { }
        private class C { }

        private class ConverterA : ValueConverterBase<A, B>
        {
            protected override object? Convert(A value, Type targetType, object? parameter) => new B();
            protected override object? ConvertBack(B? value, Type targetType, object? parameter) => new A();
        }

        private class ConverterB : ValueConverterBase<B, C>
        {
            protected override object? Convert(B? value, Type targetType, object? parameter) => new C();
            protected override object? ConvertBack(C? value, Type targetType, object? parameter) => new B();
        }
        #endregion
    }
}
