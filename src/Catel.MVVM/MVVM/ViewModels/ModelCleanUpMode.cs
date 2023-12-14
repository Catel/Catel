namespace Catel.MVVM
{
    using System.ComponentModel;

    /// <summary>
    /// Available clean up models for a model.
    /// </summary>
    public enum ModelCleanUpMode
    {
        /// <summary>
        /// Call <see cref="IEditableObject.CancelEdit"/>.
        /// </summary>
        CancelEdit,

        /// <summary>
        /// Call <see cref="IEditableObject.EndEdit"/>.
        /// </summary>
        EndEdit
    }
}
