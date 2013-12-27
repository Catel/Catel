using System.Windows.Forms;
using Catel.IoC;

namespace Catel.MVVM.Commands
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Windows.Controls;
    using Views.Interfaces;

    class ViewRoutedCommandManager : IViewRoutedCommandManager
    {
        public void RegisterView(IView view)
        {
            var uiElement = view as UIElement;
            if (uiElement != null)
            {
                System.Windows.Input.CommandManager.AddCanExecuteHandler(uiElement, CanExecuteHandler);
                System.Windows.Input.CommandManager.AddExecutedHandler(uiElement, ExecutedHandler);
                //System.Windows.Input.CommandManager.AddPreviewCanExecuteHandler(uiElement, PreviewCanExecuteHandler);
                //System.Windows.Input.CommandManager.AddPreviewExecutedHandler(uiElement, PreviewExecutedHandler);
            }
        }
        private void CanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            var command = GetCommandBindingFromRoutedCommand(sender, e.Command);

            if (command != null)
            {
                e.CanExecute = e.Handled = command.CanExecute(e);
            }
        }

        private void ExecutedHandler(object sender, ExecutedRoutedEventArgs e)
        {
            var command = GetCommandBindingFromRoutedCommand(sender, e.Command);

            if (command != null)
            {
                e.Handled = true;
                command.Execute(e);
            }
        }

        //private void PreviewCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    var command = GetCommandBindingFromRoutedCommand(sender, e.Command);

        //    if (command != null)
        //    {
        //        e.CanExecute = e.Handled = command.CanExecute(e);
        //    }
        //}

        //private void PreviewExecutedHandler(object sender, ExecutedRoutedEventArgs e)
        //{
        //    var command = GetCommandBindingFromRoutedCommand(sender, e.Command);

        //    if (command != null)
        //    {
        //        e.Handled = true;
        //        command.Execute(e);
        //    }
        //}

        private ICommand GetCommandBindingFromRoutedCommand(object sender, ICommand command)
        {
            var view = sender as IView;
            IViewModel viewModel = null;

            if (view != null)
            {
                viewModel = view.ViewModel;
            }

            if (viewModel != null)
            {
                var viewModelCommandManager = viewModel.ViewModelCommandManager;
                try
                {
                    return viewModelCommandManager.GetCommandForRoutedCommand(command);
                }
                catch (Exception)
                {
                    return null; 
                }
            }

            return null;
        }

        public void UnregisterView(IView view)
        {
            var uiElement = view as UIElement;
            if (uiElement != null)
            {
                System.Windows.Input.CommandManager.RemoveCanExecuteHandler(uiElement, CanExecuteHandler);
                System.Windows.Input.CommandManager.RemoveExecutedHandler(uiElement, ExecutedHandler);
                //System.Windows.Input.CommandManager.RemovePreviewCanExecuteHandler(uiElement, PreviewCanExecuteHandler);
                //System.Windows.Input.CommandManager.RemovePreviewExecutedHandler(uiElement, PreviewExecutedHandler);
            }
        }
    }
}
