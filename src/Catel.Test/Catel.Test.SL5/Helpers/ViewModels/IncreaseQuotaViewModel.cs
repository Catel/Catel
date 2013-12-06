namespace Catel.Test.Helpers.ViewModels
{
    using Catel.MVVM;

    /// <summary>
    /// IncreaseQuota view model.
    /// </summary>
    public class IncreaseQuotaViewModel : ViewModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="IncreaseQuotaViewModel"/> class.
        /// </summary>
        public IncreaseQuotaViewModel()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return "Increase quote"; } }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        #endregion

        #region Commands
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the view model. Normally the initialization is done in the constructor, but sometimes this must be delayed
        /// to a state where the associated UI element (user control, window, ...) is actually loaded.
        /// <para/>
        /// This method is called as soon as the associated UI element is loaded.
        /// </summary>
        /// <remarks></remarks>
        protected override void Initialize()
        {
            RaisePropertyChanged(() => Title);
        }
        #endregion
    }
}
