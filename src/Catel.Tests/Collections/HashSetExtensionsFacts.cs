namespace Catel.Tests.Collections;

using System.Collections.Generic;
using Catel.Collections;
using NUnit.Framework;

public class HashSetExtensionsFacts
{
    [TestFixture]
    public class The_AddRange_Method
    {
        [Test]
        public void Adds_All_Items_To_HashSet()
        {
            var hashSet = new HashSet<int>();
            var items = new[] { 1, 2, 3 };

            hashSet.AddRange(items);

            Assert.That(hashSet.Count, Is.EqualTo(3));
            Assert.That(hashSet.Contains(1), Is.True);
            Assert.That(hashSet.Contains(2), Is.True);
            Assert.That(hashSet.Contains(3), Is.True);
        }

        [Test]
        public void Does_Not_Add_Duplicate_Items()
        {
            var hashSet = new HashSet<int> { 1 };
            var items = new[] { 1, 2 };

            hashSet.AddRange(items);

            Assert.That(hashSet.Count, Is.EqualTo(2));
        }
    }
}
