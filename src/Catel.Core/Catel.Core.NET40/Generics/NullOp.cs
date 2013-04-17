// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullOp.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Generics
{
    /// <remarks>
    /// Code originally found at http://www.yoda.arachsys.com/csharp/miscutil/.
    /// </remarks>
    interface INullOp<T>
    {
        bool HasValue(T value);
        bool AddIfNotNull(ref T accumulator, T value);
    }

    /// <remarks>
    /// Code originally found at http://www.yoda.arachsys.com/csharp/miscutil/.
    /// </remarks>
    sealed class StructNullOp<T> : INullOp<T>, INullOp<T?>
        where T : struct
    {
        public bool HasValue(T value)
        {
            return true;
        }

        public bool AddIfNotNull(ref T accumulator, T value)
        {
            accumulator = Operator<T>.Add(accumulator, value);
            return true;
        }

        public bool HasValue(T? value)
        {
            return value.HasValue;
        }

        public bool AddIfNotNull(ref T? accumulator, T? value)
        {
            if (value.HasValue)
            {
                accumulator = accumulator.HasValue ? Operator<T>.Add(accumulator.GetValueOrDefault(), value.GetValueOrDefault()) : value;
                return true;
            }

            return false;
        }
    }

    /// <remarks>
    /// Code originally found at http://www.yoda.arachsys.com/csharp/miscutil/.
    /// </remarks>
    sealed class ClassNullOp<T> : INullOp<T>
        where T : class
    {
        public bool HasValue(T value)
        {
            return value != null;
        }

        public bool AddIfNotNull(ref T accumulator, T value)
        {
            if (value != null)
            {
                accumulator = accumulator == null ? value : Operator<T>.Add(accumulator, value);
                return true;
            }
            return false;
        }
    }
}