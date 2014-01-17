// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelEqualityComparer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System.Collections;

    /// <summary>
    /// Implementation of the <see cref="IEqualityComparer" /> for the <see cref="ModelBase" />.
    /// </summary>
    public interface IModelEqualityComparer : IEqualityComparer
    {
        /// <summary>
        /// Gets or sets a value indicating whether properties should be compared.
        /// </summary>
        /// <value><c>true</c> if properties should be compared; otherwise, <c>false</c>.</value>
        bool CompareProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether values should be compared.
        /// </summary>
        /// <value><c>true</c> if values should be compared; otherwise, <c>false</c>.</value>
        bool CompareValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether collections should be compared.
        /// </summary>
        /// <value><c>true</c> if collections should be compared; otherwise, <c>false</c>.</value>
        bool CompareCollections { get; set; }
    }
}