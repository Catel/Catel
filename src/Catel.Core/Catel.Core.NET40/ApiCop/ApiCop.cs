// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiCop.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// ApiCop writer class.
    /// </summary>
    public class ApiCop : IApiCop
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCop"/> class.
        /// </summary>
        /// <param name="targetType">The type for which this ApiCop is intented.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> is <c>null</c>.</exception>
        public ApiCop(Type targetType)
        {
            Argument.IsNotNull("targetType", targetType);

            TargetType = targetType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the target type of the ApiCop. This is the type where the ApiCop is created for.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType { get; private set; }
        #endregion

        /// <summary>
        /// Gets the results of this specific ApiCop.
        /// </summary>
        /// <returns>The results of this ApiCop.</returns>
        public IEnumerable<IApiCopResult> GetResults()
        {
            return new IApiCopResult[] {};
        }
    }
}