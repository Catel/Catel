// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingFlagsHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// The binding flags helper.
    /// </summary>
    public static class BindingFlagsHelper
    {
#if NET
		/// <summary>
		/// The default binding flags.
		/// </summary>
		public const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
#else
        /// <summary>
        /// The default binding flags.
        /// </summary>
        public const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public;
#endif

        /// <summary>
        /// Gets final binding flags and respects the <see cref="DefaultBindingFlags"/> as defined in Catel.
        /// </summary>
        /// <param name="flattenHierarchy">A value representing whether the hierarchy should be flattened. Corresponds with <see cref="BindingFlags.FlattenHierarchy"/>.</param>
        /// <param name="allowStaticMembers">A value representing whether static members should be included. Corresponds with <see cref="BindingFlags.Static"/>.</param>
        /// <param name="allowNonPublicMembers">
        /// A value representing whether non-public members should be included. Corresponds with <see cref="BindingFlags.NonPublic"/>.
        /// <para/>
        /// If this value is <c>null</c>, the default of the framework will be used. Also see <see cref="DefaultBindingFlags"/>.
        /// </param>
        /// <returns>The final binding flags.</returns>
        [DebuggerStepThrough]
        public static BindingFlags GetFinalBindingFlags(bool flattenHierarchy, bool allowStaticMembers, bool? allowNonPublicMembers = null)
        {
            var bindingFlags = DefaultBindingFlags;

            if (allowNonPublicMembers.HasValue)
            {
                if (allowNonPublicMembers.Value)
                {
                    bindingFlags = Enum<BindingFlags>.Flags.SetFlag(bindingFlags, BindingFlags.NonPublic);
                }
                else
                {
                    bindingFlags = Enum<BindingFlags>.Flags.ClearFlag(bindingFlags, BindingFlags.NonPublic);
                }
            }

            if (flattenHierarchy)
            {
                bindingFlags = Enum<BindingFlags>.Flags.SetFlag(bindingFlags, BindingFlags.FlattenHierarchy);
            }

            if (allowStaticMembers)
            {
                bindingFlags = Enum<BindingFlags>.Flags.SetFlag(bindingFlags, BindingFlags.Static);
            }

            return bindingFlags;
        }
    }
}