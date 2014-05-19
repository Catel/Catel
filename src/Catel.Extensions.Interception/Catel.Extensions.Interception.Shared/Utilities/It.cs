// --------------------------------------------------------------------------------------------------------------------
// <copyright file="It.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    /// <summary>
    /// The class which helps to get default value of types.
    /// </summary>
    public static class It
    {
        #region Methods
        /// <summary>
        /// Returns any instance.
        /// </summary>
        /// <typeparam name="T">The specified type.</typeparam>
        /// <returns>The default value.</returns>
        public static T IsAny<T>()
        {
            return default(T);
        }
        #endregion
    }
}