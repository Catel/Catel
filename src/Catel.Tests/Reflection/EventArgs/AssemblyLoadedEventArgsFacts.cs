namespace Catel.Tests.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Reflection;
using NUnit.Framework;

public class AssemblyLoadedEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
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
}
