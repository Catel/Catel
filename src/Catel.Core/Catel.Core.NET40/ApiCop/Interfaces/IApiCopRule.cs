// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApiCopRule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    /// <summary>
    /// Interface defining ApiCop rules.
    /// </summary>
    public interface IApiCopRule
    {
        /// <summary>
        /// Determines whether the specified ApiCop rule is valid.
        /// </summary>
        /// <param name="apiCop">The ApiCop.</param>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the specified ApiCop is valid; otherwise, <c>false</c>.</returns>
        bool IsValid(IApiCop apiCop, object tag);
    }
}