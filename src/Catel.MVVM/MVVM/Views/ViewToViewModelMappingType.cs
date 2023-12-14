namespace Catel.MVVM.Views
{
    /// <summary>
    /// Mapping types for the <see cref="ViewToViewModelAttribute"/>.
    /// </summary>
    public enum ViewToViewModelMappingType
    {
        /// <summary>
        /// Two way, which means that either the view or the view model will update
        /// the values of the other party as soon as they are updated.
        /// <para />
        /// When this value is used, nothing happens when the view model of the view
        /// changes. This way, it might be possible that the values of the view and the
        /// view model are different. The first one to update next will update the other.
        /// </summary>
        TwoWayDoNothing,

        /// <summary>
        /// Two way, which means that either the view or the view model will update
        /// the values of the other party as soon as they are updated.
        /// <para />
        /// When this value is used, the value of the view is used when the view model 
        /// of the view is changed, and is directly transferred to the view model value.
        /// </summary>
        TwoWayViewWins,

        /// <summary>
        /// Two way, which means that either the view or the view model will update
        /// the values of the other party as soon as they are updated.
        /// <para />
        /// When this value is used, the value of the view model is used when the view model 
        /// of the view is changed, and is directly transferred to the view value.
        /// </summary>
        TwoWayViewModelWins,

        /// <summary>
        /// The mapping is from the view to the view model only.
        /// </summary>
        ViewToViewModel,

        /// <summary>
        /// The mapping is from the view model to the view only.
        /// </summary>
        ViewModelToView
    }
}
