namespace Catel.Tests.MVVM.ViewModels.TestClasses;

using System;
using System.Collections.Generic;
using Catel.MVVM;

public class ViewModelFactoryArrayModelTestViewModel : ViewModelBase
{
    public ViewModelFactoryArrayModelTestViewModel(string[] items, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        Items = items;
    }

    public string[]? Items { get; }
}

public class ViewModelFactoryReadOnlyListModelTestViewModel : ViewModelBase
{
    public ViewModelFactoryReadOnlyListModelTestViewModel(IReadOnlyList<string> items, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        Items = items;
    }

    public IReadOnlyList<string>? Items { get; }
}

public class ViewModelFactoryEnumerableModelTestViewModel : ViewModelBase
{
    public ViewModelFactoryEnumerableModelTestViewModel(IEnumerable<string> items, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        Items = items;
    }

    public IEnumerable<string>? Items { get; }
}
