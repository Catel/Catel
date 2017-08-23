// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeArrayFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Reflection
{
    using System;

    using Catel.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class TypeArrayFacts
    {
        [TestFixture]
        private class The_Generic_From_Method
        {
            [Test]
            public void Returns_The_Same_Type_Array_Instance_Of_The_Non_Generic_Method_Version_1()
            {
                Assert.AreSame(TypeArray.From<int>(), TypeArray.From(typeof(int)));
            }

            [Test]
            public void Returns_The_Same_Type_Array_Instance_Of_The_Non_Generic_Method_Version_2()
            {
                Assert.AreSame(TypeArray.From<int, bool>(), TypeArray.From(typeof(int), typeof(bool)));
            }

            [Test]
            public void Returns_The_Same_Type_Array_Instance_Of_The_Non_Generic_Method_Version_3()
            {
                Assert.AreSame(TypeArray.From<int, bool, string>(), TypeArray.From(typeof(int), typeof(bool), typeof(string)));
            }

            [Test]
            public void Returns_The_Same_Type_Array_Instance_Of_The_Non_Generic_Method_Version_4()
            {
                Assert.AreSame(TypeArray.From<int, bool, string, object>(), TypeArray.From(typeof(int), typeof(bool), typeof(string), typeof(object)));
            }

            [Test]
            public void Returns_The_Same_Type_Array_Instance_Of_The_Non_Generic_Method_Version_5()
            {
                Assert.AreSame(TypeArray.From<int, bool, string, object, Exception>(), TypeArray.From(typeof(int), typeof(bool), typeof(string), typeof(object), typeof(Exception)));
            }
        }
    }
}