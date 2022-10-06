namespace Catel.Windows.Interactivity
{
    using System.Windows;
    using Microsoft.Xaml.Behaviors;
    using System;
    using System.ComponentModel;
    using Logging;

    /// <summary>
    /// Available moments on which the focus can be set.
    /// </summary>
    public enum FocusMoment
    {
        /// <summary>
        /// Focus when the control is loaded.
        /// </summary>
        Loaded,

        /// <summary>
        /// Focus when a property has changed.
        /// </summary>
        PropertyChanged,

        /// <summary>
        /// Focus when a specific event occurs.
        /// </summary>
        Event
    }

    /// <summary>
    /// Behavior to set focus to a <see cref="FrameworkElement"/>. This behavior sets the focus
    /// only once on the first time the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
    /// </summary>
    public class Focus : FocusBehaviorBase
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private Catel.IWeakEventListener? _weakEventListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="Focus"/> class.
        /// </summary>
        public Focus()
        {
        }

        /// <summary>
        /// Gets or sets the focus moment.
        /// <para />
        /// When this value is <see cref="Interactivity.FocusMoment.Loaded" />, no other properties need to be set.
        /// <para />
        /// When this value is <see cref="Interactivity.FocusMoment.PropertyChanged" />, both the <see cref="Source" /> and 
        /// <see cref="PropertyName" /> must be set.
        /// <para />
        /// When this value is <see cref="Interactivity.FocusMoment.Event" />, both the <see cref="Source" /> and 
        /// <see cref="EventName" /> must be set.
        /// </summary>
        /// <value>The focus moment.</value>
        public FocusMoment FocusMoment
        {
            get { return (FocusMoment)GetValue(FocusMomentProperty); }
            set { SetValue(FocusMomentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for FocusMoment.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty FocusMomentProperty =
            DependencyProperty.Register(nameof(FocusMoment), typeof(FocusMoment), typeof(Focus), new PropertyMetadata(FocusMoment.Loaded));

        /// <summary>
        /// Gets or sets the source. This value is required when the <see cref="FocusMoment" /> property is either 
        /// <see cref="Interactivity.FocusMoment.PropertyChanged" /> or <see cref="Interactivity.FocusMoment.Event" />.
        /// </summary>
        /// <value>The source.</value>
        public object? Source
        {
            get { return (object?)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(Focus), 
            new PropertyMetadata(null, (sender, e) => ((Focus)sender).OnSourceChanged(e)));

        /// <summary>
        /// Gets or sets the name of the property. This value is required when the <see cref="FocusMoment" /> property is 
        /// <see cref="Interactivity.FocusMoment.PropertyChanged" />.
        /// </summary>
        /// <value>The name of the property.</value>
        public string? PropertyName
        {
            get { return (string?)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(Focus), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the name of the event. This value is required when the <see cref="FocusMoment" /> property is 
        /// <see cref="Interactivity.FocusMoment.Event" />.
        /// </summary>
        /// <value>The name of the event.</value>
        public string? EventName
        {
            get { return (string?)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EventName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register(nameof(EventName), typeof(string), typeof(Focus), new PropertyMetadata(null));

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            if (!IsFocusAlreadySet && (FocusMoment == FocusMoment.Loaded))
            {
                StartFocus();
            }
        }

        /// <summary>
        /// Called when the event on the <see cref="Source" /> has occurred.
        /// </summary>
        private void OnSourceEventOccurred()
        {
            StartFocus();
        }

        /// <summary>
        /// Called when a property on the <see cref="Source" /> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyName)
            {
                StartFocus();
            }
        }

        /// <summary>
        /// Called when the source has changed.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldSource = e.OldValue;
            if (oldSource is not null)
            {
                switch (FocusMoment)
                {
                    case FocusMoment.Event:
                        _weakEventListener?.Detach();
                        _weakEventListener = null;
                        break;

                    case FocusMoment.PropertyChanged:
                        var sourceAsPropertyChanged = oldSource as INotifyPropertyChanged;
                        if (sourceAsPropertyChanged is not null)
                        {
                            sourceAsPropertyChanged.PropertyChanged -= OnSourcePropertyChanged;
                        }
                        else
                        {
                            Log.Warning("Cannot unsubscribe from previous source because it does not implement 'INotifyPropertyChanged', this should not be possible and can lead to memory leaks");
                        }
                        break;
                }
            }

            var newSource = e.NewValue;
            if (newSource is not null)
            {
                switch (FocusMoment)
                {
                    case FocusMoment.Event:
                        if (string.IsNullOrEmpty(EventName))
                        {
                            throw new InvalidOperationException("Property 'EventName' is required when FocusMode is 'FocusMode.Event'");
                        }

                        _weakEventListener = this.SubscribeToWeakEvent(newSource, EventName, OnSourceEventOccurred);
                        break;

                    case FocusMoment.PropertyChanged:
                        if (string.IsNullOrEmpty(PropertyName))
                        {
                            throw new InvalidOperationException("Property 'PropertyName' is required when FocusMode is 'FocusMode.PropertyChanged'");
                        }

                        var sourceAsPropertyChanged = newSource as INotifyPropertyChanged;
                        if (sourceAsPropertyChanged is null)
                        {
                            throw new InvalidOperationException("Source does not implement interface 'INotifyfPropertyChanged', either implement it or change the 'FocusMode'");
                        }

                        sourceAsPropertyChanged.PropertyChanged += OnSourcePropertyChanged;
                        break;
                }
            }
        }
    }
}
