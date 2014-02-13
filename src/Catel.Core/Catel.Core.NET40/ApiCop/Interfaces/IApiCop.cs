// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApiCop.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface to specify writers for the ApiCop functionality.
    /// </summary>
    public interface IApiCop
    {
        /// <summary>
        /// Gets the target type of the ApiCop. This is the type where the ApiCop is created for.
        /// </summary>
        /// <value>The type of the target.</value>
        Type TargetType { get; }

        /// <summary>
        /// Gets the results of this specific ApiCop.
        /// </summary>
        /// <returns>The results of this ApiCop.</returns>
        IEnumerable<IApiCopResult> GetResults();
    }
}