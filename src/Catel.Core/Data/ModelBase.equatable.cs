namespace Catel.Data
{
    using System;

    public partial class ModelBase
    {
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not IModel otherModel)
            {
                return false;
            }

            return Equals(otherModel);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            var propertyValues = _propertyBag.GetAllProperties();

            foreach (var propertyKeyValue in propertyValues)
            {
                hashCode.Add(propertyKeyValue.Value);
            }

            return hashCode.ToHashCode();
        }

        public virtual bool Equals(IModel? other)
        {
            if (other is null)
            {
                return false;
            }

            var propertyValues = _propertyBag.GetAllProperties();

            foreach (var propertyKeyValue in propertyValues)
            {
                var propertyValue = propertyKeyValue.Value;
                var otherValue = other.GetValueFastButUnsecure<object?>(propertyKeyValue.Key);

                if (propertyValue is null && otherValue is null)
                {
                    continue;
                }

                if (propertyValue is null && otherValue is not null)
                {
                    return false;
                }

                if (!propertyValue!.Equals(otherValue))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
