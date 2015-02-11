// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Reflection
{
    using System;
    using System.Reflection;
    using Catel.Reflection;

    using NUnit.Framework;

    /// <summary>
    /// The attribute helper facts.
    /// </summary>
    public class AttributeHelperFacts
    {
        /// <summary>
        /// The the try get attribute method.
        /// </summary>
        [TestFixture]
        public class TheTryGetAttributeMethod
        {
            #region Public Methods and Operators

            /// <summary>
            /// The throws argument null exception for null property info.
            /// </summary>
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullPropertyInfo()
            {
                ObsoleteAttribute attribute;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => AttributeHelper.TryGetAttribute((MemberInfo)null, out attribute));
            }

            #endregion
        }
    }
}