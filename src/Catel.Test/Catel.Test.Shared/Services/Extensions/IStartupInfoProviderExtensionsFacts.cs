// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStartupInfoProviderExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Services
{
    using Catel.Services;
    using NUnit.Framework;

    [TestFixture]
    public class IStartupInfoProviderExtensionsFacts
    {
        [TestCase(new[] { "-a", "testa", "-b", "testb" }, "-a testa -b testb")]
        [TestCase(new[] { "-a", "testa", "-b", "test with spaces" }, "-a testa -b \"test with spaces\"")]
        [TestCase(new[] { "-a", "testa", "-b", "test:with:colons" }, "-a testa -b \"test:with:colons\"")]
        [TestCase(new[] { "-a", "testa", "-b", "test-with-dashes" }, "-a testa -b \"test-with-dashes\"")]
        public void TheGetCommandLineMethod(string[] input, string expectedCommandLine)
        {
            var fixture = new StartUpInfoProviderFixture(input);

            var commandLine = fixture.GetCommandLine();

            Assert.AreEqual(expectedCommandLine, commandLine);
        }
    }
}