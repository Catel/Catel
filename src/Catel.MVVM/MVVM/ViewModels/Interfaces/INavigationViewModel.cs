namespace Catel.MVVM;

using System;
using Catel.MVVM.Navigation;

public interface INavigationViewModel : IViewModel
{ 
    event EventHandler? NavigationCompleted;

    /// <summary>
    /// Updates the navigation context. The navigation context provided by this class is different
    /// from the <see cref="NavigationContext"/>. Therefore, this method updates the navigation context
    /// to match it to the values of the <paramref name="navigationContext"/>.
    /// </summary>
    /// <param name="navigationContext">The navigation context.</param>
    public void UpdateNavigationContext(NavigationContext? navigationContext);
}
