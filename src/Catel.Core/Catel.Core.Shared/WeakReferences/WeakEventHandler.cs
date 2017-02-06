namespace Catel
{
    using System;

    /// <summary>
    /// Provides helpers that ease subscription to events that should be auto-detached if it is the only reference between observer and observed
    /// </summary>
    public static class WeakEventHandler
    {
        /// <summary>
        /// Subscribes weakly event-handler to event
        /// </summary>
        /// <example>
        /// WeakEventHandler
        ///     .Subscribe(
        ///         source, 
        ///         OnSomethingHappens, 
        ///         (s, handler) => s.SomethingHappens += handler,
        ///         (s, handler) => s.SomethingHappens += handler)
        /// </example>
        /// <typeparam name="TSource">Type of observed object</typeparam>
        /// <param name="source">Observed object</param>
        /// <param name="eventHandler">Method that should be called when event is raised</param>
        /// <param name="attachAction">It should contains (source, handler) => source.Event += handler</param>
        /// <param name="detachAction">It should contains (source, handler) => source.Event -= handler</param>
        /// <returns>Disposing of result allows manual detach</returns>
        public static IDisposable Subscribe<TSource>(
            TSource source,
            EventHandler eventHandler,
            Action<TSource, EventHandler> attachAction,
            Action<TSource, EventHandler> detachAction)
        {
            return new WeakEventWrapper<TSource>(source, eventHandler, attachAction, detachAction);
        }

        /// <summary>
        /// Subscribes weakly event-handler to event
        /// </summary>
        /// <example>
        /// WeakEventHandler
        ///     .Subscribe(
        ///         source, 
        ///         OnSomethingHappens, 
        ///         (s, handler) => s.SomethingHappens += handler,
        ///         (s, handler) => s.SomethingHappens += handler)
        /// </example>
        /// <typeparam name="TSource">Type of observed object</typeparam>
        /// <typeparam name="TArgs">Type of event&apos;s argument</typeparam>
        /// <param name="source">Observed object</param>
        /// <param name="eventHandler">Method that should be called when event is raised</param>
        /// <param name="attachAction">It should contains (source, handler) => source.Event += handler</param>
        /// <param name="detachAction">It should contains (source, handler) => source.Event -= handler</param>
        /// <returns>Disposing of result allows manual detach</returns>
        public static IDisposable Subscribe<TSource, TArgs>(
            TSource source,
            EventHandler<TArgs> eventHandler,
            Action<TSource, EventHandler<TArgs>> attachAction,
            Action<TSource, EventHandler<TArgs>> detachAction) where TArgs : EventArgs
        {
            return new WeakEventWrapper<TSource, TArgs>(source, eventHandler, attachAction, detachAction);
        }
        
        private sealed class WeakEventWrapper<TSource>: IDisposable
        {
            private readonly TSource _source;
            private readonly Action<TSource, EventHandler> _detachAction;
            private readonly WeakReference<EventHandler> _handlerReference;
            
            public WeakEventWrapper(
                TSource source,
                EventHandler handler,
                Action<TSource, EventHandler> attachAction,
                Action<TSource, EventHandler> detachAction)
            {
                _source = source;
                _detachAction = detachAction;
                _handlerReference = new WeakReference<EventHandler>(handler);
                attachAction(source, OnEvent);
            }

            private void OnEvent(object sender, EventArgs e)
            {
                EventHandler eventHandler;
                if (_handlerReference.TryGetTarget(out eventHandler))
                {
                    eventHandler(sender, e);
                }
                else
                {
                    // This method shouldn't be called internally but it makes sense in here
                    Dispose();
                }
            }

            #region IDisposable

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            // ReSharper disable once UnusedParameter.Local
            private void Dispose(bool isDisposing)
            {
                _detachAction(_source, OnEvent);
            }

            ~WeakEventWrapper()
            {
                Dispose(false);
            }

            #endregion
        }

        // Code duplication but I have no idea how to avoid it

        private sealed class WeakEventWrapper<TSource, TArgs> : IDisposable where TArgs : EventArgs
        {
            private readonly TSource _source;
            private readonly Action<TSource, EventHandler<TArgs>> _detachAction;
            private readonly WeakReference<EventHandler<TArgs>> _handlerReference;

            public WeakEventWrapper(
                TSource source,
                EventHandler<TArgs> handler,
                Action<TSource, EventHandler<TArgs>> attachAction,
                Action<TSource, EventHandler<TArgs>> detachAction)
            {
                _source = source;
                _detachAction = detachAction;
                _handlerReference = new WeakReference<EventHandler<TArgs>>(handler);
                attachAction(source, OnEvent);
            }

            private void OnEvent(object sender, TArgs e)
            {
                EventHandler<TArgs> eventHandler;
                if (_handlerReference.TryGetTarget(out eventHandler))
                {
                    eventHandler(sender, e);
                }
                else
                {
                    // This method shouldn't be called internally but it makes sense in here
                    Dispose();
                }
            }

            #region IDisposable

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            // ReSharper disable once UnusedParameter.Local
            private void Dispose(bool isDisposing)
            {
                _detachAction(_source, OnEvent);
            }

            ~WeakEventWrapper()
            {
                Dispose(false);
            }

            #endregion
        }
    }
}
