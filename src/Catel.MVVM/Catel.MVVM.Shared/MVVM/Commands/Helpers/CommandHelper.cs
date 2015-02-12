// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using Caching;
    using Data;

    /// <summary>
    /// Helper class for the <see cref="Command"/> class.
    /// </summary>
    public static class CommandHelper
    {
        //private static readonly ICacheStorage<string, ICommand> _commandsCache = new CacheStorage<string, ICommand>();

        #region Methods
        ///// <summary>
        ///// Initializes and caches <see cref="ICommand"/> instances to be used from a <see cref="ICommand"/> property getter.
        ///// <para />
        ///// Enables the declaration of a command using a get-only property without initialization code or storage field.
        ///// <para />
        ///// The command is provided using a factory function.
        ///// <para />
        ///// The source file path and property name is used as key for the cache.
        ///// </summary>
        ///// <param name="commandFactory">Function which returns a new instance of the command to be used.</param>
        ///// <param name="callerFilePath">Used as cache key. Will be filled by the compiler.</param>
        ///// <param name="callerMemberName">Used as cache key. Will be filled by the compiler.</param>
        ///// <example>
        ///// <code>
        /////  <![CDATA[
        ///// public ICommand SampleCommand
        ///// {
        /////     get
        /////     {
        /////         return Commands.GetCommand(() =>
        /////             new Catel.MVVM.Command<string>(
        /////                 execute: (x) => DoSomethingWith(x),
        /////                 canExecute: (x) => !string.IsNullOrEmpty(x))
        /////             );
        /////     }
        ///// }
        ///// ]]>
        ///// </code>
        ///// </example>
        //public static ICommand GetCommand(Func<ICommand> commandFactory,
        //                                 [CallerFilePath] string callerFilePath = null,
        //                                 [CallerMemberName] string callerMemberName = null)
        //{
        //    Argument.IsNotNull(() => commandFactory);

        //    // build the key for the cache
        //    var key = callerFilePath + ";" + callerMemberName;

        //    return _commandsCache.GetFromCacheOrFetch(key, commandFactory);
        //}

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
        #endregion
    }
}