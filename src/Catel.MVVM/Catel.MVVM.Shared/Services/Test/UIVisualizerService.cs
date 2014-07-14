// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIVisualizerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;
    using Catel.MVVM;

    /// <summary>
    /// Test implementation of the <see cref="IUIVisualizerService"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// 
    /// Test.UIVisualizerService service = (Test.UIVisualizerService)GetService<IUIVisualizerService>();
    /// 
    /// // Queue the next expected result
    /// service.ExpectedShowResults.Add(() =>
    ///              {
    ///                // If required, handle custom data manipulation here
    ///                return true;
    ///              });
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public class UIVisualizerService : IUIVisualizerService
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UIVisualizerService"/> class.
        /// </summary>
        public UIVisualizerService()
        {
            ExpectedShowResults = new Queue<Func<bool>>();
            ExpectedShowDialogResults = new Queue<Func<bool>>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the queue of expected results for the <see cref="Show(string,object,System.EventHandler{Catel.Services.UICompletedEventArgs})"/>
        /// method.
        /// </summary>
        /// <value>The expected results.</value>
        public Queue<Func<bool>> ExpectedShowResults { get; private set; }

        /// <summary>
        /// Gets the queue of expected results for the <see cref="ShowDialog(string,object,System.EventHandler{Catel.Services.UICompletedEventArgs})"/>
        /// method.
        /// </summary>
        /// <value>The expected results.</value>
        public Queue<Func<bool>> ExpectedShowDialogResults { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="windowType">Type of the window.</param>
        /// <param name="throwExceptionIfExists">if set to <c>true</c> [throw exception if exists].</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Register(string name, Type windowType, bool throwExceptionIfExists = true)
        {
            // No implementation by design
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="name">Name of the registered window.</param>
        /// <returns>
        /// 	<c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        public bool Unregister(string name)
        {
            // No implementation by design
            return true;
        }

        /// <summary>
        /// Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// 	<c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="WindowNotRegisteredException">The <paramref name="viewModel"/> is not registered by the <see cref="Register(string,System.Type,bool)"/> method first.</exception>
        public Task<bool?> Show(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            return Show(viewModel.GetType().FullName, viewModel, completedProc);
        }

        /// <summary>
        /// Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data to set as data context. If <c>null</c>, the data context will be untouched.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// 	<c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="WindowNotRegisteredException">The <paramref name="name"/> is not registered by the <see cref="Register(string,System.Type,bool)"/> method first.</exception>
        public Task<bool?> Show(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            if (ExpectedShowResults.Count == 0)
            {
                throw new Exception(ResourceHelper.GetString("NoExpectedResultsInQueueForUnitTest"));
            }

            return Task.Factory.StartNew<bool?>(() => ExpectedShowResults.Dequeue().Invoke());
        }

        /// <summary>
        /// Shows a window that is registered with the specified view model in a modal state.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// Nullable boolean representing the dialog result.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="WindowNotRegisteredException">The <paramref name="viewModel"/> is not registered by the <see cref="Register(string,System.Type,bool)"/> method first.</exception>
        public Task<bool?> ShowDialog(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            return ShowDialog(viewModel.GetType().FullName, viewModel, completedProc);
        }

        /// <summary>
        /// Shows a window that is registered with the specified view model in a modal state.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data to set as data context. If <c>null</c>, the data context will be untouched.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// Nullable boolean representing the dialog result.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="WindowNotRegisteredException">The <paramref name="name"/> is not registered by the <see cref="Register(string,System.Type,bool)"/> method first.</exception>
        public Task<bool?> ShowDialog(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            if (ExpectedShowDialogResults.Count == 0)
            {
                throw new Exception(ResourceHelper.GetString("NoExpectedResultsInQueueForUnitTest"));
            }

            return Task.Factory.StartNew<bool?>(() => ExpectedShowDialogResults.Dequeue().Invoke());
        }

        /// <summary>
        /// Determines whether the specified name is registered.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified name is registered; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool IsRegistered(string name)
        {
            return true;
        }
        #endregion
    }
}
