// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISplashScreenService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using Catel.MVVM;
    using Catel.MVVM.Tasks;

    /// <summary>
    /// The splash screen service interface.
    /// </summary>
    public interface ISplashScreenService
    {
        /// <summary>
        /// Gets a value indicating whether is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets and sets a value indicating whether the service will close the view model when done.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        bool CloseViewModelOnTerminated { get; set; }

        /// <summary>
        /// The enqueue.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="task" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        void Enqueue(ITask task);

        /// <summary>
        /// The commit.
        /// </summary>
        /// <param name="completedCallback">The completed callback.</param>
        /// <param name="viewModelType">The view mode type.</param>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        void CommitAsync(Action completedCallback = null, Type viewModelType = null);

        /// <summary>
        /// The commit.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="completedCallback">The completed callback.</param>
        /// <param name="viewModel">The viewmodel instance.</param>
        /// <param name="show">
        /// Indicates whether the view model will be shown. If the view model is <c>null</c> then this argument will be ignored. 
        /// </param>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        void CommitAsync<TViewModel>(Action completedCallback = null, TViewModel viewModel = default(TViewModel), bool show = true) where TViewModel : IProgressNotifyableViewModel;

        /// <summary>
        /// Execute in batch mode the enqueued tasks.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        void Commit(Type viewModelType = null);

        /// <summary>
        /// Execute in batch mode the enqueued tasks.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="viewModel">The viewmodel instance.</param>
        /// <param name="show">
        /// Indicates whether the view model will be shown. If the view model is <c>null</c> then this argument will be ignored. 
        /// </param>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        void Commit<TViewModel>(TViewModel viewModel = default(TViewModel), bool show = true) where TViewModel : IProgressNotifyableViewModel;
    }
}