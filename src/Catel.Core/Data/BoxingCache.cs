// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoxingCache.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using Catel.Reflection;

    /// <summary>
    /// Boxing cache helper.
    /// </summary>
    public static partial class BoxingCache
    {
        // Partial class, see T4 template
    }

    /// <summary>
    /// Caches boxed objects to minimize the memory footprint for boxed value types.
    /// </summary>
    public class BoxingCache<T>
    {
        private readonly Dictionary<T, object> _boxedValues = new Dictionary<T, object>();
        //private readonly Dictionary<object, T> _unboxedValues = new Dictionary<object, T>();

        /// <summary>
        /// Gets the default instance of the boxing cache.
        /// </summary>
        public static BoxingCache<T> Default { get; private set; } = new BoxingCache<T>();

        /// <summary>
        /// Adds the value to the cache.
        /// </summary>
        /// <param name="value">The value to add to the cache.</param>
        protected object AddUnboxedValue(T value)
        {
            var boxedValue = (object)value;

            lock (_boxedValues)
            {
                _boxedValues[value] = boxedValue;
            }

            //lock (_unboxedValues)
            //{
            //    _unboxedValues[boxedValue] = value;
            //}

            return boxedValue;
        }

        /// <summary>
        /// Adds the value to the cache.
        /// </summary>
        /// <param name="boxedValue">The value to add to the cache.</param>
        protected T AddBoxedValue(object boxedValue)
        {
            var unboxedValue = (T)boxedValue;

            lock (_boxedValues)
            {
                _boxedValues[unboxedValue] = boxedValue;
            }

            //lock (_unboxedValues)
            //{
            //    _unboxedValues[boxedValue] = unboxedValue;
            //}

            return unboxedValue;
        }

        /// <summary>
        /// Gets the boxed value representing the specified value.
        /// </summary>
        /// <param name="value">The value to box.</param>
        /// <returns>The boxed value.</returns>
        public object GetBoxedValue(T value)
        {
            if (value is null)
            {
                return null;
            }

            lock (_boxedValues)
            {
                if (!_boxedValues.TryGetValue(value, out var boxedValue))
                {
                    boxedValue = AddUnboxedValue(value);
                }

                return boxedValue;
            }
        }

        /// <summary>
        /// Gets the unboxed value representing the specified value.
        /// </summary>
        /// <param name="boxedValue">The value to unbox.</param>
        /// <returns>The unboxed value.</returns>
        public T GetUnboxedValue(object boxedValue)
        {
            return (T)boxedValue;
            //lock (_unboxedValues)
            //{
            //    if (!_unboxedValues.TryGetValue(boxedValue, out var unboxedValue))
            //    {
            //        unboxedValue = AddBoxedValue(boxedValue);
            //    }

            //    return unboxedValue;
            //}
        }
    }
}
