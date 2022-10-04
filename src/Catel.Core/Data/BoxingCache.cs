namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Timers;
    using Catel.Logging;

    /// <summary>
    /// Boxing cache helper.
    /// </summary>
    public static partial class BoxingCache
    {
        // Partial class, see T4 template
    }

    /// <summary>
    /// Caches boxed objects to minimize the memory footprint for boxed value types.
    /// </summary>
    public class BoxingCache<T>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        private readonly Dictionary<T, object> _boxedValues = new();
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        private TimeSpan _cleanUpInterval = TimeSpan.FromMinutes(5);

#pragma warning disable IDISP006 // Implement IDisposable.
        private readonly Timer _cleanUpTimer;
#pragma warning restore IDISP006 // Implement IDisposable.

        /// <summary>
        /// Gets the default instance of the boxing cache.
        /// </summary>
        public static BoxingCache<T> Default { get; private set; } = new BoxingCache<T>();

        public BoxingCache()
        {
            _cleanUpTimer = new Timer();
            _cleanUpTimer.AutoReset = false;
            _cleanUpTimer.Elapsed += OnCleanUpTimerElapsed;
        }

        /// <summary>
        /// Gets or sets the clean up interval.
        /// <para />
        /// The default value is 5 minutes.
        /// <para />
        /// To disable automatic clean up, set a value of 0.
        /// </summary>
        public TimeSpan CleanUpInterval
        {
            get
            {
                return _cleanUpInterval;
            }
            set
            {
                _cleanUpInterval = value;

                Log.Debug($"Cleanup interval is set to '{value}'");

                _cleanUpTimer.Stop();

                StartTimerIfRequired();
            }
        }

        /// <summary>
        /// Adds the value to the cache.
        /// </summary>
        /// <param name="value">The value to add to the cache.</param>
        protected object? AddUnboxedValue(T value)
        {
            if (value is null)
            {
                return null;
            }

            var boxedValue = (object)value;

            lock (_boxedValues)
            {
                _boxedValues[value] = boxedValue;
            }

            //lock (_unboxedValues)
            //{
            //    _unboxedValues[boxedValue] = value;
            //}

            StartTimerIfRequired();

            return boxedValue;
        }

        /// <summary>
        /// Adds the value to the cache.
        /// </summary>
        /// <param name="boxedValue">The value to add to the cache.</param>
        protected T AddBoxedValue(object boxedValue)
        {
            var unboxedValue = (T)boxedValue;

            lock (_boxedValues)
            {
                _boxedValues[unboxedValue] = boxedValue;
            }

            //lock (_unboxedValues)
            //{
            //    _unboxedValues[boxedValue] = unboxedValue;
            //}

            StartTimerIfRequired();

            return unboxedValue;
        }

        /// <summary>
        /// Gets the boxed value representing the specified value.
        /// </summary>
        /// <param name="value">The value to box.</param>
        /// <returns>The boxed value.</returns>
        public object? GetBoxedValue(T? value)
        {
            if (value is null)
            {
                return null;
            }

            lock (_boxedValues)
            {
                if (!_boxedValues.TryGetValue(value, out var boxedValue))
                {
                    boxedValue = AddUnboxedValue(value);
                }

                return boxedValue;
            }
        }

        /// <summary>
        /// Gets the unboxed value representing the specified value.
        /// </summary>
        /// <param name="boxedValue">The value to unbox.</param>
        /// <returns>The unboxed value.</returns>
        public T GetUnboxedValue(object boxedValue)
        {
            return (T)boxedValue;
            //lock (_unboxedValues)
            //{
            //    if (!_unboxedValues.TryGetValue(boxedValue, out var unboxedValue))
            //    {
            //        unboxedValue = AddBoxedValue(boxedValue);
            //    }

            //    return unboxedValue;
            //}
        }

        /// <summary>
        /// Invokes a clean up on the cache to release boxed objects from memory allowing them to be garbage collected.
        /// </summary>
        public void CleanUp()
        {
            Log.Debug("Cleaning up boxed values from the cache to decrease memory pressure");

            lock (_boxedValues)
            {
                _boxedValues.Clear();
            }

            //lock (_unboxedValues)
            //{
            //    _unboxedValues.Clear();
            //}
        }

        private void OnCleanUpTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            _cleanUpTimer.Stop();

            CleanUp();
        }

        private void StartTimerIfRequired()
        {
            if (!_cleanUpTimer.Enabled)
            {
                var totalMilliseconds = _cleanUpInterval.TotalMilliseconds;
                if (totalMilliseconds > 0)
                {
                    _cleanUpTimer.Interval = totalMilliseconds;
                    _cleanUpTimer.Start();
                }
            }
        }
    }
}
