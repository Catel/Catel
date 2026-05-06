namespace Catel.Tests.Collections;

using System.Collections.Generic;
using Catel.Collections;
using NUnit.Framework;

public class DictionaryExtensionsFacts
{
    [TestFixture]
    public class The_AddRange_Method
    {
        [Test]
        public void Adds_All_Items_From_Source_To_Target()
        {
            var target = new Dictionary<string, int>();
            var source = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };

            target.AddRange(source);

            Assert.That(target.Count, Is.EqualTo(2));
            Assert.That(target["a"], Is.EqualTo(1));
            Assert.That(target["b"], Is.EqualTo(2));
        }

        [Test]
        public void Overwrites_Existing_Items_When_OverwriteExisting_Is_True()
        {
            var target = new Dictionary<string, int> { ["a"] = 99 };
            var source = new Dictionary<string, int> { ["a"] = 1 };

            target.AddRange(source, overwriteExisting: true);

            Assert.That(target["a"], Is.EqualTo(1));
        }

        [Test]
        public void Does_Not_Overwrite_Existing_Items_When_OverwriteExisting_Is_False()
        {
            var target = new Dictionary<string, int> { ["a"] = 99 };
            var source = new Dictionary<string, int> { ["a"] = 1 };

            target.AddRange(source, overwriteExisting: false);

            Assert.That(target["a"], Is.EqualTo(99));
        }
    }

    [TestFixture]
    public class The_AddItemIfNotEmpty_Method
    {
        [Test]
        public void Adds_Item_When_Value_Is_Not_Empty()
        {
            var dictionary = new Dictionary<string, string>();

            dictionary.AddItemIfNotEmpty("key", "value");

            Assert.That(dictionary["key"], Is.EqualTo("value"));
        }

        [Test]
        public void Does_Not_Add_Item_When_Value_Is_Empty_String()
        {
            var dictionary = new Dictionary<string, string>();

            dictionary.AddItemIfNotEmpty("key", string.Empty);

            Assert.That(dictionary.ContainsKey("key"), Is.False);
        }

        [Test]
        public void Does_Not_Add_Item_When_Value_Is_Null()
        {
            var dictionary = new Dictionary<string, string>();

            dictionary.AddItemIfNotEmpty("key", null);

            Assert.That(dictionary.ContainsKey("key"), Is.False);
        }
    }
}
