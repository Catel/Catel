// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    /// <summary>
    /// Object extensions class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Determines whether the specified object is a sentinel.
        /// <para />
        /// For more information, see http://stackoverflow.com/questions/3868786/wpf-sentinel-objects-and-how-to-check-for-an-internal-type.
        /// <para />
        /// Original license: CC BY-SA 2.5, compatible with the MIT license.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <returns><c>true</c> if the data context is a sentinel; otherwise, <c>false</c>.</returns>
        public static bool IsSentinelBindingObject(this object dataContext)
        {
            if (dataContext is null)
            {
                return false;
            }

            var type = dataContext.GetType();
            if (string.CompareOrdinal(type.FullName, "MS.Internal.NamedObject") == 0)
            {
                return true;
            }
            
            if (string.CompareOrdinal(dataContext.ToString(), "{DisconnectedObject}") == 0)
            {
                return true;
            }

            if (string.CompareOrdinal(dataContext.ToString(), "{DisconnectedItem}") == 0)
            {
                return true;
            }

            return false;
        }
    }
}
