// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Reflection
{
    using System;
    using System.Reflection;
    using Catel.Runtime.Serialization;
    using Catel.Reflection;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    /// <summary>
    /// The attribute helper facts.
    /// </summary>
    public class AttributeHelperFacts
    {
        /// <summary>
        /// The the try get attribute method.
        /// </summary>
        [TestClass]
        public class TheTryGetAttributeMethod
        {
            #region Public Methods and Operators

            /// <summary>
            /// The throws argument null exception for null property info.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullPropertyInfo()
            {
                RedirectTypeAttribute attribute;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(
                    () => AttributeHelper.TryGetAttribute((MemberInfo)null, out attribute));
            }

            #endregion
        }
    }
}