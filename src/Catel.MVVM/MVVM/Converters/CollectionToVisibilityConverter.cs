namespace Catel.MVVM.Converters;

using System;
using System.Collections;
using System.Windows;

/// <summary>
/// Converts the count of a collection to the visibility.
/// </summary>
public partial class CollectionToCollapsingVisibilityConverter : VisibilityConverterBase
{
    /// <summary>
    /// Determines whether the specified value is visible.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns><c>true</c> if the specified value is visible; otherwise, <c>false</c>.</returns>
    protected override bool IsVisible(object? value, Type targetType, object? parameter)
    {
        var isVisible = false;

        var collection = value as ICollection;
        if (collection is not null)
        {
            isVisible = collection.Count > 0;
        }

        if (!isVisible && value is string)
        {
            isVisible = ((string)value).Length > 0;
        }

        if (!isVisible && value is long)
        {
            isVisible = ((long)value) > 0;
        }

        if (!isVisible && value is int)
        {
            isVisible = ((int)value) > 0;
        }

        if (!isVisible && value is short)
        {
            isVisible = ((short)value) > 0;
        }

        // Note: base class will invert if needed

        return isVisible;
    }
}

/// <summary>
/// Convert from bool to <see cref="T:System.Windows.Visibility" /> and back.
/// The bool value true will be converted to Visibility.Visible.
/// The bool value false will be converted to Visibility.Hidden.
/// </summary>
[System.Windows.Data.ValueConversion(typeof(bool), typeof(Visibility))]
public class CollectionToHidingVisibilityConverter : CollectionToCollapsingVisibilityConverter
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CollectionToHidingVisibilityConverter()
    {
        NotVisibleVisibility = Visibility.Hidden;
    }
}
