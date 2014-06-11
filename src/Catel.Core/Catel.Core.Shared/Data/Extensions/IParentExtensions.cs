// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParentExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="IParent"/> interface.
    /// </summary>
    public static class IParentExtensions
    {
        /// <summary>
        /// Finds the parent of the specified type.
        /// </summary>
        /// <typeparam name="TParent">The ty</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="maxLevels">The maximum levels to search. If <c>-1</c>, the number is unlimited.</param>
        /// <returns>The parent or <c>null</c> if the parent is not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        public static TParent FindParent<TParent>(this IParent model, int maxLevels = -1) 
            where TParent : class
        {
            Argument.IsNotNull("model", model);

            if (maxLevels == 0)
            {
                return null;
            }

            var parent = model.Parent;
            if (parent == null)
            {
                return null;
            }

            var parentT = parent as TParent;
            if (parentT != null)
            {
                return parentT;
            }

            return FindParent<TParent>(parent, maxLevels - 1);
        }
    }
}