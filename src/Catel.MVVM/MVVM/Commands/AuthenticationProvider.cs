namespace Catel.MVVM
{
    using System.Windows;

    public class AuthenticationProvider : IAuthenticationProvider
    {
        public bool CanCommandBeExecuted(ICatelCommand command, object? commandParameter)
        {
            return true;
        }

        public bool HasAccessToUIElement(FrameworkElement element, object? tag, object? authenticationTag)
        {
            return true;
        }
    }
}
