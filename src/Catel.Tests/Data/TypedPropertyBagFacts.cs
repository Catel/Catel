namespace Catel.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.Data;
    using NUnit.Framework;

    [TestFixture]
    public class TypedPropertyBagFacts
    {
        //[TestCase]
        //public void CanSetCompatiblePropertyValues()
        //{
        //    var propertyBag = new TypedPropertyBag();

        //    propertyBag.SetValue("Int", 42);
        //    propertyBag.SetValue("Int", 42u);
        //}

        [TestCase]
        public void PreventsRegistrationWithDifferent()
        {
            var propertyBag = new TypedPropertyBag();

            propertyBag.SetValue("Int", 42);
            ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => propertyBag.SetValue("Int", new object()));
        }
    }
}
