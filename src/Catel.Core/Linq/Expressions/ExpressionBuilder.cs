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
        // Note: cast or convert expression originally comes from https://tomsundev.wordpress.com/category/articles/set-or-get-fields-with-high-performance/
        private static Expression GetCastOrConvertExpression(Expression expression, Type targetType)
        {
            Expression result;
            var expressionType = expression.Type;

            // Check if a cast or conversion is required.
            if (!targetType.IsAssignableFrom(expressionType))
            {
                // Check if we can use the as operator for casting or if we must use the convert method
                if (targetType.IsValueTypeEx() && !targetType.IsNullableType())
                {
                    result = Expression.Convert(expression, targetType);
                }
                else
                {
                    result = Expression.TypeAs(expression, targetType);
                }
            }
            else
            {
                // Always hard cast, otherwise we might get exceptions while creating
                // the expressions
                result = Expression.Convert(expression, targetType);
            }

            return result;
        }
    }
}
