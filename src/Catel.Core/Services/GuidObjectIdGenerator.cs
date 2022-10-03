namespace Catel.Services
{
    using System;

    /// <summary>
    /// Guid object generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The entity type.
    /// </typeparam>
    public class GuidObjectIdGenerator<TObjectType> : ObjectIdGenerator<TObjectType, Guid>
        where TObjectType : class
    {
        /// <inheritdoc />
        protected override Guid GenerateUniqueIdentifier()
        {
            // Note: no need to lock, guids are unique
            return Guid.NewGuid();
        }
    }
}
