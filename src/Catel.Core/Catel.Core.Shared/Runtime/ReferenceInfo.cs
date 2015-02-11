﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime
{
    /// <summary>
    /// Info about a reference which can provide a unique reference.
    /// </summary>
    public class ReferenceInfo
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceInfo" /> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="id">The unique identifier.</param>
        /// <param name="isFirstUsage">if set to <c>true</c>, this is the first usage of this instance.</param>
        public ReferenceInfo(object instance, int id, bool isFirstUsage)
        {
            Instance = instance;
            Id = id;
            IsFirstUsage = isFirstUsage;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public object Instance { get; private set; }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>The unique identifier.</value>
        public int Id { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is the first usage of this instance.
        /// </summary>
        /// <value><c>true</c> if this instance is the first usage of this instance; otherwise, <c>false</c>.</value>
        public bool IsFirstUsage { get; internal set; }
        #endregion
    }
}