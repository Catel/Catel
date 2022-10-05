namespace Catel
{
    using System;

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
