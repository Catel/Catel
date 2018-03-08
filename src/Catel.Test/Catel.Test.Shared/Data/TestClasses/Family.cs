using System;
using System.Collections.Generic;
using System.Text;

namespace Catel.Test.Data.TestClasses
{
    using System.Collections.ObjectModel;
    using Catel.Data;
    class GrandParent : ChildAwareModelBase
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

        public static readonly PropertyData ParentsProperty = RegisterProperty(nameof(Parents), typeof(ObservableCollection<Parent>), () => new ObservableCollection<Parent>());
    }

    class Parent : ChildAwareModelBase
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

        public static readonly PropertyData ChildrenProperty = RegisterProperty(nameof(Children), typeof(ObservableCollection<Child>), () => new ObservableCollection<Child>());

    }

    class Child : ModelBase
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

        public static readonly PropertyData NameProperty = RegisterProperty(nameof(Name), typeof(string), string.Empty);
    }
}
