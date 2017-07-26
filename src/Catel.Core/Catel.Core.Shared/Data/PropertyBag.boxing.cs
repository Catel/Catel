// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBag.boxing.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;

    public partial class PropertyBag
    {
        private static readonly BoxingCache<bool> BoolBoxingCache = new BoxingCache<bool>();
        private static readonly BoxingCache<short> ShortBoxingCache = new BoxingCache<short>();
        private static readonly BoxingCache<ushort> UShortBoxingCache = new BoxingCache<ushort>();
        private static readonly BoxingCache<int> IntegerBoxingCache = new BoxingCache<int>();
        private static readonly BoxingCache<uint> UIntegerBoxingCache = new BoxingCache<uint>();
        private static readonly BoxingCache<long> LongBoxingCache = new BoxingCache<long>();
        private static readonly BoxingCache<ulong> ULongBoxingCache = new BoxingCache<ulong>();

        // Note: for now we don't store boxed values of floats, doubles, etc

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public void SetPropertyValue(string propertyName, bool value)
        {
            var boxedValue = BoolBoxingCache.GetBoxedValue(value);
            SetPropertyValue(propertyName, boxedValue);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public void SetPropertyValue(string propertyName, short value)
        {
            var boxedValue = ShortBoxingCache.GetBoxedValue(value);
            SetPropertyValue(propertyName, boxedValue);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public void SetPropertyValue(string propertyName, ushort value)
        {
            var boxedValue = UShortBoxingCache.GetBoxedValue(value);
            SetPropertyValue(propertyName, boxedValue);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public void SetPropertyValue(string propertyName, int value)
        {
            var boxedValue = IntegerBoxingCache.GetBoxedValue(value);
            SetPropertyValue(propertyName, boxedValue);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public void SetPropertyValue(string propertyName, uint value)
        {
            var boxedValue = UIntegerBoxingCache.GetBoxedValue(value);
            SetPropertyValue(propertyName, boxedValue);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public void SetPropertyValue(string propertyName, long value)
        {
            var boxedValue = LongBoxingCache.GetBoxedValue(value);
            SetPropertyValue(propertyName, boxedValue);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public void SetPropertyValue(string propertyName, ulong value)
        {
            var boxedValue = ULongBoxingCache.GetBoxedValue(value);
            SetPropertyValue(propertyName, boxedValue);
        }

        ///// <summary>
        ///// Sets the property value.
        ///// </summary>
        ///// <param name="propertyName">Name of the property.</param>
        ///// <param name="value">The value.</param>
        ///// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        //public void SetPropertyValue<TValue>(string propertyName, TValue value)
        //    where TValue : struct
        //{
        //    var boxedValue = ULongBoxingCache.Get(value);
        //    SetPropertyValue(propertyName, boxedValue);
        //}
    }
}