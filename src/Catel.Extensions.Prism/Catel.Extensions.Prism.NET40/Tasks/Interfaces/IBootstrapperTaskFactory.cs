// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBootstrapperTaskFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Tasks
{
    using System;

    using Catel.MVVM.Tasks;

    /// <summary>
    /// Factory that creates well-known tasks for the bootstrapper.
    /// </summary>
    public interface IBootstrapperTaskFactory
    {
        /// <summary>
        /// Creates the create logger task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateCreateLoggerTask(Action action);

        /// <summary>
        /// Creates the create module catalog task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateCreateModuleCatalogTask(Action action);

        /// <summary>
        /// Creates the configure module catalog task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateConfigureModuleCatalogTask(Action action);

        /// <summary>
        /// Creates the create service locator container task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateCreateServiceLocatorContainerTask(Action action);

        /// <summary>
        /// Creates the configure service locator container task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateConfigureServiceLocatorContainerTask(Action action);

        /// <summary>
        /// Creates the configure service locator task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateConfigureServiceLocatorTask(Action action);

        /// <summary>
        /// Creates the configure region adapters task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateConfigureRegionAdaptersTask(Action action);

        /// <summary>
        /// Creates the configure default region behaviors task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateConfigureDefaultRegionBehaviorsTask(Action action);

        /// <summary>
        /// Creates the register framework exception types task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateRegisterFrameworkExceptionTypesTask(Action action);

        /// <summary>
        /// Creates the create shell task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateCreateShellTask(Action action);

        /// <summary>
        /// Creates the initialize modules task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateInitializeModulesTask(Action action);

        /// <summary>
        /// Creates the initializing shell task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ITask CreateInitializingShellTask(Action action);
    }
}