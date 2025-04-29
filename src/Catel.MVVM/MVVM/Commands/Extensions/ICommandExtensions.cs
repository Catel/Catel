namespace Catel.MVVM
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    public static class ICommandExtensions
    {
        /// <summary>
        /// Gets the task of the command so it can be awaited.
        /// <para/>
        /// If the specified command does not support task-based execution,
        /// this method will return a <see cref="Task"/> that is already completed.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
#pragma warning disable CL0002 // Use "Async" suffix for async methods
        public static Task GetTask(this ICommand command)
#pragma warning restore CL0002 // Use "Async" suffix for async methods
        {
            if (command is ICatelTaskCommand taskCommand)
            {
                return taskCommand.Task;
            }

            // Nothing to return
            return Task.CompletedTask;
        }
    }
}
