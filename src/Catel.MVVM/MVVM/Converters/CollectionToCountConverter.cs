namespace Catel.MVVM.Converters
{
    using System;
    using System.Collections;
    using Catel.Data;

    /// <summary>
    /// Converts the items inside a collection to the number of elements contained by the collection.
    /// </summary>
    public class CollectionToCountConverter : ValueConverterBase
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object? Convert(object? value, Type targetType, object? parameter)
        {
            var collection = value as ICollection;
            if (collection is not null)
            {
                return BoxingCache.GetBoxedValue(collection.Count);
            }

            return BoxingCache.GetBoxedValue(0);
        }
    }
}
