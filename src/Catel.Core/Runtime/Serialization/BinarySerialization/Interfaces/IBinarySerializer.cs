// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBinarySerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET


namespace Catel.Runtime.Serialization.Binary
{
    /// <summary>
    /// Interface for the binary serializer.
    /// </summary>
    [ObsoleteEx(Message = "Binary serialization should no longer be used for security reasons, see https://github.com/Catel/Catel/issues/1216", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
    public interface IBinarySerializer : ISerializer
    {
    }
}

#endif
