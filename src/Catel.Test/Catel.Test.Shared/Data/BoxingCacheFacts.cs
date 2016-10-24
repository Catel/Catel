// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoxingCacheFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
{
    using Catel.Data;
    using NUnit.Framework;

    [TestFixture]
    public class BoxingCacheFacts
    {
        [TestCase]
        public void ReturnsSameReferenceMultipleTimes_Bool()
        {
            var boxingCache = new BoxingCache<bool>();

            var boxedValue_True_1 = boxingCache.GetBoxedValue(true);
            var boxedValue_True_2 = boxingCache.GetBoxedValue(true);

            Assert.IsTrue(ReferenceEquals(boxedValue_True_1, boxedValue_True_2));

            var boxedValue_False_1 = boxingCache.GetBoxedValue(false);
            var boxedValue_False_2 = boxingCache.GetBoxedValue(false);

            Assert.IsTrue(ReferenceEquals(boxedValue_False_1, boxedValue_False_2));
        }

        [TestCase]
        public void ReturnsSameReferenceMultipleTimes_Int()
        {
            var boxingCache = new BoxingCache<int>();

            var boxedValue_42_1 = boxingCache.GetBoxedValue(42);
            var boxedValue_42_2 = boxingCache.GetBoxedValue(42);

            Assert.IsTrue(ReferenceEquals(boxedValue_42_1, boxedValue_42_2));

            var boxedValue_84_1 = boxingCache.GetBoxedValue(84);
            var boxedValue_84_2 = boxingCache.GetBoxedValue(84);

            Assert.IsTrue(ReferenceEquals(boxedValue_84_1, boxedValue_84_2));
        }

        [TestCase]
        public void UnboxesCorrectly_Bool()
        {
            var boxingCache = new BoxingCache<bool>();

            var boxedBoolValue_True = (object) true;
            var unboxedBoolValue_True = boxingCache.GetUnboxedValue(boxedBoolValue_True);

            Assert.AreEqual(true, unboxedBoolValue_True);

            var boxedBoolValue_False = (object)false;
            var unboxedBoolValue_False = boxingCache.GetUnboxedValue(boxedBoolValue_False);

            Assert.AreEqual(false, unboxedBoolValue_False);
        }
    }
}