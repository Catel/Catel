// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISplashScreenService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using Tasks;

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
        /// The enqueue.
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="task"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        void Enqueue(ITask task);

        /// <summary>
        /// The commit.
        /// </summary>
        /// <param name="completedCallback">
        /// The completed callback.
        /// </param>
        /// <param name="viewModelType">
        /// The view mode type.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If the batch is already commited and the execution is in progress or committing via async way.
        /// </exception>
        void CommitAsync(Action completedCallback = null, Type viewModelType = null);

        /// <summary>
        /// The commit.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <param name="completedCallback">
        /// The completed callback.
        /// </param>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        void CommitAsync<TViewModel>(Action completedCallback = null) where TViewModel : IProgressNotifyableViewModel;

        /// <summary>
        /// Execute in batch mode the enqueued tasks
        /// </summary>
        /// <param name="viewModelType">
        /// The view model type.
        /// </param>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        void Commit(Type viewModelType = null);

        /// <summary>
        /// Execute in batch mode the enqueued tasks
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
       void Commit<TViewModel>() where TViewModel : IProgressNotifyableViewModel;
    }
}