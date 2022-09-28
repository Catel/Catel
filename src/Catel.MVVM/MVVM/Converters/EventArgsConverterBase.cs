namespace Catel.MVVM.Converters
{
    using System;

    public abstract class EventArgsConverterBase<TArgs> : IEventArgsConverter
        where TArgs : EventArgs
    {
        object IEventArgsConverter.Convert(object sender, object args)
        {
            return (args is TArgs eventArgs) ? Convert(sender, eventArgs) : null;
        }

        protected abstract object Convert(object sender, TArgs args);
    }
}
