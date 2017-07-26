// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingFlags.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if (NETFX_CORE && !UWP) || PCL

#pragma warning disable 1591

namespace System.Reflection
{
    [Flags]
    public enum BindingFlags
    {
        // Note: Only use flags available on all platforms (even .net core)
        //CreateInstance = 0x200,
        //DeclaredOnly = 2,
        Default = 0,
        //ExactBinding = 0x10000,
        FlattenHierarchy = 0x40,
        //GetField = 0x400,
        //GetProperty = 0x1000,
        //IgnoreCase = 1,
        //IgnoreReturn = 0x1000000,
        Instance = 4,
        //InvokeMethod = 0x100,
        NonPublic = 0x20,
        //OptionalParamBinding = 0x40000,
        Public = 0x10,
        //PutDispProperty = 0x4000,
        //PutRefDispProperty = 0x8000,
        //SetField = 0x800,
        //SetProperty = 0x2000,
        Static = 8,
        //SuppressChangeType = 0x20000
    }
}

#endif