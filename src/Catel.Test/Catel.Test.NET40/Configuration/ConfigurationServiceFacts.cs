// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationServiceFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Configuration
{
    using System;
    using Catel.Configuration;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ConfigurationServiceFacts
    {
        [TestClass]
        public class TheGetValueMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullKey()
            {
                var configurationService = new ConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.GetValue<string>(null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyKey()
            {
                var configurationService = new ConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.GetValue<string>(string.Empty));
            }
        }

        [TestClass]
        public class TheSetValueMethod
        {
            
        }
    }
}