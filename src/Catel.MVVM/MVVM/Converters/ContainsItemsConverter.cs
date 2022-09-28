namespace Catel.MVVM.Converters
{
    using System;
    using System.Collections;
    using Catel.Data;

    /// <summary>
    /// Converter that converts whether a collection contains items or not.
    /// </summary>
    [System.Windows.Data.ValueConversion(typeof(IEnumerable), typeof(bool))]
    public class ContainsItemsConverter : ValueConverterBase
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var containsItems = false;

            if (value is not null)
            {
                var collection = value as ICollection;
                if (collection is not null && collection.Count > 0)
                {
                    containsItems = true;
                }

                var enumerable = value as IEnumerable;
                if (!containsItems && enumerable is not null)
                {
                    // TODO: Would MoveNext + reset be better?
                    //var item = enumerable.GetEnumerator();
                    //item.

                    foreach (object obj in enumerable)
                    {
                        containsItems = true;
                        break;
                    }
                }
            }

            if (SupportInversionUsingCommandParameter && ConverterHelper.ShouldInvert(parameter))
            {
                containsItems = !containsItems;
            }

            return BoxingCache.GetBoxedValue(containsItems);
        }
    }
}
