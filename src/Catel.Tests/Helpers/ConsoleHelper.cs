namespace Catel.Tests
{
    public static class ConsoleHelper
    {
        public static void Write(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }
    }
}
