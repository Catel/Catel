﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseEqualityComparer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Catel.Collections;

    /// <summary>
    /// Implementation of the <see cref="EqualityComparer{T}" /> for the <see cref="ModelBase" />.
    /// </summary>
    public class ModelEqualityComparer : EqualityComparer<ModelBase>, IModelEqualityComparer
    {
        /// <summary>
        /// The property data manager.
        /// </summary>
        private static readonly PropertyDataManager PropertyDataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelEqualityComparer" /> class.
        /// </summary>
        static ModelEqualityComparer()
        {
            PropertyDataManager = PropertyDataManager.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelEqualityComparer"/> class.
        /// </summary>
        public ModelEqualityComparer()
        {
            CompareProperties = false;
            CompareValues = false;
            CompareCollections = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether properties should be compared.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if properties should be compared; otherwise, <c>false</c>.</value>
        public bool CompareProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether values should be compared as well.
        /// <para />
        /// Note that this might degrade performance on properties with large collections.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if values should be compared; otherwise, <c>false</c>.</value>
        public bool CompareValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether collections should be compared as well.
        /// <para />
        /// Note that this might degrade performance on properties with large collections.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if collections should be compared; otherwise, <c>false</c>.</value>
        public bool CompareCollections { get; set; }

        /// <summary>
        /// When overridden in a derived class, determines whether two objects of type <see cref="ModelBase" /> are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Equals(ModelBase x, ModelBase y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (((object)x == null) || ((object)y == null))
            {
                return false;
            }

            // Fix for issue 6633 (see http://catel.codeplex.com/workitem/6633)
            // Check types before the "expensive" operation of checking all property values
            var xType = x.GetType();
            var yType = y.GetType();
            if (xType != yType)
            {
                return false;
            }

            if (!CompareProperties)
            {
                return false;
            }

            lock (x._propertyValuesLock)
            {
                foreach (var propertyValue in x._propertyBag.GetAllProperties())
                {
                    var propertyData = PropertyDataManager.GetPropertyData(xType, propertyValue.Key);

                    // Only check if this is not an internal data object base property
                    if (!propertyData.IsModelBaseProperty)
                    {
                        object valueA = propertyValue.Value;
                        object valueB = y.GetValue(propertyValue.Key);

                        if (!ReferenceEquals(valueA, valueB))
                        {
                            if ((valueA == null) || (valueB == null))
                            {
                                return false;
                            }

                            // Is this an IEnumerable (but not a string)?
                            var valueAAsIEnumerable = valueA as IEnumerable;
                            if ((valueAAsIEnumerable != null) && !(valueA is string))
                            {
                                // Yes, loop all sub items and check them
                                if (CompareCollections)
                                {
                                    if (!CollectionHelper.IsEqualTo(valueAAsIEnumerable, (IEnumerable)valueB))
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                // No, check objects via equals method
                                if (CompareValues)
                                {
                                    if (!valueA.Equals(valueB))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object for which to get a hash code.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode(ModelBase obj)
        {
            if (obj == null)
            {
                return 0;
            }

            var objType = obj.GetType();
            return objType.FullName.GetHashCode();
        }
    }
}