// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventArgsConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2019 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Converters
{
    public interface IEventArgsConverter
    {
        object Convert(object sender, object args);
    }
}
