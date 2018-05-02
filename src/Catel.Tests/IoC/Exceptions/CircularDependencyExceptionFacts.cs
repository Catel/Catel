// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularDependencyExceptionFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Tests.IoC.Exceptions
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
                TypeRequestInfo requestInfo = new TypeRequestInfo(typeof(object));
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new CircularDependencyException(requestInfo, null, string.Empty));
            }
        }
    }
}