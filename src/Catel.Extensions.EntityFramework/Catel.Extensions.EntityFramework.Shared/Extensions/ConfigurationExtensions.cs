// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System.Data.Entity.ModelConfiguration;

    /// <summary>
    /// Class EntityTypeConfigurationExtensions.
    /// </summary>
    public static class EntityTypeConfigurationExtensions
    {
        #region Methods
        /// <summary>
        /// Ignores the catel properties for database mappings.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>EntityTypeConfiguration&lt;TEntity&gt;.</returns>
        public static EntityTypeConfiguration<TEntity> IgnoreCatelProperties<TEntity>(this EntityTypeConfiguration<TEntity> configuration)
            where TEntity : ModelBase
        {
            //configuration.Ignore(x => x.BusinessRuleErrorCount);
            //configuration.Ignore(x => x.BusinessRuleWarningCount);
            //configuration.Ignore(x => x.FieldErrorCount);
            //configuration.Ignore(x => x.FieldWarningCount);
            //configuration.Ignore(x => x.HasErrors);
            //configuration.Ignore(x => x.HasWarnings);
            configuration.Ignore(x => x.IsDirty);
            //configuration.Ignore(x => x.IsInEditSession);
            configuration.Ignore(x => x.IsReadOnly);
            //configuration.Ignore(x => x.Mode);
            //configuration.Ignore(x => x.ValidationContext);
            //configuration.Ignore(x => x.Validator);

            return configuration;
        }
        #endregion
    }
}