namespace Catel.MVVM
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Catel.Services;
    using Data;

    /// <summary>
    /// Helper class for the <see cref="Command"/> class.
    /// </summary>
    public static class CommandHelper
    {
        /// <summary>
        /// Creates a new <see cref="Command"/> that automatically determines whether it can be executed. It does this
        /// by checking the right validation summary, which should be in a property..
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="execute">The action to execute when the command is being invoked.</param>
        /// <param name="validationSummaryPropertyExpression">The validation summary property expression.</param>
        /// <param name="tag">The tag for the command.</param>
        /// <returns>The created command.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="execute"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationSummaryPropertyExpression"/> is <c>null</c>.</exception>
        public static Command CreateCommand(IServiceProvider serviceProvider, Action execute, Expression<Func<IValidationSummary>> validationSummaryPropertyExpression, object? tag = null)
        {    
            ArgumentNullException.ThrowIfNull(execute);
            ArgumentNullException.ThrowIfNull(validationSummaryPropertyExpression);

            var property = validationSummaryPropertyExpression.Compile();

            var command = new Command(serviceProvider, execute, () =>
            {
                var validationSummary = property.Invoke();
                return ((validationSummary is null) || !validationSummary.HasErrors);
            }, tag);

            return command;
        }

        /// <summary>
        /// Creates a new <see cref="Command{TExecuteParameter}"/> that automatically determines whether it can be executed. It does this
        /// by checking the right validation summary, which should be in a property..
        /// </summary>
        /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="execute">The action to execute when the command is being invoked.</param>
        /// <param name="validationSummaryPropertyExpression">The validation summary property expression.</param>
        /// <param name="tag">The tag for the command.</param>
        /// <returns>The created command.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="execute"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationSummaryPropertyExpression"/> is <c>null</c>.</exception>
        public static Command<TExecuteParameter> CreateCommand<TExecuteParameter>(IServiceProvider serviceProvider, 
            Action<TExecuteParameter?> execute, Expression<Func<IValidationSummary>> validationSummaryPropertyExpression, object? tag = null)
        {
            ArgumentNullException.ThrowIfNull(execute);
            ArgumentNullException.ThrowIfNull(validationSummaryPropertyExpression);

            var property = validationSummaryPropertyExpression.Compile();

            var command = new Command<TExecuteParameter>(serviceProvider, execute, parameter =>
            {
                var validationSummary = property.Invoke();
                return ((validationSummary is null) || !validationSummary.HasErrors);
            }, tag);

            return command;
        }

        /// <summary>
        /// Creates a new <see cref="TaskCommand"/> that automatically determines whether it can be executed. It does this
        /// by checking the right validation summary, which should be in a property..
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="execute">The action to execute when the command is being invoked.</param>
        /// <param name="validationSummaryPropertyExpression">The validation summary property expression.</param>
        /// <param name="tag">The tag for the command.</param>
        /// <returns>The created command.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="execute"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationSummaryPropertyExpression"/> is <c>null</c>.</exception>
        public static TaskCommand CreateTaskCommand(IServiceProvider serviceProvider, 
            Func<Task> execute, Expression<Func<IValidationSummary>> validationSummaryPropertyExpression, object? tag = null)
        {
            ArgumentNullException.ThrowIfNull(execute);
            ArgumentNullException.ThrowIfNull(validationSummaryPropertyExpression);

            var property = validationSummaryPropertyExpression.Compile();

            var command = new TaskCommand(serviceProvider, execute, () =>
            {
                var validationSummary = property.Invoke();
                return ((validationSummary is null) || !validationSummary.HasErrors);
            }, tag);

            return command;
        }

        /// <summary>
        /// Creates a new <see cref="TaskCommand{TExecuteParameter}"/> that automatically determines whether it can be executed. It does this
        /// by checking the right validation summary, which should be in a property..
        /// </summary>
        /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="execute">The action to execute when the command is being invoked.</param>
        /// <param name="validationSummaryPropertyExpression">The validation summary property expression.</param>
        /// <param name="tag">The tag for the command.</param>
        /// <returns>The created command.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="execute"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationSummaryPropertyExpression"/> is <c>null</c>.</exception>
        public static TaskCommand<TExecuteParameter> CreateTaskCommand<TExecuteParameter>(IServiceProvider serviceProvider, 
            Func<TExecuteParameter?, Task> execute, Expression<Func<IValidationSummary>> validationSummaryPropertyExpression, object? tag = null)
        {
            ArgumentNullException.ThrowIfNull(execute);
            ArgumentNullException.ThrowIfNull(validationSummaryPropertyExpression);

            var property = validationSummaryPropertyExpression.Compile();

            var command = new TaskCommand<TExecuteParameter>(serviceProvider, execute, parameter =>
            {
                var validationSummary = property.Invoke();
                return ((validationSummary is null) || !validationSummary.HasErrors);
            }, tag);

            return command;
        }
    }
}
