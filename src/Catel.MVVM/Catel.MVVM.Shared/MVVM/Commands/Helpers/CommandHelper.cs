// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Caching;
    using Data;

    /// <summary>
    /// Helper class for the <see cref="Command"/> class.
    /// </summary>
    public static class CommandHelper
    {
        #region Methods
        /// <summary>
        /// Creates a new <see cref="Command"/> that automatically determines whether it can be executed. It does this
        /// by checking the right validation summary, which should be in a property..
        /// </summary>
        /// <param name="execute">The action to execute when the command is being invoked.</param>
        /// <param name="validationSummaryPropertyExpression">The validation summary property expression.</param>
        /// <param name="tag">The tag for the command.</param>
        /// <returns>The created command.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="execute"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationSummaryPropertyExpression"/> is <c>null</c>.</exception>
        public static Command CreateCommand(Action execute, Expression<Func<IValidationSummary>> validationSummaryPropertyExpression, object tag = null)
        {
            Argument.IsNotNull("execute", execute);
            Argument.IsNotNull("validationSummaryPropertyExpression", validationSummaryPropertyExpression);

            var property = validationSummaryPropertyExpression.Compile();

            var command = new Command(execute, () =>
            {
                var validationSummary = property.Invoke();
                return ((validationSummary == null) || !validationSummary.HasErrors);
            }, tag);

            return command;
        }

        /// <summary>
        /// Creates a new <see cref="Command{TExecuteParameter}"/> that automatically determines whether it can be executed. It does this
        /// by checking the right validation summary, which should be in a property..
        /// </summary>
        /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
        /// <param name="execute">The action to execute when the command is being invoked.</param>
        /// <param name="validationSummaryPropertyExpression">The validation summary property expression.</param>
        /// <param name="tag">The tag for the command.</param>
        /// <returns>The created command.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="execute"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationSummaryPropertyExpression"/> is <c>null</c>.</exception>
        public static Command<TExecuteParameter> CreateCommand<TExecuteParameter>(Action<TExecuteParameter> execute, Expression<Func<IValidationSummary>> validationSummaryPropertyExpression, object tag = null)
        {
            Argument.IsNotNull("execute", execute);
            Argument.IsNotNull("validationSummaryPropertyExpression", validationSummaryPropertyExpression);

            var property = validationSummaryPropertyExpression.Compile();

            var command = new Command<TExecuteParameter>(execute, parameter =>
            {
                var validationSummary = property.Invoke();
                return ((validationSummary == null) || !validationSummary.HasErrors);
            }, tag);

            return command;
        }

        /// <summary>
        /// Creates a new <see cref="TaskCommand"/> that automatically determines whether it can be executed. It does this
        /// by checking the right validation summary, which should be in a property..
        /// </summary>
        /// <param name="execute">The action to execute when the command is being invoked.</param>
        /// <param name="validationSummaryPropertyExpression">The validation summary property expression.</param>
        /// <param name="tag">The tag for the command.</param>
        /// <returns>The created command.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="execute"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationSummaryPropertyExpression"/> is <c>null</c>.</exception>
        public static TaskCommand CreateTaskCommand(Func<Task> execute, Expression<Func<IValidationSummary>> validationSummaryPropertyExpression, object tag = null)
        {
            Argument.IsNotNull("execute", execute);
            Argument.IsNotNull("validationSummaryPropertyExpression", validationSummaryPropertyExpression);

            var property = validationSummaryPropertyExpression.Compile();

            var command = new TaskCommand(execute, () =>
            {
                var validationSummary = property.Invoke();
                return ((validationSummary == null) || !validationSummary.HasErrors);
            }, tag);

            return command;
        }

        /// <summary>
        /// Creates a new <see cref="TaskCommand{TExecuteParameter}"/> that automatically determines whether it can be executed. It does this
        /// by checking the right validation summary, which should be in a property..
        /// </summary>
        /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
        /// <param name="execute">The action to execute when the command is being invoked.</param>
        /// <param name="validationSummaryPropertyExpression">The validation summary property expression.</param>
        /// <param name="tag">The tag for the command.</param>
        /// <returns>The created command.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="execute"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationSummaryPropertyExpression"/> is <c>null</c>.</exception>
        public static TaskCommand<TExecuteParameter> CreateTaskCommand<TExecuteParameter>(Func<TExecuteParameter, Task> execute, Expression<Func<IValidationSummary>> validationSummaryPropertyExpression, object tag = null)
        {
            Argument.IsNotNull("execute", execute);
            Argument.IsNotNull("validationSummaryPropertyExpression", validationSummaryPropertyExpression);

            var property = validationSummaryPropertyExpression.Compile();

            var command = new TaskCommand<TExecuteParameter>(execute, parameter =>
            {
                var validationSummary = property.Invoke();
                return ((validationSummary == null) || !validationSummary.HasErrors);
            }, tag);

            return command;
        }
        #endregion
    }
}