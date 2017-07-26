// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferPoolableBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Pooling
{
    using System;

    /// <summary>
    /// Base class for buffer poolables.
    /// </summary>
    public abstract class BufferPoolableBase : PoolableBase
    {
        private readonly int _size;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferPoolableBase"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        protected BufferPoolableBase(int size)
        {
            _size = size;

            Data = new byte[size];
        }

        /// <summary>
        /// Gets the byte array.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        public override int Size { get { return _size; } }

        /// <summary>
        /// Resets the object.
        /// </summary>
        public override void Reset()
        {
            var buffer = Data;
            Array.Clear(buffer, 0, buffer.Length);
        }
    }
}