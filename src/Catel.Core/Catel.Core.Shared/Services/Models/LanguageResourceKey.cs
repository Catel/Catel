// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageResourceKey.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Language resource key.
    /// </summary>
    public class LanguageResourceKey : IEquatable<LanguageResourceKey>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageResourceKey"/> class.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="cultureInfo">The culture information.</param>
        public LanguageResourceKey(string resourceName, CultureInfo cultureInfo)
        {
            ResourceName = resourceName;
            CultureInfo = cultureInfo;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the resource.
        /// </summary>
        /// <value>The name of the resource.</value>
        public string ResourceName { get; private set; }

        /// <summary>
        /// Gets the culture information.
        /// </summary>
        /// <value>The culture information.</value>
        public CultureInfo CultureInfo { get; private set; }
        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((LanguageResourceKey)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(LanguageResourceKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(ResourceName, other.ResourceName) && CultureInfo.Equals(other.CultureInfo);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (ResourceName.GetHashCode() * 397) ^ CultureInfo.GetHashCode();
            }
        }
    }
}