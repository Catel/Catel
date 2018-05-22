// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuspensionContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Suspension context that can track properties during a suspension period.
    /// </summary>
    public class SuspensionContext
    {
        private readonly HashSet<string> _hashSet = new HashSet<string>();

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public IEnumerable<string> Properties
        {
            get
            {
                lock (_hashSet)
                {
                    return _hashSet.ToArray();
                }
            }
        }

        /// <summary>
        /// Gets the counter.
        /// </summary>
        /// <value>
        /// The counter.
        /// </value>
        public int Counter { get; private set; }

        /// <summary>
        /// Increments this instance.
        /// </summary>
        public void Increment()
        {
            Counter++;
        }

        /// <summary>
        /// Decrements this instance.
        /// </summary>
        public void Decrement()
        {
            if (Counter > 0)
            {
                Counter--;
            }
        }

        /// <summary>
        /// Adds the specified property name to the suspension context.
        /// </summary>
        /// <param name="propertyName">Name of the property. If <c>null</c>, this will be converted to <c>string.Empty</c>.</param>
        public void Add(string propertyName)
        {
            lock (_hashSet)
            {
                // We can't store null, but we need to raise string.Empty
                if (propertyName == null)
                {
                    propertyName = string.Empty;
                }

                if (!_hashSet.Contains(propertyName))
                {
                    _hashSet.Add(propertyName);
                }
            }
        }
    }
}