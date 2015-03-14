// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressNotifyableViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.MVVM
{
    using System.ComponentModel;

    using Data;
    using IoC;
    using MVVM.Tasks;

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
       
        /// <summary>Register the TaskPercentage property so it is known in the class.</summary>
        public static readonly PropertyData TaskIsIndeterminateProperty = RegisterProperty("TaskIsIndeterminate", typeof(bool), true);

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

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressNotifyableViewModel"/> class.
        /// </summary>
        /// <remarks>Must have a public constructor in order to be serializable.</remarks>
        public ProgressNotifyableViewModel()
            : this(false, false, false)
        {
            DispatchPropertyChangedEvent = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressNotifyableViewModel"/> class.
        /// </summary>
        /// <param name="supportIEditableObject">if set to <c>true</c>, the view model will natively support models that
        /// implement the <see cref="IEditableObject"/> interface.</param>
        /// <param name="ignoreMultipleModelsWarning">if set to <c>true</c>, the warning when using multiple models is ignored.</param>
        /// <param name="skipViewModelAttributesInitialization">
        /// if set to <c>true</c>, the initialization will be skipped and must be done manually via <see cref="ViewModelBase.InitializeViewModelAttributes"/>.
        /// </param>
        /// <exception cref="ModelNotRegisteredException">A mapped model is not registered.</exception>
        /// <exception cref="PropertyNotFoundInModelException">A mapped model property is not found.</exception>
        public ProgressNotifyableViewModel(bool supportIEditableObject, bool ignoreMultipleModelsWarning, bool skipViewModelAttributesInitialization)
            : base(supportIEditableObject, ignoreMultipleModelsWarning, skipViewModelAttributesInitialization)
        {
            DispatchPropertyChangedEvent = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// <para/>
        /// This constructor allows the injection of a custom <see cref="IServiceLocator"/>.
        /// </summary>
        /// <param name="serviceLocator">The service locator to inject. If <c>null</c>, the <see cref="Catel.IoC.ServiceLocator.Default"/> will be used.</param>
        /// <param name="supportIEditableObject">if set to <c>true</c>, the view model will natively support models that
        /// implement the <see cref="IEditableObject"/> interface.</param>
        /// <param name="ignoreMultipleModelsWarning">if set to <c>true</c>, the warning when using multiple models is ignored.</param>
        /// <param name="skipViewModelAttributesInitialization">if set to <c>true</c>, the initialization will be skipped and must be done manually via <see cref="ViewModelBase.InitializeViewModelAttributes"/>.</param>
        /// <exception cref="ModelNotRegisteredException">A mapped model is not registered.</exception>
        /// <exception cref="PropertyNotFoundInModelException">A mapped model property is not found.</exception>
        public ProgressNotifyableViewModel(IServiceLocator serviceLocator, bool supportIEditableObject = true, bool ignoreMultipleModelsWarning = false, bool skipViewModelAttributesInitialization = false)
            : base(serviceLocator, supportIEditableObject, ignoreMultipleModelsWarning, skipViewModelAttributesInitialization)
        {
            DispatchPropertyChangedEvent = true;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the task message.
        /// </summary>
        [ViewModelToModel("Task", "Message")]
        public string TaskMessage
        {
            get { return GetValue<string>(TaskMessageProperty); }
        }

        /// <summary>
        /// Gets or sets the task name.
        /// </summary>
        [ViewModelToModel("Task", "Name")]
        public string TaskName
        {
            get { return GetValue<string>(TaskNameProperty); }
        }

        /// <summary>
        /// Gets or sets the task percentage.
        /// </summary>
        [ViewModelToModel("Task", "Percentage")]
        public int TaskPercentage
        {
            get { return GetValue<int>(TaskPercentageProperty); }
        }
        
        /// <summary>
        /// Gets or sets the task percentage.
        /// </summary>
        [ViewModelToModel("Task", "IsIndeterminate")]
        public bool TaskIsIndeterminate
        {
            get { return GetValue<bool>(TaskIsIndeterminateProperty); }
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

        /// <summary>
        /// The update status.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        /// <param name="totalItems">The total items.</param>
        /// <param name="task">The task</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="task" /> is <c>null</c>.</exception>
        public void UpdateStatus(int currentItem, int totalItems, ITask task)
        {
            Argument.IsNotNull(() => task);

            Task = task;

            _currentItem = currentItem;
            _totalItems = totalItems;

            RaisePropertyChanged(() => Percentage);
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

#endif