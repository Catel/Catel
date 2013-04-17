// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavableDataObjectBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;

    /// <summary>
    /// Abstract class that makes the <see cref="DataObjectBase{TDataObject}"/> serializable.
    /// </summary>
    /// <typeparam name="T">Type that the class should hold (same as the defined type).</typeparam>
#if NET
    [Serializable]
#endif
    [ObsoleteEx(Replacement = "SavableModelBase", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
    public abstract class SavableDataObjectBase<T> : SavableModelBase<T>, ISavableDataObjectBase
        where T : class
    {
    }
}