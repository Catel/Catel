// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisabledApiCop.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Disabled implementation of the <see cref="IApiCop"/> which is used in production environment.
    /// </summary>
    public class DisabledApiCop : IApiCop
    {
        #region IApiCop Members
        /// <summary>
        /// Gets the target type of the ApiCop. This is the type where the ApiCop is created for.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Gets the results of this specific ApiCop.
        /// </summary>
        /// <returns>The results of this ApiCop.</returns>
        public IEnumerable<IApiCopResult> GetResults()
        {
            return new IApiCopResult[] {};
        }
        #endregion
    }
}