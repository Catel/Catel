// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lazy.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace System
{
    using Catel;

    /// <summary>
    /// Implementation of the Lazy class which is not available in Windows Phone 7.
    /// </summary>
    public class Lazy<T>
    {
        private readonly Func<T> _valueFactory;
        private bool _valueCreated;
        private T _value;

        private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="valueFactory"/> is <c>null</c>.</exception>
        public Lazy(Func<T> valueFactory)
        {
            Argument.IsNotNull("valueFactory", valueFactory);

            _valueFactory = valueFactory;
        }

        /// <summary>
        /// Gets a value that indicates whether a value has been created for this instance.
        /// </summary>
        public bool IsValueCreated
        {
            get
            {
                lock (_lock)
                {
                    return _valueCreated;
                }
            }
        }

        /// <summary>
        /// Gets the lazily initialized value of the current instance.
        /// </summary>
        public T Value
        {
            get
            {
                lock (_lock)
                {
                    if (!_valueCreated)
                    {
                        _value = _valueFactory();
                        _valueCreated = true;
                    }

                    return _value;
                }
            }
        }
    }
}