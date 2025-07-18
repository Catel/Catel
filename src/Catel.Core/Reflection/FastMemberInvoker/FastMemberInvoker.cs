namespace Catel.Reflection
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Wrapper class allowing fast member access as an alternative to reflection.
    /// </summary>
    public partial class FastMemberInvoker<TEntity> : IFastMemberInvoker
    //where TEntity : class // Removed, see https://github.com/Catel/Catel/issues/1610
    {
        protected virtual Action<TEntity, TMemberType> Compile<TMemberType>(Expression<Action<TEntity, TMemberType>> expression)
        {
            return expression.Compile();
        }

        protected virtual Func<TEntity, TMemberType> Compile<TMemberType>(Expression<Func<TEntity, TMemberType>> expression)
        {
            return expression.Compile();
        }

        public bool TrySetPropertyValue<TValue>(object entity, string propertyName, TValue value) =>
            TrySetPropertyValue((TEntity)entity, propertyName, value);

        public bool TryGetPropertyValue<TValue>(object entity, string propertyName, out TValue value) =>
            TryGetPropertyValue((TEntity)entity, propertyName, out value);

        public bool TrySetFieldValue<TValue>(object entity, string fieldName, TValue value) =>
            TrySetFieldValue((TEntity)entity, fieldName, value);

        public bool TryGetFieldValue<TValue>(object entity, string fieldName, out TValue value) =>
            TryGetFieldValue((TEntity)entity, fieldName, out value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TrySetPropertyValue<TValue>(TEntity entity, string propertyName, TValue value)
        {
            var setter = MembersCache<TValue>.GetPropertySetter(propertyName, this);
            if (setter is not null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetPropertyValue<TValue>(TEntity entity, string propertyName, out TValue value)
        {
            var getter = MembersCache<TValue>.GetPropertyGetter(propertyName, this);
            if (getter is not null)
            {
                value = getter(entity);
                return true;
            }
            // It will not be null for value types and it is allowed to be null if return false
            // so we have to live with that 
#pragma warning disable CS8601 
            value = default;
#pragma warning restore CS8601
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TrySetFieldValue<TValue>(TEntity entity, string fieldName, TValue value)
        {
            var setter = MembersCache<TValue>.GetFieldSetter(fieldName, this);
            if (setter is not null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetFieldValue<TValue>(TEntity entity, string fieldName, out TValue value)
        {
            var getter = MembersCache<TValue>.GetFieldGetter(fieldName, this);
            if (getter is not null)
            {
                value = getter(entity);
                return true;
            }
            // It will not be null for value types and it is allowed to be null if return false
            // so we have to live with that 
#pragma warning disable CS8601
            value = default;
#pragma warning restore CS8601
            return false;
        }
    }
}
