namespace Catel.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.Reflection;

    public static partial class ExpressionBuilder
    {
        public static Expression<Func<object, TProperty>> CreatePropertyGetter<TProperty>(Type modelType, string propertyName)
        {
            Argument.IsNotNullOrWhitespace(() => propertyName);

            var property = modelType.GetPropertyEx(propertyName);
            return property?.GetMethod is null ? null : CreatePropertyGetter<object, TProperty>(property);
        }

        public static Expression<Func<T, TProperty>> CreatePropertyGetter<T, TProperty>(string propertyName)
        {
            Argument.IsNotNullOrWhitespace(() => propertyName);

            var property = typeof(T).GetPropertyEx(propertyName);
            return property?.GetMethod is null ? null : CreatePropertyGetter<T, TProperty>(property);
        }

        public static Expression<Func<T, TProperty>> CreatePropertyGetter<T, TProperty>(PropertyInfo propertyInfo)
        {
            Argument.IsNotNull(() => propertyInfo);

            return propertyInfo.GetMethod is null ? null : CreatePropertyGetterExpression<T, TProperty>(propertyInfo);
        }

        public static Expression<Func<T, object>> CreatePropertyGetter<T>(string propertyName)
        {
            Argument.IsNotNullOrWhitespace(() => propertyName);

            var property = typeof(T).GetPropertyEx(propertyName);
            return property?.GetMethod is null ? null : CreatePropertyGetter<T>(property);
        }

        public static Expression<Func<T, object>> CreatePropertyGetter<T>(PropertyInfo propertyInfo)
        {
            Argument.IsNotNull(() => propertyInfo);

            return propertyInfo.GetMethod is null ? null : CreatePropertyGetterExpression<T, object>(propertyInfo);
        }

        public static IReadOnlyDictionary<string, Expression<Func<T, TProperty>>> CreatePropertyGetters<T, TProperty>()
        {
            var propertyGetters = new Dictionary<string, Expression<Func<T, TProperty>>>(StringComparer.OrdinalIgnoreCase);
            var properties = typeof(T).GetPropertiesEx().Where(w => w.GetMethod != null && w.PropertyType == typeof(TProperty));

            foreach (var property in properties)
            {
                var propertyGetter = CreatePropertyGetter<T, TProperty>(property);
                if (propertyGetter is null)
                {
                    continue;
                }

                propertyGetters[property.Name] = propertyGetter;
            }

            return new ReadOnlyDictionary<string, Expression<Func<T, TProperty>>>(propertyGetters);
        }

        public static IReadOnlyDictionary<string, Expression<Func<T, object>>> CreatePropertyGetters<T>()
        {
            var propertyGetters = new Dictionary<string, Expression<Func<T, object>>>(StringComparer.OrdinalIgnoreCase);
            var properties = typeof(T).GetPropertiesEx().Where(w => w.GetMethod != null);

            foreach (var property in properties)
            {
                var propertyGetter = CreatePropertyGetter<T>(property);
                if (propertyGetter is null)
                {
                    continue;
                }

                propertyGetters[property.Name] = propertyGetter;
            }

            return new ReadOnlyDictionary<string, Expression<Func<T, object>>>(propertyGetters);
        }

        private static Expression<Func<T, TProperty>> CreatePropertyGetterExpression<T, TProperty>(PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.GetMethod;

            if (targetType is null || methodInfo is null)
            {
                return null;
            }

            var target = Expression.Parameter(typeof(T), "target");
            var targetExpression = GetCastOrConvertExpression(target, targetType);

            var body = Expression.Call(targetExpression, methodInfo);

            var finalExpression = GetCastOrConvertExpression(body, typeof(TProperty));
            var lambda = Expression.Lambda<Func<T, TProperty>>(finalExpression, target);
            return lambda;
        }
    }
}
