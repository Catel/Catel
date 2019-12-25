// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyListChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Collections
{
    using System.ComponentModel;

    /// <summary>
    /// The notify list changed event args.
    /// </summary>
    public class NotifyListChangedEventArgs : ListChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of change.</param>
        public NotifyListChangedEventArgs(ListChangedType listChangedType)
            : base(listChangedType, -1)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of change.</param>
        /// <param name="newIndex">The index of the item that was added, changed, or removed.</param>
        public NotifyListChangedEventArgs(ListChangedType listChangedType, int newIndex)
            : base(listChangedType, newIndex)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of change.</param>
        /// <param name="propDesc">The <see cref="PropertyDescriptor"/> that was added, removed, or changed.</param>
        public NotifyListChangedEventArgs(ListChangedType listChangedType, PropertyDescriptor propDesc)
            : base(listChangedType, propDesc)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of change.</param>
        /// <param name="newIndex">The index of the item that was added, changed, or removed.</param>
        /// <param name="newItem">The item that was added, changed, or removed.</param>
        public NotifyListChangedEventArgs(ListChangedType listChangedType, int newIndex, object newItem)
            : base(listChangedType, newIndex)
        {
            NewItem = newItem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of change.</param>
        /// <param name="newIndex">The index of the item that was added or changed.</param>
        /// <param name="propDesc">The <see cref="PropertyDescriptor"/> describing the item.</param>
        public NotifyListChangedEventArgs(ListChangedType listChangedType, int newIndex, PropertyDescriptor propDesc)
            : base(listChangedType, newIndex, propDesc)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of change.</param>
        /// <param name="newIndex">The index of the item that was added or changed.</param>
        /// <param name="newItem">The item that was added, changed, or removed.</param>
        /// <param name="propDesc">The <see cref="PropertyDescriptor"/> describing the item.</param>
        public NotifyListChangedEventArgs(ListChangedType listChangedType, int newIndex, object newItem, PropertyDescriptor propDesc)
            : base(listChangedType, newIndex, propDesc)
        {
            NewItem = newItem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of change.</param>
        /// <param name="newIndex">The new index of the item that was moved.</param>
        /// <param name="oldIndex">The old index of the item that was moved.</param>
        public NotifyListChangedEventArgs(ListChangedType listChangedType, int newIndex, int oldIndex)
            : base(listChangedType, newIndex, oldIndex)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of change.</param>
        /// <param name="newIndex">The new index of the item that was moved.</param>
        /// <param name="newItem">The new item that was moved.</param>
        /// <param name="oldIndex">The old index of the item that was moved.</param>
        /// <param name="oldItem">The old item that was moved.</param>
        public NotifyListChangedEventArgs(ListChangedType listChangedType, int newIndex, object newItem, int oldIndex, object oldItem)
            : base(listChangedType, newIndex, oldIndex)
        {
            NewItem = newItem;
            OldItem = oldItem;
        }

        /// <summary>
        /// The new item.
        /// </summary>
        public object NewItem { get; private set; }

        /// <summary>
        /// The old item.
        /// </summary>
        public object OldItem { get; private set; }
    }
}

#endif
