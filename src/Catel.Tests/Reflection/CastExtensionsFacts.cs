#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation

namespace Catel.Tests.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.Reflection;
    using NUnit.Framework;

    public class CastExtensionsFacts
    {
        [TestFixture]
        public class TheCastToBooleanMethod
        {
            //[TestCase]
            //public void ConvertsBooleanValues<T>()
            //{
            //    var boolRef = (T)(object)true;
            //    var boolResult = boolRef.CastToBoolean();

            //    Assert.IsTrue(boolResult);
            //}
        }

        [TestFixture]
        public class TheCastToMethod
        {
            public static IEnumerable<ITester> TestCases()
            {
                yield return new Tester<bool> { ExpectedValue = true };
            }

            [TestCaseSource("TestCases")]
            public void TestReverse(ITester tester)
            {
                tester.ExecuteTest();
            }

            public interface ITester
            {
                void ExecuteTest();
            }

            public class Tester<T> : ITester
            {
                public T ExpectedValue { get; set; }

                public Tester()
                {
                }

                public void ExecuteTest()
                {
                    var actualValue = true.CastTo<T>();

                    Assert.AreEqual(actualValue, ExpectedValue);
                }
            }
        }
    }
}
