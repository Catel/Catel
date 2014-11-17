// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularDependencyExceptionFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.IoC.Exceptions
{
    using System;
    using Catel.IoC;
    using NUnit.Framework;

    public class CircularDependencyExceptionFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullTypePath()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new CircularDependencyException(null, string.Empty));
            }
        }
    }
}