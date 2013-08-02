// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingFlags.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace System.Reflection
{
    public enum BindingFlags
    {
        CreateInstance = 0x200,
        DeclaredOnly = 2,
        Default = 0,
        ExactBinding = 0x10000,
        FlattenHierarchy = 0x40,
        GetField = 0x400,
        GetProperty = 0x1000,
        IgnoreCase = 1,
        IgnoreReturn = 0x1000000,
        Instance = 4,
        InvokeMethod = 0x100,
        NonPublic = 0x20,
        OptionalParamBinding = 0x40000,
        Public = 0x10,
        PutDispProperty = 0x4000,
        PutRefDispProperty = 0x8000,
        SetField = 0x800,
        SetProperty = 0x2000,
        Static = 8,
        SuppressChangeType = 0x20000
    }
}