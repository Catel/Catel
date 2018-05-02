// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentPage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System;
    using System.Threading.Tasks;

    using Catel.Logging;
    using global::Xamarin.Forms;

    /// <summary>
    ///     Is a relative layout that allow show view as modal.
    /// </summary>
    [ContentProperty("Content")]
    public class PopupLayout : RelativeLayout
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private RelativeLayout _backdrop;

        /// <summary>
        ///     The content
        /// </summary>
        private View _content;

        /// <summary>
        ///     The popup view
        /// </summary>
        private View _popupView;

        /// <summary>
        ///     The content view.
        /// </summary>
        public View Content
        {
            get { return _content; }

            set
            {
                if (_content != null)
                {
                    Children.Remove(_content);
                }

                _content = value;
                Children.Add(_content, () => Bounds);
            }
        }

        /// <summary>
        /// </summary>
        public bool IsPopupActive => _popupView != null;

        /// <summary>
        ///     Show a view as
        /// </summary>
        /// <param name="view">The view to be displayed as popup</param>
        /// <returns>
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="view" /> is <c>null</c>.</exception>
        public async Task<bool> ShowPopupAsync(View view)
        {
            Argument.IsNotNull(() => view);

            await DismissPopupAsync();

            _popupView = view;
            _content.InputTransparent = true;
            var backdrop = new RelativeLayout
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.4),
                Opacity = 1,
                GestureRecognizers =
                {
                    new TapGestureRecognizer()
                }
            };

            if (_popupView.BackgroundColor == Color.Default)
            {
                _popupView.BackgroundColor = Color.FromRgb(40, 40, 40);
            }

            if (_popupView.WidthRequest < 0)
            {
                _popupView.WidthRequest = Width;
            }

            if (_popupView.HeightRequest < 0)
            {
                _popupView.HeightRequest = Height;
            }

            backdrop.Children.Add(_popupView,
                Constraint.RelativeToParent(p => Width/2 - _popupView.WidthRequest/2),
                Constraint.RelativeToParent(p => Height/2 - _popupView.HeightRequest/2),
                Constraint.RelativeToParent(p => _popupView.WidthRequest),
                Constraint.RelativeToParent(p => _popupView.HeightRequest));

            _backdrop = backdrop;

            Children.Add(backdrop, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));

            UpdateChildrenLayout();

            return await _backdrop.FadeTo(1);
        }

        /// <summary>
        ///     Dismiss popup.
        /// </summary>
        /// <returns></returns>
        public async Task DismissPopupAsync()
        {
            try
            {
                if (_popupView != null)
                {
                    await Task.WhenAll(_popupView.FadeTo(0), _backdrop.FadeTo(0));

                    if (_backdrop.Children.Contains(_popupView))
                    {
                        _backdrop.Children.Remove(_popupView);
                    }

                    if (Children.Contains(_backdrop))
                    {
                        Children.Remove(_backdrop);
                    }

                    _popupView = null;
                }

                _content.InputTransparent = false;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}