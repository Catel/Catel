// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupInfoProviderFixture.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Services
{
    using Catel.Services;

    public class StartUpInfoProviderFixture : IStartUpInfoProvider
    {
        public StartUpInfoProviderFixture(params string[] arguments)
        {
            Arguments = arguments;
        }

        public string[] Arguments { get; private set; }
    }
}