// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumToVisibilityConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS || ANDROID

namespace Catel.MVVM.Converters
{
    using System;
    using System.Reflection;
    using System.Windows;
    using Reflection;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#endif

    /// <summary>
    /// Convert from an enum value to <see cref="Visibility"/>. The allowed values must be defined inside the <c>ConverterParameter</c> property.
    /// <para />
    /// If the <c>ConverterParameter</c> starts with a <c>!</c>, the element will not be visible for the specified enum values.
    /// </summary>
    public class EnumToCollapsingVisibilityConverter : VisibilityConverterBase
    {
        private readonly char[] SplitChars = new[] { '|', ',' };

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumToCollapsingVisibilityConverter"/> class.
        /// </summary>
        public EnumToCollapsingVisibilityConverter()
            : this(Visibility.Collapsed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumToCollapsingVisibilityConverter"/> class.
        /// </summary>
        /// <param name="notVisibleVisibility">The <see cref="Visibility"/> state when not visibible should be returned.</param>
        /// <exception cref="ArgumentException">The <paramref name="notVisibleVisibility"/> is <see cref="Visibility.Visible"/>.</exception>
        internal EnumToCollapsingVisibilityConverter(Visibility notVisibleVisibility)
            : base(notVisibleVisibility)
        {
            SupportInversionUsingCommandParameter = false;
        }
        #endregion

        /// <summary>
        /// Determines what value this converter should return.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <returns>
        /// <c>true</c> if the specified value is visible; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsVisible(object value, Type targetType, object parameter)
        {
            if (value == null)
            {
                return false;
            }

            var enumType = value.GetType();
            if (!enumType.IsEnumEx())
            {
                return false;
            }

            var stringParameter = parameter as string;
            if (string.IsNullOrWhiteSpace(stringParameter))
            {
                return false;
            }

            var invert = false;
            if (stringParameter.StartsWith("!"))
            {
                invert = true;
                stringParameter = stringParameter.Substring(1);
            }

            var genericEnumType = typeof(Enum<>).MakeGenericType(enumType);
            var bindingFlags = BindingFlags.Public | BindingFlags.Static;
            
            var parseMethod = genericEnumType.GetMethodEx("Parse", TypeArray.From<string, bool>(), bindingFlags);

            var allowedEnumValues = stringParameter.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
            foreach (var allowedEnumValueAsString in allowedEnumValues)
            {
                var parameters = new object[] { allowedEnumValueAsString, true };

                try
                {
                    var allowedEnumValue = parseMethod.Invoke(null, parameters);
                    if (value.Equals(allowedEnumValue))
                    {
                        return !invert;
                    }
                }
                catch (Exception)
                {
                    // Next!
                }
            }

            return invert;
        }
    }

#if NET
    /// <summary>
    /// Convert from an enum value to <see cref="Visibility"/>. The allowed values must be defined inside the <c>ConverterParameter</c> property.
    /// <para />
    /// If the <c>ConverterParameter</c> starts with a <c>!</c>, the element will not be visible for the specified enum values.
    /// </summary>
    public class EnumToHidingVisibilityConverter : EnumToCollapsingVisibilityConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumToHidingVisibilityConverter"/> class.
        /// </summary>
        public EnumToHidingVisibilityConverter()
            : base(Visibility.Hidden)
        {
        }
    }
#endif
}

#endif