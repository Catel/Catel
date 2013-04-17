namespace Catel.Test.Helpers.Views
{
    using System.IO.IsolatedStorage;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Windows;

    public partial class IncreaseQuotaWindow : DataWindow
    {
        private readonly long _quotaToIncreaseTo;

        static IncreaseQuotaWindow()
        {
            var viewModelLocator = ServiceLocator.Default.ResolveType<IViewModelLocator>();
            viewModelLocator.NamingConventions.Add(string.Format("Catel.Test.Helpers.ViewModels.[VW]ViewModel"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncreaseQuotaWindow"/> class.
        /// </summary>
        /// <param name="quotaToIncreaseTo">The quota to increase to.</param>
        public IncreaseQuotaWindow(long quotaToIncreaseTo)
        {
            _quotaToIncreaseTo = quotaToIncreaseTo;

            InitializeComponent();
        }

        /// <summary>
        /// Applies all changes made by this window.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        protected override bool ApplyChanges()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                store.IncreaseQuotaTo(_quotaToIncreaseTo);
            }

            return base.ApplyChanges();
        }
    }
}

