// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensionFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Text
{
    using Catel.Text;
    using System.Text;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class StringBuilderExtensionFacts
    {
        [TestClass]
        public class TheAppendLineMethod
        {
            [TestMethod]
            public void CorrectlyAppendsLineWithFormatting()
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine("test with {0} {1}", "formatting", 1);

                Assert.AreEqual("test with formatting 1\r\n", stringBuilder.ToString());
            }
        }
    }
}