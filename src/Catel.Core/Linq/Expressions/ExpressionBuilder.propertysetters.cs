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
        public static Expression<Action<T, TProperty>> CreatePropertySetter<T, TProperty>(string propertyName)
        {
            Argument.IsNotNullOrWhitespace(() => propertyName);

            var property = typeof(T).GetPropertyEx(propertyName);
            return property?.SetMethod is null ? null : CreatePropertySetter<T, TProperty>(property);
        }

        public static Expression<Action<T, TProperty>> CreatePropertySetter<T, TProperty>(PropertyInfo propertyInfo)
        {
            Argument.IsNotNull(() => propertyInfo);

            return propertyInfo.SetMethod is null ? null : CreatePropertySetterExpression<T, TProperty>(propertyInfo);
        }

        public static Expression<Action<T, object>> CreatePropertySetter<T>(string propertyName)
        {
            Argument.IsNotNullOrWhitespace(() => propertyName);

            var property = typeof(T).GetPropertyEx(propertyName);
            return property?.SetMethod is null ? null : CreatePropertySetter<T>(property);
        }

        public static Expression<Action<T, object>> CreatePropertySetter<T>(PropertyInfo propertyInfo)
        {
            Argument.IsNotNull(() => propertyInfo);

            return propertyInfo.SetMethod is null ? null : CreatePropertySetterExpression<T, object>(propertyInfo);
        }

        public static IReadOnlyDictionary<string, Expression<Action<T, object>>> CreatePropertySetters<T>()
        {
            var propertySetters = new Dictionary<string, Expression<Action<T, object>>>(StringComparer.OrdinalIgnoreCase);
            var properties = typeof(T).GetPropertiesEx().Where(w => w.SetMethod != null);

            foreach (var property in properties)
            {
                var propertySetter = CreatePropertySetter<T>(property);
                if (propertySetter is null)
                {
                    continue;
                }

                propertySetters[property.Name] = propertySetter;
            }

            return new ReadOnlyDictionary<string, Expression<Action<T, object>>>(propertySetters);
        }

        public static IReadOnlyDictionary<string, Expression<Action<T, TProperty>>> CreatePropertySetters<T, TProperty>()
        {
            var propertySetters = new Dictionary<string, Expression<Action<T, TProperty>>>(StringComparer.OrdinalIgnoreCase);
            var properties = typeof(T).GetPropertiesEx().Where(w => w.SetMethod != null && w.PropertyType == typeof(TProperty));

            foreach (var property in properties)
            {
                var propertySetter = CreatePropertySetter<T, TProperty>(property);
                if (propertySetter is null)
                {
                    continue;
                }

                propertySetters[property.Name] = propertySetter;
            }

            return new ReadOnlyDictionary<string, Expression<Action<T, TProperty>>>(propertySetters);
        }

        private static Expression<Action<T, TProperty>> CreatePropertySetterExpression<T, TProperty>(PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.SetMethod;

            if (targetType is null || methodInfo is null)
            {
                return null;
            }

            var target = Expression.Parameter(targetType, "target");
            var targetExpression = GetCastOrConvertExpression(target, typeof(T));

            var valueParameter = Expression.Parameter(typeof(TProperty), "property");
            var valueParameterExpression = GetCastOrConvertExpression(valueParameter, propertyInfo.PropertyType);

            var body = Expression.Call(targetExpression, methodInfo, valueParameterExpression);

            var lambda = Expression.Lambda<Action<T, TProperty>>(body, target, valueParameter);
            return lambda;
        }
    }
}
