// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    /// <summary>
    /// Hash helper class to generate hashCodes for objects.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Combine multiple hashcodes in to one.
        /// </summary>
        /// <param name="hashCodes">An array of hashcodes.</param>
        /// <returns>An 'unique' hashcode.</returns>
        /// <remarks>Based on System.Web.UI.HashCodeCombiner (use Reflector).</remarks>
        public static int CombineHash(params int[] hashCodes)
        {
            int hash = 5381; // 0x1505L

            foreach (int inp in hashCodes)
            {
                hash = ((hash << 5) + hash) ^ inp;
            }

            // Make sure the hash is not negative
            if (hash < 0)
            {
                hash = hash * -1;
            }

            return hash;
        }
    }
}