// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapperTaskFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateCreateLoggerTask(Action action, bool dispatch = false)
        {
            Argument.IsNotNull(() => action);

            return CreateCreateLoggerTask(action, "Creating logger", dispatch);
        }

        /// <summary>
        /// Creates the create logger task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateCreateLoggerTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) { AutomaticallyDispatch = dispatch };
        }

        /// <summary>
        /// Creates the create module catalog task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateCreateModuleCatalogTask(Action action, bool dispatch = false)
        {
            Argument.IsNotNull(() => action);

            return CreateCreateModuleCatalogTask(action, "Creating module catalog", dispatch);
        }

        /// <summary>
        /// Creates the create module catalog task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateCreateModuleCatalogTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) { AutomaticallyDispatch = dispatch };
        }

        /// <summary>
        /// Creates the configure module catalog task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateConfigureModuleCatalogTask(Action action, bool dispatch = false)
        {
            Argument.IsNotNull(() => action);

            return CreateConfigureModuleCatalogTask(action, "Configuring module catalog", dispatch);
        }

        /// <summary>
        /// Creates the configure module catalog task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateConfigureModuleCatalogTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) { AutomaticallyDispatch = dispatch };
        }

        /// <summary>
        /// Creates the create service locator container task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateCreateServiceLocatorContainerTask(Action action, bool dispatch = false)
        {
            Argument.IsNotNull(() => action);

            return CreateCreateServiceLocatorContainerTask(action, "Creating service locator container", dispatch);
        }

        /// <summary>
        /// Creates the create service locator container task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateCreateServiceLocatorContainerTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) { AutomaticallyDispatch = dispatch };
        }

        /// <summary>
        /// Creates the configure service locator container task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateConfigureServiceLocatorContainerTask(Action action, bool dispatch = false)
        {
            Argument.IsNotNull(() => action);

            return CreateConfigureServiceLocatorContainerTask(action, "Configuring service locator container", dispatch);
        }

        /// <summary>
        /// Creates the configure service locator container task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateConfigureServiceLocatorContainerTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) { AutomaticallyDispatch = dispatch };
        }

        /// <summary>
        /// Creates the configure service locator task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateConfigureServiceLocatorTask(Action action, bool dispatch = false)
        {
            Argument.IsNotNull(() => action);

            return CreateConfigureServiceLocatorTask(action, "Configuring service locator", dispatch);
        }

        /// <summary>
        /// Creates the configure service locator task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateConfigureServiceLocatorTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) { AutomaticallyDispatch = dispatch };
        }

        /// <summary>
        /// Creates the configure region adapters task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateConfigureRegionAdaptersTask(Action action, bool dispatch = false)
        {
            Argument.IsNotNull(() => action);

            return CreateConfigureRegionAdaptersTask(action, "Configuring region adapters", dispatch);
        }

        /// <summary>
        /// Creates the configure region adapters task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateConfigureRegionAdaptersTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) { AutomaticallyDispatch = dispatch };
        }

        /// <summary>
        /// Creates the configure default region behaviors task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateConfigureDefaultRegionBehaviorsTask(Action action, bool dispatch = false)
        {
            Argument.IsNotNull(() => action);

            return CreateConfigureDefaultRegionBehaviorsTask(action, "Configuring default region behaviors", dispatch);
        }

        /// <summary>
        /// Creates the configure default region behaviors task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateConfigureDefaultRegionBehaviorsTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) { AutomaticallyDispatch = dispatch };
        }

        /// <summary>
        /// Creates the register framework exception types task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateRegisterFrameworkExceptionTypesTask(Action action, bool dispatch = false)
        {
            Argument.IsNotNull(() => action);

            return CreateRegisterFrameworkExceptionTypesTask(action, "Registering framework exception types", dispatch);
        }

        /// <summary>
        /// Creates the register framework exception types task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateRegisterFrameworkExceptionTypesTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) { AutomaticallyDispatch = dispatch };
        }

        /// <summary>
        /// Creates the create shell task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateCreateShellTask(Action action, bool dispatch = true)
        {
            Argument.IsNotNull(() => action);

            return CreateCreateShellTask(action, "Creating the shell", dispatch);
        }

        /// <summary>
        /// Creates the create shell task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateCreateShellTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) { AutomaticallyDispatch = dispatch };
        }

        /// <summary>
        /// Creates the initialize modules task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateInitializeModulesTask(Action action, bool dispatch = true)
        {
            Argument.IsNotNull(() => action);

            return CreateInitializeModulesTask(action, "Initializing modules", dispatch);
        }

        /// <summary>
        /// Creates the initialize modules task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateInitializeModulesTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) { AutomaticallyDispatch = dispatch };
        }

        /// <summary>
        /// Creates the initializing shell task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public virtual ITask CreateInitializingShellTask(Action action, bool dispatch = true)
        {
            Argument.IsNotNull(() => action);

            return CreateInitializingShellTask(action, "Initializing the shell", dispatch);
        }

        /// <summary>
        /// Creates the initializing shell task.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">if set to <c>true</c>, this action is dispatched to the UI thread.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="description"/> is <c>null</c>.</exception>
        protected virtual ITask CreateInitializingShellTask(Action action, string description, bool dispatch)
        {
            Argument.IsNotNull(() => action);
            Argument.IsNotNull(() => description);

            return new ActionTask(description, x => action()) {AutomaticallyDispatch = dispatch};
        }
    }
}