// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUIElement.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using MVVM;

    /// <summary>
    /// Interface defining shared members for views.
    /// </summary>
    public interface IView : IUIElement, IViewModelContainer
    {
    }
}