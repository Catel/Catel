namespace Catel.MVVM.Converters
{
    public interface IEventArgsConverter
    {
        object? Convert(object? sender, object? args);
    }
}
