// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventArgsConverterBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2019 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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
