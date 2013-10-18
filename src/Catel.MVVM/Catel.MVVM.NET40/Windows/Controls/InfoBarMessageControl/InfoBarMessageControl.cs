// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoBarMessageControl.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;

    using Logging;

#if NET
    using System.ComponentModel;
    using System.Windows.Data;
    using Exceptions = MVVM.Properties.Exceptions;
#endif

    /// <summary>
    /// The display mode for the <see cref="InfoBarMessageControl"/>.
    /// </summary>
    public enum InfoBarMessageControlMode
    {
        /// <summary>
        /// Displays the control inline, which means all controls below are moved down a bit when the
        /// control becomes visible.
        /// </summary>
        Inline,

        /// <summary>
        /// Displays the control as an overlay, which might lead to overlapping of existing controls.
        /// </summary>
        Overlay
    }

    /// <summary>
    /// Control for displaying messages to the user.
    /// </summary>
    /// <remarks>
    /// A long, long, long time ago, the messages were hold in a dependency property (DP). However, even though DP values are
    /// not static, several instances that were open at the same time were still clearing eachother values (thus it seemed the
    /// DP behaves like it's a static member). Therefore, the messages are now hold in a field, and all problems are now gone.
    /// <para />
    /// And the control lived happily ever after.
    /// </remarks>
    [TemplatePart(Name = ElementMessageBar, Type = typeof(FrameworkElement))]
    public class InfoBarMessageControl : ContentControl
    {
        #region Constants
        /// <summary>
        /// The bar that will show the initial message bar.
        /// </summary>
        private const string ElementMessageBar = "PART_MessageBar";
        #endregion

        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly object _lock = new object();

        private readonly List<object> _objectsToIgnore = new List<object>();
        private readonly Dictionary<object, List<string>> _warnings = new Dictionary<object, List<string>>();
        private readonly Dictionary<object, List<string>> _errors = new Dictionary<object, List<string>>();

        private readonly ObservableCollection<string> _warningMessages = new ObservableCollection<string>();
        private readonly ObservableCollection<string> _errorMessages = new ObservableCollection<string>();

        private readonly Dictionary<int, WarningAndErrorValidator> _warningAndErrorValidators = new Dictionary<int, WarningAndErrorValidator>(); 
        private bool _subscribedToEvents;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="InfoBarMessageControl"/> class.
        /// </summary>
        static InfoBarMessageControl()
        {
            DefaultTextPropertyValue = MVVM.Properties.Resources.InfoBarMessageControlErrorTitle;

#if NET
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InfoBarMessageControl), new FrameworkPropertyMetadata(typeof(InfoBarMessageControl)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoBarMessageControl"/> class.
        /// </summary>
        public InfoBarMessageControl()
        {
            Text = DefaultTextPropertyValue;
            IsTabStop = false;

#if !NET
            DefaultStyleKey = typeof (InfoBarMessageControl);
#endif

#if NET
            Focusable = false;
#endif

            if (Catel.Environment.IsInDesignMode)
            {
                return;
            }

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the default property value for the <see cref="Text"/> property.
        /// </summary>
        /// <value>The default text property value.</value>
        public static string DefaultTextPropertyValue { get; set; }

        /// <summary>
        /// Gets or sets the mode in which the control is displayed.
        /// </summary>
        /// <value>The mode in which the control is displayed.</value>
        public InfoBarMessageControlMode Mode
        {
            get { return (InfoBarMessageControlMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for Mode.
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(InfoBarMessageControlMode), typeof(InfoBarMessageControl),
            new PropertyMetadata(InfoBarMessageControlMode.Inline, (sender, e) => ((InfoBarMessageControl)sender).OnModeChanged()));

        /// <summary>
        /// Gets or sets the text to display when there are warnings and/or messages.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for Text.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InfoBarMessageControl), new PropertyMetadata(Catel.Environment.DefaultMultiLingualDependencyPropertyValue));

        /// <summary>
        /// Info message for the info bar.
        /// </summary>
        public string InfoMessage
        {
            get { return (string)GetValue(InfoMessageProperty); }
            set { SetValue(InfoMessageProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for InfoMessage.
        /// </summary>
        public static readonly DependencyProperty InfoMessageProperty =
            DependencyProperty.Register("InfoMessage", typeof(string), typeof(InfoBarMessageControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets MessageCount.
        /// </summary>
        /// <remarks>
        /// Wrapper for the MessageCount dependency property.
        /// </remarks>
        public int MessageCount
        {
            get { return (int)GetValue(MessageCountProperty); }
            private set { SetValue(MessageCountProperty, value); }
        }

        /// <summary>
        /// Definition of the dependency property is private.
        /// </summary>
        public static readonly DependencyProperty MessageCountProperty =
            DependencyProperty.Register("MessageCount", typeof(int), typeof(InfoBarMessageControl), new PropertyMetadata(0));

        /// <summary>
        /// Gets the warning message collection.
        /// </summary>
        /// <value>The warning message collection.</value>
        /// <remarks>
        /// This property is not defined as dependency property, since it seems to cause some issues when several windows/controls with
        /// this control are open at the same time (dependency properties seem to behave static, but they shouldn't).
        /// </remarks>
        public ObservableCollection<string> WarningMessageCollection
        {
            get { return _warningMessages; }
        }

        /// <summary>
        /// Gets the error message collection.
        /// </summary>
        /// <value>The error message collection.</value>
        /// <remarks>
        /// This property is not defined as dependency property, since it seems to cause some issues when several windows/controls with
        /// this control are open at the same time (dependency properties seem to behave static, but they shouldn't).
        /// </remarks>
        public ObservableCollection<string> ErrorMessageCollection
        {
            get { return _errorMessages; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when the control is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SubscribeToEvents();
        }

        /// <summary>
        /// Called when the control is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            UnsubscribeFromEvents();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <c>System.Windows.FrameworkElement.ApplyTemplate</c>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

#if NET
            if (GetTemplateChild(ElementMessageBar) == null)
            {
                throw new NotSupportedException(string.Format(Exceptions.ControlTemplateMustContainPart, ElementMessageBar));
            }
#endif

            OnModeChanged();
        }

        /// <summary>
        /// Called when the <see cref="Mode"/> property has changed.
        /// </summary>
        private void OnModeChanged()
        {
            var messageBar = GetTemplateChild(ElementMessageBar) as FrameworkElement;
            if (messageBar != null)
            {
                int gridRow = 0;

                switch (Mode)
                {
                    case InfoBarMessageControlMode.Inline:
                        gridRow = 0;
                        break;

                    case InfoBarMessageControlMode.Overlay:
                        gridRow = 1;
                        break;
                }

                messageBar.SetValue(Grid.RowProperty, gridRow);
            }
        }

        /// <summary>
        /// Subscribes an instance of the <see cref="WarningAndErrorValidator"/> class.
        /// </summary>
        /// <param name="validator">The validator.</param>
        /// <remarks>
        /// Keep in mind that this method is normally handled by Catel. Only use this method if you really know
        /// what you are doing.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="validator"/> is <c>null</c>.</exception>
        public void SubscribeWarningAndErrorValidator(WarningAndErrorValidator validator)
        {
            Argument.IsNotNull("validator", validator);

            if (!_warningAndErrorValidators.ContainsKey(validator.UniqueIdentifier))
            {
                validator.Validation += OnInfoBarMessageValidation;
                _warningAndErrorValidators.Add(validator.UniqueIdentifier, validator);
            }
        }

        /// <summary>
        /// Unsubscribes the warning and error validator.
        /// </summary>
        /// <param name="validator">The validator.</param>
        /// <remarks>
        /// Keep in mind that this method is normally handled by Catel. Only use this method if you really know
        /// what you are doing.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="validator"/> is <c>null</c>.</exception>
        public void UnsubscribeWarningAndErrorValidator(WarningAndErrorValidator validator)
        {
            Argument.IsNotNull("validator", validator);

            if (_warningAndErrorValidators.ContainsKey(validator.UniqueIdentifier))
            {
                validator.Validation -= OnInfoBarMessageValidation;
                _warningAndErrorValidators.Remove(validator.UniqueIdentifier);
            }
        }

        /// <summary>
        /// Subscribes to events.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_subscribedToEvents)
            {
                return;
            }

#if NET
            Validation.AddErrorHandler(this, OnInfoBarMessageErrorValidation);
#endif

            _subscribedToEvents = true;
        }

        /// <summary>
        /// Unsubscribes from events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (!_subscribedToEvents)
            {
                return;
            }

#if NET
            Validation.RemoveErrorHandler(this, OnInfoBarMessageErrorValidation);
#endif

            foreach (var warningAndErrorValidator in _warningAndErrorValidators)
            {
                warningAndErrorValidator.Value.Validation -= OnInfoBarMessageValidation;
            }

            _warningAndErrorValidators.Clear();

            _subscribedToEvents = false;
        }

        /// <summary>
        /// Clears the object messages for the specified binding object.
        /// </summary>
        /// <param name="bindingObject">The binding object.</param>
        /// <remarks>
        /// This method is implemented because of the DataContext issue (DataContext cannot be changed before a
        /// user control is loaded, and therefore might be binding to the wrong object).
        /// </remarks>
        internal void ClearObjectMessages(object bindingObject)
        {
            object realBindingObject = bindingObject;

            ProcessValidationMessage(realBindingObject, null, ValidationEventAction.ClearAll, ValidationType.Warning);
            ProcessValidationMessage(realBindingObject, null, ValidationEventAction.ClearAll, ValidationType.Error);

            UpdateMessages();
        }

        /// <summary>
        /// Adds an object to the ignore list so this control does not show messages for the specified object any longer.
        /// </summary>
        /// <param name="bindingObject">The binding object.</param>
        internal void IgnoreObject(object bindingObject)
        {
            object realBindingObject = bindingObject;

            _objectsToIgnore.Add(realBindingObject);

            ClearObjectMessages(bindingObject);
        }

#if NET
        /// <summary>
        /// Handling data errors.
        /// </summary>
        /// <param name="sender">A sender.</param>
        /// <param name="e">The event arguments</param>
        private void OnInfoBarMessageErrorValidation(object sender, ValidationErrorEventArgs e)
        {
            e.Handled = true;

            var validationEventAction = ValidationEventAction.Added;
            if (e.Action == ValidationErrorEventAction.Added)
            {
                validationEventAction = ValidationEventAction.Added;
            }
            else if (e.Action == ValidationErrorEventAction.Removed)
            {
                validationEventAction = ValidationEventAction.Removed;
            }

            object bindingObject = GetBindingObject(e.Error.BindingInError);
            string message = (e.Error != null) ? e.Error.ErrorContent.ToString() : string.Empty;

            // There seems to be an issue where validations are removed, even when 
            // ((IDataErrorInfo)bindingObject)["property"] has a value, so check for that

            var bindingObjectAsIDataErrorInfo = bindingObject as IDataErrorInfo;
            var bindingInErrorAsBindingExpression = e.Error.BindingInError as BindingExpression;
            if ((validationEventAction == ValidationEventAction.Removed) && (bindingObjectAsIDataErrorInfo != null) &&
                (bindingInErrorAsBindingExpression != null))
            {
                if (!string.IsNullOrEmpty(bindingObjectAsIDataErrorInfo[bindingInErrorAsBindingExpression.ParentBinding.Path.Path]))
                {
                    Log.Debug("Received 'Remove' action for error '{0}', but it is invalid because the error still exists on the object", message);
                    return;
                }
            }

            ProcessValidationMessage(bindingObject, message, validationEventAction, ValidationType.Error);

            UpdateMessages();
        }
#endif

        /// <summary>
        /// Handling business data errors.
        /// </summary>
        /// <param name="sender">A sender.</param>
        /// <param name="e">The event arguments</param>
        private void OnInfoBarMessageValidation(object sender, ValidationEventArgs e)
        {
            ProcessValidationMessage(e.Value, e.Message, e.Action, e.Type);

            UpdateMessages();
        }

#if NET
        /// <summary>
        /// Gets the binding object.
        /// </summary>
        /// <param name="bindingObject">The binding object.</param>
        /// <returns>object from the binding.</returns>
        private static object GetBindingObject(object bindingObject)
        {
            object result;

            // Check whether the data error is throwed on an single binding or a bindinggroup and process the error message
            if (bindingObject as BindingExpression != null)
            {
                // Use data item of binding
                result = ((BindingExpression)bindingObject).DataItem;
            }
            else if (bindingObject as BindingGroup != null)
            {
                // Use data group (object itself)
                // ReSharper disable RedundantCast
                result = ((BindingGroup)bindingObject);
                // ReSharper restore RedundantCast
            }
            else
            {
                // Just use the object
                result = bindingObject;
            }

            return result;
        }
#endif

        /// <summary>
        /// Process an validation message.
        /// </summary>
        /// <param name="bindingObject">The binding object which will be used as key in dictionary with error messages. Allowed to be <c>null</c> if <see cref="ValidationEventAction.ClearAll"/>.</param>
        /// <param name="message">The actual warning or error message.</param>
        /// <param name="action">An error event action. See <see cref="ValidationErrorEventAction"/>.</param>
        /// <param name="type">The validation type.</param>
        private void ProcessValidationMessage(object bindingObject, string message, ValidationEventAction action, ValidationType type)
        {
            if ((action != ValidationEventAction.ClearAll) && (bindingObject == null))
            {
                Log.Warning("Null-values are not allowed when not using ValidationEventAction.ClearAll");
                return;
            }

            if (_objectsToIgnore.Contains(bindingObject) && (action != ValidationEventAction.ClearAll))
            {
                Log.Debug("Object '{0}' is in the ignore list, thus messages will not be handled", bindingObject);
                return;
            }

            Dictionary<object, List<string>> messages = (type == ValidationType.Warning) ? _warnings : _errors;

            lock (_lock)
            {
                switch (action)
                {
                    case ValidationEventAction.Added:
                    case ValidationEventAction.Refresh:
                        if (!messages.ContainsKey(bindingObject))
                        {
                            messages.Add(bindingObject, new List<string>());
                        }

                        if (!messages[bindingObject].Contains(message))
                        {
                            messages[bindingObject].Add(message);
                        }
                        break;

                    case ValidationEventAction.Removed:
                        if (messages.ContainsKey(bindingObject))
                        {
                            messages[bindingObject].Remove(message);
                        }
                        break;

                    case ValidationEventAction.ClearAll:
                        if (bindingObject != null)
                        {
                            messages.Remove(bindingObject);
                        }
                        else
                        {
                            messages.Clear();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Update the content of the control with the found warnings and errors.
        /// </summary>
        private void UpdateMessages()
        {
            lock (_lock)
            {
                UpdatesMessageCollection(_warningMessages, _warnings);
                UpdatesMessageCollection(_errorMessages, _errors);
            }

            MessageCount = _warningMessages.Count + _errorMessages.Count;

            InfoMessage = (MessageCount > 0) ? Text : string.Empty;
        }

        /// <summary>
        /// Updates a message collection by adding new messages and removing old ones that no longer exist.
        /// </summary>
        /// <param name="messageCollection">The message collection.</param>
        /// <param name="messageSource">The message source.</param>
        private static void UpdatesMessageCollection(ObservableCollection<string> messageCollection, Dictionary<object, List<string>> messageSource)
        {
            foreach (List<string> sourceMessageCollection in messageSource.Values)
            {
                foreach (string message in sourceMessageCollection)
                {
                    if (!messageCollection.Contains(message))
                    {
                        messageCollection.Add(message);
                    }
                }
            }

            for (int i = messageCollection.Count - 1; i >= 0; i--)
            {
                string message = messageCollection[i];

                bool isValid = false;
                foreach (List<string> sourceMessageCollection in messageSource.Values)
                {
                    if (sourceMessageCollection.Contains(message))
                    {
                        isValid = true;
                        break;
                    }
                }

                if (!isValid)
                {
                    messageCollection.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}
