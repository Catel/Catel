// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyDependency.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    public interface IDummyDependency
    {
        string Value { get; set; }
    }

    public class DummyDependency : IDummyDependency
    {
        public string Value { get; set; }
    }
}