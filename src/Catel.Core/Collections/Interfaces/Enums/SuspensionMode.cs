namespace Catel.Collections
{
    using System.Collections.Specialized;

    /// <summary>
    /// The suspension mode.
    /// </summary>
    public enum SuspensionMode
    {
        /// <summary>
        /// No specific mode. When the suspension stops, this will result in a single <see cref="NotifyCollectionChangedAction.Reset"/>
        /// event.
        /// </summary>
        None,

        /// <summary>
        /// Adding mode. When the suspension stops, this will result in a single <see cref="NotifyCollectionChangedAction.Add"/>
        /// event.
        /// </summary>
        Adding,

        /// <summary>
        /// Removing mode. When the suspension stops, this will result in a single <see cref="NotifyCollectionChangedAction.Remove"/>
        /// event.
        /// </summary>
        Removing,

        /// <summary>
        /// Mixed mode (combination of Adding and Removing). This behaves the same as <see cref="None"/>, except that it holds additional
        /// lists of the changed items, their indices and the concrete actions.
        /// </summary>
        Mixed,

        /// <summary>
        /// MixedBash mode (combination of Adding and Removing). This behaves the same as <see cref="Mixed"/>, except
        /// that this raises multiple <see cref="NotifyCollectionChangedAction.Add"/> events and <see cref="NotifyCollectionChangedAction.Remove"/>
        /// events instead of single <see cref="NotifyCollectionChangedAction.Reset"/> event.
        /// </summary>
        MixedBash,

        /// <summary>
        /// MixedConsolidate mode (combination of Adding and Removing). This behaves the same as <see cref="MixedBash"/>, except
        /// that this consolidates those add and remove events which annulled each other.
        /// </summary>
        MixedConsolidate,

        /// <summary>
        /// Silent mode. When the suspension stops, this will result in no event.
        /// </summary>
        Silent
    }
}
