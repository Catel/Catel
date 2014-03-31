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

    public partial class NavigationAdapter
    {
        private class ActivityEventArgs : EventArgs
        {
            public ActivityEventArgs(Activity activity)
            {
                Activity = activity;
            }

            public Activity Activity { get; private set; }
        }

        private class ActivityLifecycleCallbacksListener : Java.Lang.Object, Application.IActivityLifecycleCallbacks
        {
            public event EventHandler<ActivityEventArgs> ActivityCreated;

            public event EventHandler<ActivityEventArgs> ActivityDestroyed;

            public event EventHandler<ActivityEventArgs> ActivityPaused;

            public event EventHandler<ActivityEventArgs> ActivityResumed;

            public event EventHandler<ActivityEventArgs> ActivityStarted;

            public event EventHandler<ActivityEventArgs> ActivityStopped;

            public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
            {
                ActivityCreated.SafeInvoke(this, new ActivityEventArgs(activity));
            }

            public void OnActivityDestroyed(Activity activity)
            {
                ActivityDestroyed.SafeInvoke(this, new ActivityEventArgs(activity));
            }

            public void OnActivityPaused(Activity activity)
            {
                ActivityPaused.SafeInvoke(this, new ActivityEventArgs(activity));
            }

            public void OnActivityResumed(Activity activity)
            {
                ActivityResumed.SafeInvoke(this, new ActivityEventArgs(activity));
            }

            public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
            {
                // not required
            }

            public void OnActivityStarted(Activity activity)
            {
                ActivityStarted.SafeInvoke(this, new ActivityEventArgs(activity));
            }

            public void OnActivityStopped(Activity activity)
            {
                ActivityStopped.SafeInvoke(this, new ActivityEventArgs(activity));
            }
        }

        private ActivityLifecycleCallbacksListener _activityLifecycleCallbacksListener;

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

            _activityLifecycleCallbacksListener = new ActivityLifecycleCallbacksListener();
            _activityLifecycleCallbacksListener.ActivityStarted += OnActivityStarted;
            _activityLifecycleCallbacksListener.ActivityPaused += OnActivityPaused;
            _activityLifecycleCallbacksListener.ActivityStopped += OnActivityStopped;

            application.RegisterActivityLifecycleCallbacks(_activityLifecycleCallbacksListener);
        }

        partial void Uninitialize()
        {
            if (_activityLifecycleCallbacksListener != null)
            {
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

        private void OnActivityStarted(object sender, ActivityEventArgs e)
        {
            var eventArgs = new NavigatedEventArgs(e.Activity.LocalClassName, NavigationMode.New);
            RaiseNavigatedTo(eventArgs);
        }

        private void OnActivityPaused(object sender, ActivityEventArgs e)
        {
            // We are navigating away
            var eventArgs = new NavigatingEventArgs(e.Activity.LocalClassName, NavigationMode.New);
            RaiseNavigatingAway(eventArgs);

            //e.Cancel = eventArgs.Cancel;
        }

        private void OnActivityStopped(object sender, ActivityEventArgs e)
        {
            var eventArgs = new NavigatedEventArgs(e.Activity.LocalClassName, NavigationMode.New);
            RaiseNavigatedAway(eventArgs);
        }
    }
}
#endif