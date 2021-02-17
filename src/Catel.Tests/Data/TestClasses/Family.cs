namespace Catel.Tests.Data.TestClasses
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Collections.ObjectModel;
    using Catel.Data;

    internal class GrandParent : ChildAwareModelBase
    {
        public void ResetDirtyFlag()
        {
            IsDirty = false;
        }

        public ObservableCollection<Parent> Parents
        {
            get => GetValue<ObservableCollection<Parent>>(ParentsProperty);
            set => SetValue(ParentsProperty, value);
        }

        public static readonly IPropertyData ParentsProperty = RegisterProperty(nameof(Parents), () => new ObservableCollection<Parent>());
    }

    internal class Parent : ChildAwareModelBase
    {
        public void ResetDirtyFlag()
        {
            IsDirty = false;
        }

        public ObservableCollection<Child> Children

        {
            get => GetValue<ObservableCollection<Child>>(ChildrenProperty);
            set => SetValue(ChildrenProperty, value);
        }

        public static readonly IPropertyData ChildrenProperty = RegisterProperty(nameof(Children), () => new ObservableCollection<Child>());

    }

    internal class Child : ModelBase
    {
        public void ResetDirtyFlag()
        {
            IsDirty = false;
        }

        public string Name
        {
            get => GetValue<string>(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public static readonly IPropertyData NameProperty = RegisterProperty(nameof(Name), string.Empty);
    }
}
