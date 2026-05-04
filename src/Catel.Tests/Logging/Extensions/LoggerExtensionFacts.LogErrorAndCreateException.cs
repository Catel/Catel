namespace Catel.Tests.Logging;

using System;
using System.Threading.Tasks;
using Catel.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using VerifyNUnit;

public partial class LoggerExtensionFacts
{
    [TestFixture]
    public class The_LogErrorAndCreateException_Method
    {
        [Test]
        public async Task Logs_Error_And_Creates_Exception()
        {
            var logger = new NullLogger<LoggerExtensionFacts>();
            var exceptionMessage = "This is an error message";
            var exception = logger.LogErrorAndCreateException<InvalidOperationException>(exceptionMessage);

            Assert.That(exception, Is.Not.Null);
            await Verifier.Verify(exception.Message);
        }

        [Test]
        public async Task Logs_Error_And_Creates_Exception_StringFormat()
        {
            var logger = new NullLogger<LoggerExtensionFacts>();
            var exceptionMessage = "This is a {0} error message";
            var exception = logger.LogErrorAndCreateException<InvalidOperationException>(exceptionMessage, "special");

            Assert.That(exception, Is.Not.Null);
            await Verifier.Verify(exception.Message);
        }

        [Test]
        public async Task Logs_Error_And_Creates_Exception_StructuredFormat()
        {
            var logger = new NullLogger<LoggerExtensionFacts>();
            var exceptionMessage = "This is a {Category} error message";
            var exception = logger.LogErrorAndCreateException<InvalidOperationException>(exceptionMessage, "special");

            Assert.That(exception, Is.Not.Null);
            await Verifier.Verify(exception.Message);
        }
    }
}
