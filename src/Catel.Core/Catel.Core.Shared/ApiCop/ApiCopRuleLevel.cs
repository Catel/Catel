// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiRuleLevel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    /// <summary>
    /// Enum containing the ApiCop rule levels.
    /// </summary>
    public enum ApiCopRuleLevel
    {
        /// <summary>
        /// Following the hints of this rule might improve the usage of the Api, but can be ignored if too much work.
        /// </summary>
        Hint,

        /// <summary>
        /// Following the hints of this rule might improve the usage of the Api.
        /// </summary>
        Warning,

        /// <summary>
        /// This rule must be taken seriously.
        /// </summary>
        Error
    }
}