namespace Catel.Tests.Reflection;

using System.Reflection;
using Catel.Reflection;
using NUnit.Framework;

public class BindingFlagsHelperFacts
{
    [TestFixture]
    public class The_GetFinalBindingFlags_Method
    {
        [Test]
        public void Includes_Static_Members_When_AllowStaticMembers_Is_True()
        {
            var flags = BindingFlagsHelper.GetFinalBindingFlags(false, true);

            Assert.That(flags.HasFlag(BindingFlags.Static), Is.True);
        }

        [Test]
        public void Excludes_Static_Members_When_AllowStaticMembers_Is_False()
        {
            var flags = BindingFlagsHelper.GetFinalBindingFlags(false, false);

            Assert.That(flags.HasFlag(BindingFlags.Static), Is.False);
        }

        [Test]
        public void Includes_FlattenHierarchy_When_FlattenHierarchy_Is_True()
        {
            var flags = BindingFlagsHelper.GetFinalBindingFlags(true, false);

            Assert.That(flags.HasFlag(BindingFlags.FlattenHierarchy), Is.True);
        }

        [Test]
        public void Includes_NonPublic_When_AllowNonPublicMembers_Is_True()
        {
            var flags = BindingFlagsHelper.GetFinalBindingFlags(false, false, true);

            Assert.That(flags.HasFlag(BindingFlags.NonPublic), Is.True);
        }

        [Test]
        public void Excludes_NonPublic_When_AllowNonPublicMembers_Is_False()
        {
            var flags = BindingFlagsHelper.GetFinalBindingFlags(false, false, false);

            Assert.That(flags.HasFlag(BindingFlags.NonPublic), Is.False);
        }
    }
}
