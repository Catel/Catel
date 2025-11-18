namespace Catel.Reflection
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Wrapper class allowing fast member access as an alternative to reflection.
    /// </summary>
    public partial class FastMemberInvoker<TEntity> : IFastMemberInvoker
    {
        protected virtual Action<TEntity, TMemberType> Compile<TMemberType>(Expression<Action<TEntity, TMemberType>> expression)
        {
            return expression.Compile();
        }

        protected virtual Func<TEntity, TMemberType> Compile<TMemberType>(Expression<Func<TEntity, TMemberType>> expression)
        {
            return expression.Compile();
        }

        public bool TrySetPropertyValue<TValue>(object entity, string propertyName, TValue value)
        {
            if (typeof(TValue).IsValueType)
            {
                return TrySetPropertyValue((TEntity)entity, propertyName, value);
            }
            else
            {
                return TrySetPropertyValueObject((TEntity)entity, propertyName, value);
            }
        }

        public bool TryGetPropertyValue<TValue>(object entity, string propertyName, out TValue value)
        {
            if (typeof(TValue).IsValueType)
            {
                return TryGetPropertyValue((TEntity)entity, propertyName, out value);
            }
            else
            {
                return TryGetPropertyValueObject((TEntity)entity, propertyName, out value);
            }
        }

        public bool TrySetFieldValue<TValue>(object entity, string fieldName, TValue value)
        {
            if (typeof(TValue).IsValueType)
            {
                return TrySetFieldValue((TEntity)entity, fieldName, value);
            }
            else
            {
                return TrySetFieldValueObject((TEntity)entity, fieldName, value);
            }
        }

        public bool TryGetFieldValue<TValue>(object entity, string fieldName, out TValue value)
        {
            if (typeof(TValue).IsValueType)
            {
                return TryGetFieldValue((TEntity)entity, fieldName, out value);
            }
            else
            {
                return TryGetFieldValueObject((TEntity)entity, fieldName, out value);
            }
        }

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
        private bool TrySetPropertyValueObject<TValue>(TEntity entity, string propertyName, TValue value)
        {
            var setter = MembersCache<object>.GetPropertySetter(propertyName, this);
            if (setter is not null)
            {
                // It will not be null for value types and it is allowed to be null if return false
                // so we have to live with that 
#pragma warning disable CS8604
                setter(entity, value);
#pragma warning restore CS8604
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
        private bool TryGetPropertyValueObject<TValue>(TEntity entity, string propertyName, out TValue value)
        {
            var getter = MembersCache<object>.GetPropertyGetter(propertyName, this);
            if (getter is not null)
            {
                value = (TValue)getter(entity);
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
        private bool TrySetFieldValueObject<TValue>(TEntity entity, string fieldName, TValue value)
        {
            var setter = MembersCache<object>.GetFieldSetter(fieldName, this);
            if (setter is not null)
            {
                // It will not be null for value types and it is allowed to be null if return false
                // so we have to live with that 
#pragma warning disable CS8604
                setter(entity, value);
#pragma warning restore CS8604
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetFieldValueObject<TValue>(TEntity entity, string fieldName, out TValue value)
        {
            var getter = MembersCache<object>.GetFieldGetter(fieldName, this);
            if (getter is not null)
            {
                value = (TValue)getter(entity);
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
