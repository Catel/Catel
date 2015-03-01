// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCacheFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Reflection
{
    using System;
    using System.Collections.Generic;
    using Catel.Reflection;
    using NUnit.Framework;

    public class TypeCacheFacts
    {
        [TestFixture]
        public class TheGetTypeMethod
        {
            [TestCase]
            public void ReturnsTypeFromMsCorlib()
            {
                var type = TypeCache.GetType("System.String");

                Assert.AreEqual(typeof(string), type);
            }

            [TestCase]
            public void ReturnsTypeFromSystem()
            {
                var type = TypeCache.GetType("System.Uri");

                Assert.AreEqual(typeof(Uri), type);
            }

            [TestCase]
            public void ReturnsTypeFromSystemCore()
            {
                var type = TypeCache.GetType("System.Lazy`1");

                Assert.AreEqual(typeof(Lazy<>), type);
            }

            [TestCase]
            public void ReturnsTypeForLateBoundGenericTypeMultipleTimes()
            {
                var type = TypeCache.GetType("System.Collections.Generic.List`1[[System.Int32]]");
                Assert.AreEqual(typeof(List<int>), type);

                var type2 = TypeCache.GetType("System.Collections.Generic.List`1[[System.Int32]]");
                Assert.AreEqual(typeof(List<int>), type2);
            }

            [TestCase("Catel.Test.Reflection.TypeCacheFacts[]", typeof(TypeCacheFacts[]))]
            [TestCase("System.Collections.Generic.List`1[[Catel.Test.Reflection.TypeCacheFacts]]", typeof(List<TypeCacheFacts>))]
            public void ReturnsRightType(string typeName, Type expectedType)
            {
                var type = TypeCache.GetType(typeName);

                Assert.AreEqual(expectedType, type);
            }
        }
    }
}