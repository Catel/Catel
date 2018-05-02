// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Buffer4096.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Pooling
{
    /// <summary>
    /// Poolable buffer of 4096 bytes.
    /// </summary>
    public class Buffer4096Poolable : BufferPoolableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer4096Poolable"/> class.
        /// </summary>
        public Buffer4096Poolable()
            : base(4096)
        {
        }
    }
}