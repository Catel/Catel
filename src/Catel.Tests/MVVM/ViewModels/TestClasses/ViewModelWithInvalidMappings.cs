namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using System;
    using Catel.MVVM;

    /// <summary>
    /// View model with invalid mappings.
    /// </summary>
    public class ViewModelWithInvalidMappings : ViewModelBase
    {
        public ViewModelWithInvalidMappings(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "View model with invalid mappings"; }
        }
    }
}
