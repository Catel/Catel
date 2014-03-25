// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserControlBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Providers
{
    using System;
    using Catel.Logging;
    using Catel.MVVM.Views;
    using Catel.Reflection;
    using Catel.Windows.Interactivity;
    using MVVM;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using ControlType = global::Windows.UI.Xaml.Controls.UserControl;
#elif NET
    using ControlType = System.Windows.Controls.ContentControl;
    using System.Windows.Interactivity;
#else
    using ControlType = System.Windows.Controls.UserControl;
    using System.Windows.Interactivity;
#endif

    /// <summary>
    /// A <see cref="Behavior{T}"/> implementation for a user control. 
    /// </summary>
    public class UserControlBehavior : MVVMBehaviorBase<ControlType, UserControlLogic>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets a value indicating whether the user control should close any existing
        /// view model when the control is unloaded from the visual tree.
        /// <para />
        /// Set this property to <c>false</c> if a view model should be kept alive and re-used
        /// for unloading/loading instead of creating a new one.
        /// <para />
        /// By default, this value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the view model should be closed when the control is unloaded; otherwise, <c>false</c>.
        /// </value>
        public bool CloseViewModelOnUnloaded
        {
            get { return Logic.CloseViewModelOnUnloaded; }
            set { Logic.CloseViewModelOnUnloaded = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether parent view model containers are supported. If supported,
        /// the user control will search for a <see cref="DependencyObject"/> that implements the <see cref="IViewModelContainer"/>
        /// interface. During this search, the user control will use both the visual and logical tree.
        /// <para />
        /// If a user control does not have any parent control implementing the <see cref="IViewModelContainer"/> interface, searching
        /// for it is useless and requires the control to search all the way to the top for the implementation. To prevent this from
        /// happening, set this property to <c>false</c>.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if parent view model containers are supported; otherwise, <c>false</c>.
        /// </value>
        public bool SupportParentViewModelContainers
        {
            get { return Logic.SupportParentViewModelContainers; }
            set { Logic.SupportParentViewModelContainers = value; }
        }

#if NET || SL5
        /// <summary>
        /// Gets or sets a value indicating whether to skip the search for an info bar message control. If not skipped,
        /// the user control will search for a the first <c>InfoBarMessageControl</c> that can be found. 
        /// During this search, the user control will use both the visual and logical tree.
        /// <para />
        /// If a user control does not have any <c>InfoBarMessageControl</c>, searching
        /// for it is useless and requires the control to search all the way to the top for the implementation. To prevent this from
        /// happening, set this property to <c>true</c>.
        /// <para />
        /// The default value is determined by the <see cref="DefaultSkipSearchingForInfoBarMessageControlValue"/> property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the search for an info bar message control should be skipped; otherwise, <c>false</c>.
        /// </value>
        public bool SkipSearchingForInfoBarMessageControl
        {
            get { return Logic.SkipSearchingForInfoBarMessageControl; }
            set { Logic.SkipSearchingForInfoBarMessageControl = value; }
        }

        /// <summary>
        /// Gets or sets a value for the <see cref="SkipSearchingForInfoBarMessageControl"/> property. This way, the behavior
        /// can be changed an entire application to prevent disabling it on every control.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value>The default value.</value>
        /// <remarks>
        /// Internally this value uses the <see cref="UserControlLogic.DefaultSkipSearchingForInfoBarMessageControlValue"/> property.
        /// </remarks>
        public static bool DefaultSkipSearchingForInfoBarMessageControlValue
        {
            get { return UserControlLogic.DefaultSkipSearchingForInfoBarMessageControlValue; }
            set { UserControlLogic.DefaultSkipSearchingForInfoBarMessageControlValue = value; }
        }
#endif

        /// <summary>
        /// Gets or sets a value indicating whether the user control should automatically be disabled when there is no
        /// active view model.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the user control should automatically be disabled when there is no active view model; otherwise, <c>false</c>.
        /// </value>
        public bool DisableWhenNoViewModel
        {
            get { return Logic.DisableWhenNoViewModel; }
            set { Logic.DisableWhenNoViewModel = value; }
        }

        /// <summary>
        /// Creates the logic required for MVVM.
        /// </summary>
        /// <returns>The <see cref="LogicBase"/> implementation uses by this behavior.</returns>
        protected override UserControlLogic CreateLogic()
        {
            var associatedObjectType = AssociatedObject.GetType();
            if (!associatedObjectType.ImplementsInterfaceEx<IView>())
            {
                string error = string.Format("Type '{0}' does not implement IView, make sure to implement the interface correctly", associatedObjectType);
                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            return new UserControlLogic((IView)AssociatedObject, ViewModelType);
        }
    }
}
