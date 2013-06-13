// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestClasses.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.SpecialTest
{
    public class NoNamingConventionViewModel
    {
    }

    public class NoNamingConventionViewModel2
    {
    }

    public class FollowingNoNamingConventionView
    {
    }
}

namespace Catel.Test.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;

    public class PersonViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Register the Name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);
    }
}

namespace Catel.Test.Views
{
    public class PersonView
    {
    }
}

namespace Catel.Test.Controls
{
    public class PersonControl
    {
    }
}

namespace Catel.Test.Windows
{
    public class PersonWindow
    {
    }
}

namespace Catel.Test.Pages
{
    public class PersonPage
    {
    }
}