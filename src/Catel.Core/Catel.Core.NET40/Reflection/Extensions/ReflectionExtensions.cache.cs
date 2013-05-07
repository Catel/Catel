// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cache.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Text;
    using Text;

    /// <summary>
    /// Reflection extensions cache info.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        /// <summary>
        /// The reflection types.
        /// </summary>
        private enum ReflectionTypes
        {
            /// <summary>
            /// The constructor.
            /// </summary>
            Constructor,

            /// <summary>
            /// The field.
            /// </summary>
            Field,

            /// <summary>
            /// The property.
            /// </summary>
            Property,

            /// <summary>
            /// The event.
            /// </summary>
            Event,

            /// <summary>
            /// The method.
            /// </summary>
            Method
        }

        /// <summary>
        /// The reflection cache key.
        /// </summary>
        private class ReflectionCacheKey
        {
            private readonly int _hashCode;

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="ReflectionCacheKey"/> class. 
            /// </summary>
            /// <param name="type">
            /// The type.
            /// </param>
            /// <param name="reflectionType">
            /// The reflection type.
            /// </param>
            /// <param name="bindingFlags">
            /// The binding flags.
            /// </param>
            /// <param name="additionalInfo">
            /// The additional info.
            /// </param>
            public ReflectionCacheKey(Type type, ReflectionTypes reflectionType, BindingFlags bindingFlags, object additionalInfo = null)
            {
                Type = type;
                ReflectionType = reflectionType;
                BindingFlags = bindingFlags;
                AdditionalInfo = additionalInfo;

                _hashCode = CalculateHashCode();
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets Type.
            /// </summary>
            public Type Type { get; private set; }

            /// <summary>
            /// Gets ReflectionType.
            /// </summary>
            public ReflectionTypes ReflectionType { get; private set; }

            /// <summary>
            /// Gets BindingFlags.
            /// </summary>
            public BindingFlags BindingFlags { get; private set; }

            /// <summary>
            /// Gets AdditionalInfo.
            /// </summary>
            public object AdditionalInfo { get; private set; }
            #endregion

            #region Methods
            /// <summary>
            /// Equalses the specified other.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns></returns>
            protected bool Equals(ReflectionCacheKey other)
            {
                return Equals(Type, other.Type) &&
                       ReflectionType == other.ReflectionType &&
                       BindingFlags == other.BindingFlags &&
                       Equals(AdditionalInfo, other.AdditionalInfo) &&
                       true;
            }

            /// <summary>
            /// Calculates the hash code.
            /// </summary>
            /// <returns>The hash code.</returns>
            private int CalculateHashCode()
            {
                unchecked
                {
                    int hashCode = (Type != null ? Type.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (int)ReflectionType;
                    hashCode = (hashCode * 397) ^ (int)BindingFlags;
                    hashCode = (hashCode * 397) ^ (AdditionalInfo != null ? AdditionalInfo.GetHashCode() : 0);
                    return hashCode;
                }
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return _hashCode;
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
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
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((ReflectionCacheKey)obj);
            }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The to string.
            /// </returns>
            public override string ToString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Type:{0}", Type.AssemblyQualifiedName);
                stringBuilder.AppendLine("ReflectionType:{0}", (int)ReflectionType);
                stringBuilder.AppendLine("BindingFlags:{0}", (int)BindingFlags);
                if (AdditionalInfo != null)
                {
                    stringBuilder.Append("AdditionalInfo:");

                    var additionalInfoAsEnumerable = AdditionalInfo as IEnumerable;
                    if (additionalInfoAsEnumerable == null || AdditionalInfo is string)
                    {
                        stringBuilder.Append(AdditionalInfo);
                    }
                    else
                    {
                        foreach (object additionalInfoItem in additionalInfoAsEnumerable)
                        {
                            stringBuilder.AppendFormat("{0}|", additionalInfoItem);
                        }
                    }
                }

                return stringBuilder.ToString();
            }
            #endregion
        }
    }
}