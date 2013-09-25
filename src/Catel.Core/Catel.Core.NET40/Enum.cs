// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enum.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using Catel.Reflection;

    /// <summary>
    /// Generic enumeration wrapper.
    /// </summary>
    /// <typeparam name="TEnum">Type of the enumeration to wrap.</typeparam>
    public static class Enum<TEnum>
        where TEnum : struct, IComparable, IFormattable
    {
        #region Static Methods
        /// <summary>
        /// Converts an enumaration to a list.
        /// </summary>
        /// <returns><see cref="List{TEnum}"/> containing all the values.</returns>
        public static List<TEnum> ToList()
        {
            return GetValues().ToList();
        }

        /// <summary>
        /// Converts a specific enum value from one specific enum type to another enum type by it's name.
        /// <para/>
        /// For example, to convert <c>Catel.MVVM.Services.CameraType</c> to <c>Microsoft.Devices.CameraType</c>, use the
        /// following code:
        /// <para/>
        /// ConvertEnum&lt;Microsoft.Devices.CameraType&gt;(Catel.MVVM.Services.CameraType.Primary);
        /// </summary>
        /// <param name="inputEnumValue">The input enum value.</param>
        /// <returns>The converted enum value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="inputEnumValue"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="inputEnumValue"/> is not of type <see cref="Enum"/>.</exception>
        /// <exception cref="ArgumentException">The value of <paramref name="inputEnumValue"/> cannot be converted to a value of <typeparamref name="TEnum"/>.</exception>
        public static TEnum ConvertFromOtherEnumValue(object inputEnumValue)
        {
            Argument.IsNotNull("inputEnumValue", inputEnumValue);
            Argument.IsOfType("inputEnumValue", inputEnumValue, typeof (Enum));

            string value = inputEnumValue.ToString();

            return (TEnum) Enum.Parse(typeof (TEnum), value, true);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The name of the value.</returns>
        public static string GetName(int value)
        {
            return Enum.GetName(typeof (TEnum), value);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The name of the value.</returns>
        public static string GetName(long value)
        {
            return Enum.GetName(typeof (TEnum), value);
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <returns>Array of names of an enum.</returns>
        public static string[] GetNames()
        {
            var enumType = typeof (TEnum);

            var fields = from field in GetFields(enumType)
                         select field.Name;

            return fields.ToArray();
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns><see cref="List{TEnum}"/> of values.</returns>
        public static List<TEnum> GetValues()
        {
            var enumType = typeof (TEnum);

            var fields = GetFields(enumType);

            return (from field in fields
                    select field.GetValue(enumType)).Cast<TEnum>().ToList();
        }

        /// <summary>
        /// Gets the fields from an enum.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns>Array of <see cref="FieldInfo"/> values.</returns>
        private static FieldInfo[] GetFields(Type enumType)
        {
            var fields = from field in enumType.GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(true, true))
                         where field.IsLiteral
                         select field;

            return fields.ToArray();
        }

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, the case should be ignored.</param>
        /// <returns>The enum value.</returns>
        public static TEnum Parse(string input, bool ignoreCase = false)
        {
            return (TEnum) Enum.Parse(typeof (TEnum), input, ignoreCase);
        }

        /// <summary>
        /// Tries to parse an enum value name.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public static bool TryParse(string input, out TEnum? result)
        {
            result = null;
            if (!Enum.IsDefined(typeof (TEnum), input))
            {
                return false;
            }

            try
            {
                result = (TEnum)Enum.Parse(typeof(TEnum), input, true);    
            }
            catch (Exception)
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Tries to parse an enum value name.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public static bool TryParse(string input, out TEnum result)
        {
            TEnum? temp;
            if (!TryParse(input, out temp))
            {
                // input not found in the Enum, fill the out parameter with the first item from the enum
                var values = GetValues().ToArray();

                result = (TEnum) values.GetValue(values.GetLowerBound(0));
                return false;
            }

            result = temp.Value;
            return true;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The name of the value.</returns>
        private static string GetName(TEnum value)
        {
            return Enum.GetName(typeof (TEnum), value);
        }
        #endregion

        #region Nested type: DataBinding
        /// <summary>
        /// DataBinding class.
        /// </summary>
        public static class DataBinding
        {
            #region Delegates
            /// <summary>
            /// Delegate used for formatting an enum name.
            /// </summary>
            /// <param name="value">The value to format.</param>
            /// <returns>String containing the enum name.</returns>
            public delegate string FormatEnumName(TEnum value);
            #endregion

            #region Static Methods
#if NET
            /// <summary>
            /// Creates a list based on an enum.
            /// </summary>
            /// <param name="formatName">Name of the format.</param>
            /// <returns>List containing bindable enums based on the format name.</returns>
            public static IList<IBindableEnum<TEnum>> CreateList(FormatEnumName formatName = null)
            {
                Array values = Enum.GetValues(typeof (TEnum));

                return (from TEnum value in values
                        select formatName != null ? new InternalBindableEnum(value, formatName(value)) : new InternalBindableEnum(value)).Cast<IBindableEnum<TEnum>>().ToList();
            }
#endif
            #endregion

            #region Nested type: InternalBindableEnum
            /// <summary>
            /// Internal bindable enum.
            /// </summary>
            private class InternalBindableEnum : IBindableEnum<TEnum>
            {
                #region Readonly & Static Fields
                /// <summary>
                /// Name of the internal bindable enum.
                /// </summary>
                private readonly string _name;
                #endregion

                #region Fields
                /// <summary>
                /// Value of the internal bindable enum.
                /// </summary>
                private TEnum _value;
                #endregion

                #region Constructors
                /// <summary>
                /// Initializes a new instance of the <see cref="Enum{TEnum}.DataBinding.InternalBindableEnum"/> class.
                /// </summary>
                /// <param name="value">The value.</param>
                public InternalBindableEnum(TEnum value)
                {
                    _value = value;
                    _name = Enum<TEnum>.GetName(value);
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="Enum{TEnum}.DataBinding.InternalBindableEnum"/> class.
                /// </summary>
                /// <param name="value">The value of the internal bindable enum.</param>
                /// <param name="name">The name of the internal bindable enum.</param>
                public InternalBindableEnum(TEnum value, string name)
                {
                    _value = value;
                    _name = name;
                }
                #endregion

                #region Instance Properties
                /// <summary>
                /// Gets the name.
                /// </summary>
                /// <value>The name of the enum.</value>
                public string Name
                {
                    get { return _name; }
                }

                /// <summary>
                /// Gets the value.
                /// </summary>
                /// <value>The value of the enum.</value>
                public TEnum Value
                {
                    get { return _value; }
                }
                #endregion

                #region Instance Public Methods
                /// <summary>
                /// Compares the current object with another object of the same type.
                /// </summary>
                /// <param name="other">An object to compare with this object.</param>
                /// <returns>
                /// A 32-bit signed integer that indicates the relative order of the objects being compared.
                /// The return value has the following meanings: Value Meaning Less than zero This object is less
                /// than the other parameter.  Zero This object is equal to other. Greater than zero This object is
                /// greater than other.
                /// </returns>
                public int CompareTo(IBindableEnum<TEnum> other)
                {
                    return _value.CompareTo(other);
                }

                /// <summary>
                /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
                /// </summary>
                /// <param name="other">The <see cref="System.Object"/> to compare with this instance.</param>
                /// <returns>
                /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
                /// </returns>
                /// <exception cref="T:System.NullReferenceException">
                /// The <paramref name="other"/> parameter is null.
                /// </exception>
                public bool Equals(IBindableEnum<TEnum> other)
                {
                    return _value.Equals(other.Value);
                }
                #endregion
            }
            #endregion
        }
        #endregion

        #region Nested type: Flags
        /// <summary>
        /// Flags class.
        /// </summary>
        public static class Flags
        {
            #region Static Methods
            /// <summary>
            /// Clears the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToClear">The flag to clear.</param>
            /// <returns>Flags without the flag that should be cleared.</returns>
            public static TEnum ClearFlag(TEnum flags, TEnum flagToClear)
            {
                // all other integral types except unsigned* which are not yet supported....
                return ClearFlag(Convert.ToInt32(flags, CultureInfo.CurrentCulture), Convert.ToInt32(flagToClear, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Clears the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToClear">The flag to clear.</param>
            /// <returns>Flags without the flag that should be cleared.</returns>
            public static TEnum ClearFlag(int flags, TEnum flagToClear)
            {
                return ClearFlag(flags, Convert.ToInt32(flagToClear, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Clears the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToClear">The flag to clear.</param>
            /// <returns>Flags without the flag that should be cleared.</returns>
            public static TEnum ClearFlag(long flags, TEnum flagToClear)
            {
                return ClearFlag(flags, Convert.ToInt64(flagToClear, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Clears the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToClear">The flag to clear.</param>
            /// <returns>Flags without the flag that should be cleared.</returns>
            public static TEnum ClearFlag(int flags, int flagToClear)
            {
// ReSharper disable RedundantCast
                return ClearFlag((long) flags, (long) flagToClear);
// ReSharper restore RedundantCast
            }

            /// <summary>
            /// Clears the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToClear">The flag to clear.</param>
            /// <returns>Flags without the flag that should be cleared.</returns>
            public static TEnum ClearFlag(long flags, long flagToClear)
            {
                if (IsFlagSet(flags, flagToClear))
                {
                    flags &= ~flagToClear;
                }

                return (TEnum) Enum.ToObject(typeof (TEnum), flags);
            }

            /// <summary>
            /// Determines whether a specific flag is set.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToFind">The flag to find.</param>
            /// <returns>
            /// 	<c>true</c> if the flag is set; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsFlagSet(TEnum flags, TEnum flagToFind)
            {
                return IsFlagSet(Convert.ToInt32(flags, CultureInfo.CurrentCulture), Convert.ToInt32(flagToFind, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Determines whether a specific flag is set.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToFind">The flag to find.</param>
            /// <returns>
            /// 	<c>true</c> if the flag is set; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsFlagSet(int flags, TEnum flagToFind)
            {
                return IsFlagSet(Convert.ToInt32(flags, CultureInfo.CurrentCulture), Convert.ToInt32(flagToFind, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Determines whether a specific flag is set.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToFind">The flag to find.</param>
            /// <returns>
            /// 	<c>true</c> if the flag is set; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsFlagSet(long flags, TEnum flagToFind)
            {
                return IsFlagSet(Convert.ToInt64(flags, CultureInfo.CurrentCulture), Convert.ToInt32(flagToFind, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Determines whether a specific flag is set.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToFind">The flag to find.</param>
            /// <returns>
            /// 	<c>true</c> if the flag is set; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsFlagSet(int flags, int flagToFind)
            {
                return (flags & flagToFind) == flagToFind;
            }

            /// <summary>
            /// Determines whether a specific flag is set.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToFind">The flag to find.</param>
            /// <returns>
            /// 	<c>true</c> if the flag is set; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsFlagSet(long flags, long flagToFind)
            {
                return (flags & flagToFind) == flagToFind;
            }

            /// <summary>
            /// Sets the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToSet">The flag to set.</param>
            /// <returns>Flags with the flag that should be set.</returns>
            public static TEnum SetFlag(TEnum flags, TEnum flagToSet)
            {
                return SetFlag(Convert.ToInt32(flags, CultureInfo.CurrentCulture), Convert.ToInt32(flagToSet, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Sets the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToSet">The flag to set.</param>
            /// <returns>Flags with the flag that should be set.</returns>
            public static TEnum SetFlag(int flags, TEnum flagToSet)
            {
                return SetFlag(flags, Convert.ToInt32(flagToSet, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Sets the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToSet">The flag to set.</param>
            /// <returns>Flags with the flag that should be set.</returns>
            public static TEnum SetFlag(long flags, TEnum flagToSet)
            {
                return SetFlag(flags, Convert.ToInt64(flagToSet, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Sets the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToSet">The flag to set.</param>
            /// <returns>Flags with the flag that should be set.</returns>
            public static TEnum SetFlag(int flags, int flagToSet)
            {
// ReSharper disable RedundantCast
                return SetFlag((long) flags, (long) flagToSet);
// ReSharper restore RedundantCast
            }

            /// <summary>
            /// Sets the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToSet">The flag to set.</param>
            /// <returns>Flags with the flag that should be set.</returns>
            public static TEnum SetFlag(long flags, long flagToSet)
            {
                if (!IsFlagSet(flags, flagToSet))
                {
                    flags |= flagToSet;
                }

                return (TEnum) Enum.ToObject(typeof (TEnum), flags);
            }

            /// <summary>
            /// Swaps the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToSwap">The flag to swap.</param>
            /// <returns>Flags with the flag swapped that should be swapped.</returns>
            public static TEnum SwapFlag(TEnum flags, TEnum flagToSwap)
            {
                return SwapFlag(Convert.ToInt32(flags, CultureInfo.CurrentCulture), Convert.ToInt32(flagToSwap, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Swaps the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToSwap">The flag to swap.</param>
            /// <returns>Flags with the flag swapped that should be swapped.</returns>
            public static TEnum SwapFlag(int flags, TEnum flagToSwap)
            {
                return SwapFlag(flags, Convert.ToInt32(flagToSwap, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Swaps the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToSwap">The flag to swap.</param>
            /// <returns>Flags with the flag swapped that should be swapped.</returns>
            public static TEnum SwapFlag(long flags, TEnum flagToSwap)
            {
                return SwapFlag(flags, Convert.ToInt64(flagToSwap, CultureInfo.CurrentCulture));
            }

            /// <summary>
            /// Swaps the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToSwap">The flag to swap.</param>
            /// <returns>Flags with the flag swapped that should be swapped.</returns>
            public static TEnum SwapFlag(int flags, int flagToSwap)
            {
// ReSharper disable RedundantCast
                return SwapFlag((long) flags, (long) flagToSwap);
// ReSharper restore RedundantCast
            }

            /// <summary>
            /// Swaps the flag.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="flagToSwap">The flag to swap.</param>
            /// <returns>Flags with the flag swapped that should be swapped.</returns>
            public static TEnum SwapFlag(long flags, long flagToSwap)
            {
                if (IsFlagSet(flags, flagToSwap))
                {
                    return ClearFlag(flags, flagToSwap);
                }

                return SetFlag(flags, flagToSwap);
            }
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// Bindable enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    public interface IBindableEnum<TEnum> : IComparable<IBindableEnum<TEnum>>, IEquatable<IBindableEnum<TEnum>>
        where TEnum : struct, IComparable, IFormattable
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name of the bindable enum.</value>
        string Name { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value of the bindable enum.</value>
        TEnum Value { get; }
    }
}