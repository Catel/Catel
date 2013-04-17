// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Linq.Expressions;
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
                IValidationSummary validationSummary = property.Invoke();
                return (validationSummary != null) && (!validationSummary.HasErrors);
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
                IValidationSummary validationSummary = property.Invoke();
                return (validationSummary != null) && (!validationSummary.HasErrors);
            }, tag);

            return command;
        }
        #endregion
    }
}