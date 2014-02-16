// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApiCopListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface defining ApiCop listeners.
    /// </summary>
    public interface IApiCopListener
    {
        /// <summary>
        /// Writes the results of the ApiCop feature.
        /// <para />
        /// Note that this will only contain invalid results. Valid results are not written to the
        /// listeners.
        /// </summary>
        /// <param name="results">The results.</param>
        void WriteResults(IEnumerable<IApiCopResult> results);
    }
}