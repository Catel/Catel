// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestClasses.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.SpecialTest
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

namespace Catel.Tests.ViewModels
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
        public static readonly IPropertyData NameProperty = RegisterProperty("Name", typeof(string), null);
    }
}

namespace Catel.Tests
{
    using Catel.Data;
    using Catel.MVVM;

    public class SameNamespacePersonViewModel : ViewModelBase
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
        public static readonly IPropertyData NameProperty = RegisterProperty("Name", typeof(string), null);
    }

    public class SameNamespacePersonView
    {
    }
}

namespace Catel.Tests.Views
{
    public class PersonView
    {
    }
}

namespace Catel.Tests.Controls
{
    public class PersonControl
    {
    }
}

namespace Catel.Tests.Windows
{
    public class PersonWindow
    {
    }
}

namespace Catel.Tests.Pages
{
    public class PersonPage
    {
    }
}