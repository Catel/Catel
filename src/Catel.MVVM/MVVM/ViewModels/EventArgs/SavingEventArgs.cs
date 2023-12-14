namespace Catel.MVVM
{
    /// <summary>
    /// EventArgs for the <see cref="IViewModel.SavingAsync"/> event.
    /// </summary>
    public class SavingEventArgs : CancellableEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SavingEventArgs"/> class.
        /// </summary>
        public SavingEventArgs()
        {
        }
    }
}
