﻿namespace Catel.Data
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using Catel.Runtime.Serialization;

    /// <summary>
    /// Comparable model base.
    /// </summary>
    public abstract class ComparableModelBase : ModelBase
    {
        /// <summary>
        /// Backing field for the <see cref="GetHashCode"/> method so it only has to be calculated once to gain the best performance possible.
        /// </summary>
        private int? _hashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparableModelBase"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        protected ComparableModelBase(ISerializer serializer, IModelEqualityComparer equalityComparer)
            : base(serializer)
        {
            EqualityComparer = equalityComparer;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="firstObject">The first object.</param>
        /// <param name="secondObject">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ComparableModelBase firstObject, ComparableModelBase secondObject)
        {
            return Equals(firstObject, secondObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="firstObject">The first object.</param>
        /// <param name="secondObject">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ComparableModelBase firstObject, ComparableModelBase secondObject)
        {
            return !(firstObject == secondObject);
        }

        /// <summary>
        /// Gets or sets the equality comparer used to compare model bases with each other.
        /// </summary>
        /// <value>The equality comparer.</value>
        [Browsable(false)]
        [XmlIgnore]
        protected IModelEqualityComparer EqualityComparer { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object? obj)
        {
            // Note: at first we only implemented the EqualityComparer, but the IEqualityComparer of Microsoft
            // throws an exception when the 2 types are not the same. Although MS does recommend not to throw exceptions,
            // they do it themselves. Check for null and check the types before feeding it to the equality comparer.

            if (obj is null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var equalityComparer = EqualityComparer;
            return equalityComparer.Equals(this, obj);
        }

        // ReSharper disable RedundantOverridenMember

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                var equalityComparer = EqualityComparer;
                _hashCode = equalityComparer.GetHashCode(this);
            }

            return _hashCode.Value;
        }
    }
}
