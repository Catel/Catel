// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapperTaskFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Tasks
{
    using System;

    using Catel.MVVM.Tasks;

    /// <summary>
    /// Factory that creates well-known tasks for the bootstrapper.
    /// <para />
    /// This implementation allows customization of both the descriptions and the actual logic.
    /// </summary>
    public class BootstrapperTaskFactory : IBootstrapperTaskFactory
    {
        /// <summary>
        /// Creates the create logger task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateCreateLoggerTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateCreateLoggerTask(action, "Creating logger");
        }

        /// <summary>
        /// Creates the create logger task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateCreateLoggerTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }

        /// <summary>
        /// Creates the create module catalog task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateCreateModuleCatalogTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateCreateModuleCatalogTask(action, "Creating module catalog");
        }

        /// <summary>
        /// Creates the create module catalog task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateCreateModuleCatalogTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }

        /// <summary>
        /// Creates the configure module catalog task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateConfigureModuleCatalogTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateConfigureModuleCatalogTask(action, "Configuring module catalog");
        }

        /// <summary>
        /// Creates the configure module catalog task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateConfigureModuleCatalogTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }

        /// <summary>
        /// Creates the create service locator container task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateCreateServiceLocatorContainerTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateCreateServiceLocatorContainerTask(action, "Creating service locator container");
        }

        /// <summary>
        /// Creates the create service locator container task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateCreateServiceLocatorContainerTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }

        /// <summary>
        /// Creates the configure service locator container task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateConfigureServiceLocatorContainerTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateConfigureServiceLocatorContainerTask(action, "Configuring service locator container");
        }

        /// <summary>
        /// Creates the configure service locator container task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateConfigureServiceLocatorContainerTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }

        /// <summary>
        /// Creates the configure service locator task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateConfigureServiceLocatorTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateConfigureServiceLocatorTask(action, "Configuring service locator");
        }

        /// <summary>
        /// Creates the configure service locator task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateConfigureServiceLocatorTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }

        /// <summary>
        /// Creates the configure region adapters task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateConfigureRegionAdaptersTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateConfigureRegionAdaptersTask(action, "Configuring region adapters");
        }

        /// <summary>
        /// Creates the configure region adapters task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateConfigureRegionAdaptersTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }

        /// <summary>
        /// Creates the configure default region behaviors task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateConfigureDefaultRegionBehaviorsTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateConfigureDefaultRegionBehaviorsTask(action, "Configuring default region behaviors");
        }

        /// <summary>
        /// Creates the configure default region behaviors task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateConfigureDefaultRegionBehaviorsTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }

        /// <summary>
        /// Creates the register framework exception types task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateRegisterFrameworkExceptionTypesTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateRegisterFrameworkExceptionTypesTask(action, "Registering framework exception types");
        }

        /// <summary>
        /// Creates the register framework exception types task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateRegisterFrameworkExceptionTypesTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }

        /// <summary>
        /// Creates the create shell task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateCreateShellTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateCreateShellTask(action, "Creating the shell");
        }

        /// <summary>
        /// Creates the create shell task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateCreateShellTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }

        /// <summary>
        /// Creates the initialize modules task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateInitializeModulesTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateInitializeModulesTask(action, "Initializing modules");
        }

        /// <summary>
        /// Creates the initialize modules task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateInitializeModulesTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }

        /// <summary>
        /// Creates the initializing shell task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateInitializingShellTask(Action action)
        {
            Argument.IsNotNull(() => action);

            return CreateInitializeModulesTask(action, "Initializing the shell");
        }

        /// <summary>
        /// Creates the initializing shell task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateInitializingShellTask(Action action, string description)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action());
        }
    }
}