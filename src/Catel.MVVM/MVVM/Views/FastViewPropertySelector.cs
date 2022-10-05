namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// Very fast view property selector because it does not select any view properties.
    /// <para />
    /// Use this one for best performance but loose the automatic view property change notifications.
    /// </summary>
    public class FastViewPropertySelector : ViewPropertySelector
    {
        /// <summary>
        /// Determines whether all view properties must be subscribed for this type.
        /// </summary>
        /// <param name="targetViewType">Type of the target view.</param>
        /// <returns><c>true</c> if all view properties must be subscribed to, <c>false</c> otherwise.</returns>
        public override bool MustSubscribeToAllViewProperties(Type targetViewType)
        {
            return false;
        }
    }
}
