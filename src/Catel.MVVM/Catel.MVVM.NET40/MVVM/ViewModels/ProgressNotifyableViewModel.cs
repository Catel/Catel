// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressNotifyableViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.ViewModels
{
    using Data;
    using Tasks;

    /// <summary>
    /// The progress notifyable view model base.
    /// </summary>
    public class ProgressNotifyableViewModel : ViewModelBase, IProgressNotifyableViewModel
    {
        #region Constants

        /// <summary>Register the Task property so it is known in the class.</summary>
        public static readonly PropertyData TaskProperty = RegisterProperty("Task", typeof(ITask), default(ITask));

        /// <summary>Register the TaskMessage property so it is known in the class.</summary>
        public static readonly PropertyData TaskMessageProperty = RegisterProperty("TaskMessage", typeof(string), default(string), (s, e) => ((ProgressNotifyableViewModel)s).OnTaskMessageChanged());

        /// <summary>Register the TaskName property so it is known in the class.</summary>
        public static readonly PropertyData TaskNameProperty = RegisterProperty("TaskName", typeof(string));

        /// <summary>Register the TaskPercentage property so it is known in the class.</summary>
        public static readonly PropertyData TaskPercentageProperty = RegisterProperty("TaskPercentage", typeof(int), default(int), (s, e) => ((ProgressNotifyableViewModel)s).OnTaskPercentageChanged());

        /// <summary>Register the DetailedMessage property so it is known in the class.</summary>
        public static readonly PropertyData DetailedMessageProperty = RegisterProperty("DetailedMessage", typeof(string));
        #endregion

        #region Fields

        /// <summary>
        /// The _current item.
        /// </summary>
        private int _currentItem;

        /// <summary>
        /// The _total items.
        /// </summary>
        private int _totalItems;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the task message.
        /// </summary>
        [ViewModelToModel("Task", "Message")]
        public string TaskMessage
        {
            get { return GetValue<string>(TaskMessageProperty); }
            set { SetValue(TaskMessageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the task name.
        /// </summary>
        [ViewModelToModel("Task", "Name")]
        public string TaskName
        {
            get { return GetValue<string>(TaskNameProperty); }
            set { SetValue(TaskNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the task percentage.
        /// </summary>
        [ViewModelToModel("Task", "Percentage")]
        public int TaskPercentage
        {
            get { return GetValue<int>(TaskPercentageProperty); }
            set { SetValue(TaskPercentageProperty, value); }
        }
        #endregion

        #region IProgressNotifyableViewModel Members

        /// <summary>
        /// Gets the task.
        /// </summary>
        [Model]
        public ITask Task
        {
            get { return GetValue<ITask>(TaskProperty); }
            private set { SetValue(TaskProperty, value); }
        }

        /// <summary>
        /// Gets the detailed message.
        /// </summary>
        public string DetailedMessage
        {
            get { return GetValue<string>(DetailedMessageProperty); }
            private set { SetValue(DetailedMessageProperty, value); }
        }

        /// <summary>
        /// The update status.
        /// </summary>
        /// <param name="currentItem">
        /// The current item.
        /// </param>
        /// <param name="totalItems">
        /// The total items.
        /// </param>
        /// <param name="task">
        /// The task
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="task"/> is <c>null</c>. 
        /// </exception>
        public void UpdateStatus(int currentItem, int totalItems, ITask task)
        {
            Argument.IsNotNull("task", task);

            Task = task;

            _currentItem = currentItem;
            _totalItems = totalItems;

            RaisePropertyChanged(() => Percentage);
        }

        /// <summary>
        /// Gets the percentage.
        /// </summary>
        public int Percentage
        {
            get
            {
                if (_totalItems <= 0)
                {
                    return 0;
                }

                var nextPercentage = (int)((100.0f * (_currentItem + 1)) / _totalItems);
                var currentPercentage = (int)((100.0f * _currentItem) / _totalItems);

                float deltaPercentage = nextPercentage - currentPercentage;
                float scaledTaskPercentage = Task.Percentage * deltaPercentage / 100.0f;

                return (int)(currentPercentage + scaledTaskPercentage);
            }
        }
        #endregion

        #region Methods

        /// <summary>Occurs when the value of the TaskMessage property is changed.</summary>
        private void OnTaskMessageChanged()
        {
            DetailedMessage = string.Format("{0}: {1}", Task.Name, Task.Message);
        }

        /// <summary>Occurs when the value of the TaskPercentage property is changed.</summary>
        private void OnTaskPercentageChanged()
        {
            RaisePropertyChanged(() => Percentage);
        }
        #endregion
    }
}