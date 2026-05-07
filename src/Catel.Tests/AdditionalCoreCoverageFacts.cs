namespace Catel.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Catel.Data;
using Catel.Logging;
using Catel.Reflection;
using Catel.Scoping;
using Catel.ThirdPartyNotices;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

public class AdditionalCoreCoverageFacts
{
    [TestFixture]
    public class The_LogEntryEventArgs_Class
    {
        [Test]
        public void Stores_LogEntry()
        {
            var logEntry = new LogEntry
            {
                Category = "category",
                DateTime = DateTimeOffset.UtcNow,
                LogLevel = LogLevel.Information,
                Message = "message"
            };

            var eventArgs = new LogEntryEventArgs(logEntry);

            Assert.That(eventArgs.LogEntry, Is.EqualTo(logEntry));
        }
    }

    [TestFixture]
    public class The_ScopeClosedEventArgs_Class
    {
        [Test]
        public void Stores_Provided_Values()
        {
            var scopeObject = new object();

            var eventArgs = new ScopeClosedEventArgs(scopeObject, "scope");

            Assert.That(eventArgs.ScopeObject, Is.EqualTo(scopeObject));
            Assert.That(eventArgs.ScopeName, Is.EqualTo("scope"));
        }
    }

    [TestFixture]
    public class The_ExcludeFromValidationAttribute_Class
    {
        [Test]
        public void Can_Be_Instantiated()
        {
            var attribute = new ExcludeFromValidationAttribute();

            Assert.That(attribute, Is.Not.Null);
        }
    }

    [TestFixture]
    public class The_ValidateModelAttribute_Class
    {
        [Test]
        public void Stores_ValidatorType()
        {
            var attribute = new ValidateModelAttribute(typeof(string));

            Assert.That(attribute.ValidatorType, Is.EqualTo(typeof(string)));
        }
    }

    [TestFixture]
    public class The_ValidationEventArgs_Class
    {
        [Test]
        public void Stores_ValidationContext()
        {
            var validationContext = new Mock<IValidationContext>().Object;

            var eventArgs = new ValidationEventArgs(validationContext);

            Assert.That(eventArgs.ValidationContext, Is.EqualTo(validationContext));
        }
    }

    [TestFixture]
    public class The_AssemblyLoadedEventArgs_Class
    {
        [Test]
        public void Supports_Lazy_Loaded_Types()
        {
            var invocationCount = 0;
            var lazyLoadedTypes = new Lazy<IEnumerable<Type>>(() =>
            {
                invocationCount++;
                return new[] { typeof(string) };
            });

            var eventArgs = new AssemblyLoadedEventArgs(typeof(string).Assembly, lazyLoadedTypes);

            Assert.That(invocationCount, Is.EqualTo(0));

            var firstResult = eventArgs.LoadedTypes.ToArray();
            var secondResult = eventArgs.LoadedTypes.ToArray();

            Assert.That(invocationCount, Is.EqualTo(1));
            Assert.That(firstResult, Is.EquivalentTo(new[] { typeof(string) }));
            Assert.That(secondResult, Is.EquivalentTo(new[] { typeof(string) }));
        }

        [Test]
        public void Stores_Eager_Loaded_Types()
        {
            var loadedTypes = new[] { typeof(int), typeof(string) };

            var eventArgs = new AssemblyLoadedEventArgs(typeof(string).Assembly, loadedTypes);

            Assert.That(eventArgs.LoadedTypes, Is.EquivalentTo(loadedTypes));
        }
    }

    [TestFixture]
    public class The_MustBeImplementedException_Class
    {
        [Test]
        public void Contains_Default_Message()
        {
            var exception = new MustBeImplementedException();

            Assert.That(exception.Message, Does.Contain("limited resources"));
        }
    }

    [TestFixture]
    public class The_NotSupportedInPlatformException_Class
    {
        [Test]
        public void Stores_Reason_And_Platform()
        {
            var exception = new NotSupportedInPlatformException("{0} is missing", "Feature");

            Assert.That(exception.Reason, Is.EqualTo("Feature is missing"));
            Assert.That(exception.Platform, Is.EqualTo(Platforms.CurrentPlatform));
            Assert.That(exception.Message, Is.EqualTo("Feature is currently not yet supported for this platform"));
        }
    }

    [TestFixture]
    public class The_ThirdPartyNotice_Class
    {
        [Test]
        public void Initializes_With_Empty_Values()
        {
            var notice = new ThirdPartyNotice();

            Assert.That(notice.Title, Is.EqualTo(string.Empty));
            Assert.That(notice.Content, Is.EqualTo(string.Empty));
            Assert.That(notice.Url, Is.EqualTo(string.Empty));
        }
    }

    [TestFixture]
    public class The_FontThirdPartyNotice_Class
    {
        [Test]
        public void Stores_Provided_Values()
        {
            var notice = new FontThirdPartyNotice("Open Sans", "https://example.com");

            Assert.That(notice.Title, Is.EqualTo("Open Sans"));
            Assert.That(notice.Url, Is.EqualTo("https://example.com"));
            Assert.That(notice.Content, Is.EqualTo(string.Empty));
        }
    }

    [TestFixture]
    public class The_FileBasedThirdPartyNotice_Class
    {
        [Test]
        public void Reads_File_Content()
        {
            var fileName = Path.GetTempFileName();

            try
            {
                File.WriteAllText(fileName, "test content");

                var notice = new FileBasedThirdPartyNotice("title", "url", fileName);

                Assert.That(notice.Title, Is.EqualTo("title"));
                Assert.That(notice.Url, Is.EqualTo("url"));
                Assert.That(notice.Content, Is.EqualTo("test content"));
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
