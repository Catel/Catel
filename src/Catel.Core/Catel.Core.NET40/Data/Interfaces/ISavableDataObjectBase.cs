// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISavableDataObjectBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    /// <summary>
    /// ISavableDataObjectBase that defines the additional methods to save a <see cref="IDataObject"/> object.
    /// </summary>
    [ObsoleteEx(Replacement = "ISavableModel", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
    public interface ISavableDataObjectBase : ISavableModel
    {
    }
}