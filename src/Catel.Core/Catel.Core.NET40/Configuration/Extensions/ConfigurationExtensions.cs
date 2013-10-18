// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Configuration
{
    using System.Configuration;

    /// <summary>
    /// The configuration extension methods.
    /// </summary>
    public static class ConfigurationExtensions
    {
        #region Methods
        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <param name="this">The instance</param>
        /// <param name="sectionName">The section name</param>
        /// <param name="sectionGroupName">The section group name</param>
        /// <typeparam name="TSection">The type of the section</typeparam>
        /// <returns>The section</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="this"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="sectionName"/> is <c>null</c> or empty.</exception>
        public static TSection GetSection<TSection>(this Configuration @this, string sectionName, string sectionGroupName = null)
            where TSection : ConfigurationSection
        {
            Argument.IsNotNull("@this", @this);
            Argument.IsNotNullOrEmpty("sectionName", sectionName);

            TSection section = null;
            if (!string.IsNullOrEmpty(sectionGroupName))
            {
                ConfigurationSectionGroup configurationSectionGroup = @this.GetSectionGroup(sectionGroupName);
                if (configurationSectionGroup != null)
                {
                    section = (TSection) configurationSectionGroup.Sections[sectionName];
                }
            }
            else
            {
                section = (TSection) @this.Sections[sectionName];
            }

            return section;
        }
        #endregion
    }
}