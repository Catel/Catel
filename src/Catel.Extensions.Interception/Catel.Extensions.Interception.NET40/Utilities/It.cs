// --------------------------------------------------------------------------------------------------------------------
// <copyright file="It.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class It
    {
        #region Methods
        /// <summary>
        /// Returns any instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Any<T>()
        {
            return default(T);
        }
        #endregion
    }
}