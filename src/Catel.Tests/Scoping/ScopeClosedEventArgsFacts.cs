namespace Catel.Tests.Scoping;

using Catel.Scoping;
using NUnit.Framework;

public class ScopeClosedEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
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
}
