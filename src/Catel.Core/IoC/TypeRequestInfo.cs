namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Request information about a type.
    /// </summary>
    public class TypeRequestInfo
    {
        private int? _hash;
        private string? _string;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeRequestInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public TypeRequestInfo(Type type, object? tag = null)
        {
            Type = type;
            Tag = tag;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object? Tag { get; private set; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="firstObject">The first object.</param>
        /// <param name="secondObject">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TypeRequestInfo firstObject, TypeRequestInfo secondObject)
        {
            if (ReferenceEquals(firstObject, secondObject))
            {
                return true;
            }

            if (ReferenceEquals(null, firstObject))
            {
                return false;
            }

            return firstObject.Equals(secondObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="firstObject">The first object.</param>
        /// <param name="secondObject">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(TypeRequestInfo firstObject, TypeRequestInfo secondObject)
        {
            return !(firstObject == secondObject);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            var other = (TypeRequestInfo) obj;

            if (Type != other.Type)
            {
                return false;
            }

            if (!TagHelper.AreTagsEqual(Tag, other.Tag))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            if (!_hash.HasValue)
            {
#pragma warning disable HAA0101 // Array allocation for params parameter
                _hash = HashHelper.CombineHash(Type.GetHashCode(), (Tag is not null) ? Tag.GetHashCode() : 0);
#pragma warning restore HAA0101 // Array allocation for params parameter
            }

            return _hash.Value;
        }

        /// <summary>
        /// Converts the type to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            if (_string is null)
            {
                _string = string.Format("{0} (tag = {1})", Type.FullName, ObjectToStringHelper.ToString(Tag));
            }

            return _string;
        }
    }
}
