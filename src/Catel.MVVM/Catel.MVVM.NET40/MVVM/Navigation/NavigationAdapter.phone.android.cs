// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.phone.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if ANDROID
namespace Catel.MVVM.Navigation
{
    using System;
    using Catel.Logging;
    using global::Android.App;
    using global::Android.OS;
    using global::Android.Runtime;

    /// <summary>
    /// Event args for the activity.
    /// </summary>
    public class ActivityEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityEventArgs"/> class.
        /// </summary>
        /// <param name="activity">The activity.</param>
        public ActivityEventArgs(Activity activity)
        {
            Activity = activity;
        }

        /// <summary>
        /// Gets the activity.
        /// </summary>
        /// <value>The activity.</value>
        public Activity Activity { get; private set; }
    }

    /// <summary>
    /// ActivityLifecycleCallbacksListener implementation.
    /// </summary>
    public class ActivityLifecycleCallbacksListener : Java.Lang.Object, Application.IActivityLifecycleCallbacks
    {
        private readonly Activity _activity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLifecycleCallbacksListener"/> class.
        /// </summary>
        public ActivityLifecycleCallbacksListener(Activity activity)
        {
            _activity = activity;

            var catelActivity = activity as Android.App.Activity;
            if (catelActivity != null)
            {
                catelActivity.BackKeyPress += OnBackKeyPress;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLifecycleCallbacksListener"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="transfer">The transfer.</param>
        public ActivityLifecycleCallbacksListener(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }

        /// <summary>
        /// Occurs when the back key is pressed.
        /// </summary>
        public event EventHandler<ActivityEventArgs> BackKeyPressed;

        /// <summary>
        /// Occurs when the activity is created.
        /// </summary>
        public event EventHandler<ActivityEventArgs> ActivityCreated;

        /// <summary>
        /// Occurs when the activity is destroyed.
        /// </summary>
        public event EventHandler<ActivityEventArgs> ActivityDestroyed;

        /// <summary>
        /// Occurs when the activity is paused.
        /// </summary>
        public event EventHandler<ActivityEventArgs> ActivityPaused;

        /// <summary>
        /// Occurs when the activity is resumed.
        /// </summary>
        public event EventHandler<ActivityEventArgs> ActivityResumed;

        /// <summary>
        /// Occurs when the activity is started.
        /// </summary>
        public event EventHandler<ActivityEventArgs> ActivityStarted;

        /// <summary>
        /// Occurs when the activity is stopped.
        /// </summary>
        public event EventHandler<ActivityEventArgs> ActivityStopped;

        /// <summary>
        /// Called when the activity is created.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="savedInstanceState">State of the saved instance.</param>
        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            if (!ReferenceEquals(activity, _activity))
            {
                return;
            }

            var eventArgs = new ActivityEventArgs(activity);
            ActivityCreated.SafeInvoke(this, eventArgs);
        }

        /// <summary>
        /// Called when the activity is destroyed.
        /// </summary>
        /// <param name="activity">The activity.</param>
        public void OnActivityDestroyed(Activity activity)
        {
            if (!ReferenceEquals(activity, _activity))
            {
                return;
            }

            var eventArgs = new ActivityEventArgs(activity);
            ActivityDestroyed.SafeInvoke(this, eventArgs);
        }

        /// <summary>
        /// Called when the activity is paused.
        /// </summary>
        /// <param name="activity">The activity.</param>
        public void OnActivityPaused(Activity activity)
        {
            if (!ReferenceEquals(activity, _activity))
            {
                return;
            }

            var eventArgs = new ActivityEventArgs(activity);
            ActivityPaused.SafeInvoke(this, eventArgs);
        }

        /// <summary>
        /// Called when the activity is resumed.
        /// </summary>
        /// <param name="activity">The activity.</param>
        public void OnActivityResumed(Activity activity)
        {
            if (!ReferenceEquals(activity, _activity))
            {
                return;
            }

            var eventArgs = new ActivityEventArgs(activity);
            ActivityResumed.SafeInvoke(this, eventArgs);
        }

        /// <summary>
        /// Called when the acitvity saves the instance state.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="outState">State of the out.</param>
        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            // not required
        }

        /// <summary>
        /// Called when the activity is started.
        /// </summary>
        /// <param name="activity">The activity.</param>
        public void OnActivityStarted(Activity activity)
        {
            if (!ReferenceEquals(activity, _activity))
            {
                return;
            }

            var eventArgs = new ActivityEventArgs(activity);
            ActivityStarted.SafeInvoke(this, eventArgs);
        }

        /// <summary>
        /// Called when the activity is stopped.
        /// </summary>
        /// <param name="activity">The activity.</param>
        public void OnActivityStopped(Activity activity)
        {
            if (!ReferenceEquals(activity, _activity))
            {
                return;
            }

            var eventArgs = new ActivityEventArgs(activity);
            ActivityStopped.SafeInvoke(this, eventArgs);
        }

        private void OnBackKeyPress(object sender, EventArgs e)
        {
            var eventArgs = new ActivityEventArgs((Activity)sender);
            BackKeyPressed.SafeInvoke(this, eventArgs);
        }
    }

    public partial class NavigationAdapter
    {
        private ActivityLifecycleCallbacksListener _activityLifecycleCallbacksListener;
        private Activity _lastActivity;

        partial void Initialize()
        {
            var activity = GetNavigationTarget<Activity>();
            var application = activity.Application;
            if (application == null)
            {
                const string error = "To support navigation events in Android, Catel uses a custom ActivityLifecycleCallbacksListener. This requires an app instance though. Please make sure that the Android app contains an Application class.";
                Log.Error(error);

                throw new NotSupportedException(error);
            }

            _activityLifecycleCallbacksListener = new ActivityLifecycleCallbacksListener(activity);
            _activityLifecycleCallbacksListener.BackKeyPressed += OnActivityBackKeyPressed;
            _activityLifecycleCallbacksListener.ActivityResumed += OnActivityResumed;
            _activityLifecycleCallbacksListener.ActivityPaused += OnActivityPaused;
            _activityLifecycleCallbacksListener.ActivityStopped += OnActivityStopped;

            application.RegisterActivityLifecycleCallbacks(_activityLifecycleCallbacksListener);
        }

        partial void Uninitialize()
        {
            if (_activityLifecycleCallbacksListener != null)
            {
                var activity = GetNavigationTarget<Activity>();
                var application = activity.Application;
                application.UnregisterActivityLifecycleCallbacks(_activityLifecycleCallbacksListener);

                _activityLifecycleCallbacksListener.BackKeyPressed -= OnActivityBackKeyPressed;
                _activityLifecycleCallbacksListener.ActivityResumed -= OnActivityResumed;
                _activityLifecycleCallbacksListener.ActivityPaused -= OnActivityPaused;
                _activityLifecycleCallbacksListener.ActivityStopped -= OnActivityStopped;
                _activityLifecycleCallbacksListener.Dispose();
                _activityLifecycleCallbacksListener = null;
            }
        }

        partial void DetermineNavigationContext()
        {
            var activity = GetNavigationTarget<Activity>();
            var intentExtras = activity.Intent.Extras;
            if (intentExtras != null)
            {
                foreach (var item in intentExtras.KeySet())
                {
                    NavigationContext.Values[item] = intentExtras.Get(item);
                }
            }
        }

        /// <summary>
        /// Determines whether the navigation can be handled by this adapter.
        /// </summary>
        /// <returns><c>true</c> if the navigation can be handled by this adapter; otherwise, <c>false</c>.</returns>
        protected override bool CanHandleNavigation()
        {
            return ReferenceEquals(_lastActivity, NavigationTarget);
        }

        private void OnActivityResumed(object sender, ActivityEventArgs e)
        {
            _lastActivity = e.Activity;

            var eventArgs = new NavigatedEventArgs(e.Activity.LocalClassName, NavigationMode.New);
            RaiseNavigatedTo(eventArgs);
        }

        private void OnActivityBackKeyPressed(object sender, ActivityEventArgs e)
        {
            _lastActivity = e.Activity;

            var eventArgs = new NavigatedEventArgs(e.Activity.LocalClassName, NavigationMode.Back);
            RaiseNavigatedAway(eventArgs);
        }

        private void OnActivityPaused(object sender, ActivityEventArgs e)
        {
            _lastActivity = e.Activity;

            // We are navigating away
            var eventArgs = new NavigatingEventArgs(e.Activity.LocalClassName, NavigationMode.New);
            RaiseNavigatingAway(eventArgs);

            //e.Cancel = eventArgs.Cancel;
        }

        private void OnActivityStopped(object sender, ActivityEventArgs e)
        {
            _lastActivity = e.Activity;

            var eventArgs = new NavigatedEventArgs(e.Activity.LocalClassName, NavigationMode.New);
            RaiseNavigatedAway(eventArgs);
        }
    }
}
#endif