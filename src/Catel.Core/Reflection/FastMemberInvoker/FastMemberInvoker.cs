namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Wrapper class allowing fast member access as an alternative to reflection.
    /// </summary>
    public partial class FastMemberInvoker<TEntity>
        where TEntity : class
    {
        protected virtual Action<TEntity, TMemberType> Compile<TMemberType>(Expression<Action<TEntity, TMemberType>> expression)
        {
            return expression.Compile();
        }

        protected virtual Func<TEntity, TMemberType> Compile<TMemberType>(Expression<Func<TEntity, TMemberType>> expression)
        {
            return expression.Compile();
        }
    }
}
