// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.phone.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if ANDROID
namespace Catel.MVVM.Navigation
{
    using System;
    using Android;
    using Logging;
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
            ActivityCreated?.Invoke(this, eventArgs);
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
            ActivityDestroyed?.Invoke(this, eventArgs);
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
            ActivityPaused?.Invoke(this, eventArgs);
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
            ActivityResumed?.Invoke(this, eventArgs);
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
            ActivityStarted?.Invoke(this, eventArgs);
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
            ActivityStopped?.Invoke(this, eventArgs);
        }
    }

    public partial class NavigationAdapter
    {
        private ActivityLifecycleCallbacksListener _activityLifecycleCallbacksListener;
        private Activity _lastActivity;
        private bool _isFirstTimeLoaded = true;

        partial void Initialize()
        {
            var activity = GetNavigationTarget<Activity>();
            var application = activity.Application;
            if (application is null)
            {
                const string error = "To support navigation events in Android, Catel uses a custom ActivityLifecycleCallbacksListener. This requires an app instance though. Please make sure that the Android app contains an Application class.";
                Log.Error(error);

                throw new NotSupportedException(error);
            }

            _activityLifecycleCallbacksListener = new ActivityLifecycleCallbacksListener(activity);
            _activityLifecycleCallbacksListener.ActivityPaused += OnActivityPaused;
            _activityLifecycleCallbacksListener.ActivityStopped += OnActivityStopped;

            application.RegisterActivityLifecycleCallbacks(_activityLifecycleCallbacksListener);

            // The first time, the general adapter will take care of this
            if (_isFirstTimeLoaded)
            {
                _isFirstTimeLoaded = false;
            }
            else
            {
                // Note: we don't subscribe to ActivityResumed because that equals the Loaded event. This adapter
                // is also created on the loaded
                _lastActivity = activity;

                var eventArgs = new NavigatedEventArgs(GetNavigationUri(activity), NavigationMode.New);
                RaiseNavigatedTo(eventArgs);
            }

            ContextHelper.CurrentContext = activity;
        }

        partial void Uninitialize()
        {
            if (_activityLifecycleCallbacksListener is not null)
            {
                var activity = GetNavigationTarget<Activity>();
                var application = activity.Application;
                application.UnregisterActivityLifecycleCallbacks(_activityLifecycleCallbacksListener);

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
            if (intentExtras is not null)
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

        /// <summary>
        /// Gets the navigation URI for the target page.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        protected override string GetNavigationUri(object target)
        {
            var activity = target as Activity;
            if (activity is null)
            {
                return null;
            }

            return activity.LocalClassName;
        }

        private void OnActivityPaused(object sender, ActivityEventArgs e)
        {
            _lastActivity = e.Activity;

            // We are navigating away
            var eventArgs = new NavigatingEventArgs(GetNavigationUri(e.Activity), NavigationMode.New);
            RaiseNavigatingAway(eventArgs);

            //e.Cancel = eventArgs.Cancel;
        }

        private void OnActivityStopped(object sender, ActivityEventArgs e)
        {
            _lastActivity = e.Activity;

            var eventArgs = new NavigatedEventArgs(GetNavigationUri(e.Activity), NavigationMode.New);
            RaiseNavigatedAway(eventArgs);
        }
    }
}
#endif
