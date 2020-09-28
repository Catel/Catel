namespace Catel.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class ShimDispatcherService : IDispatcherService
    {
        public void BeginInvoke(Action action, bool onlyBeginInvokeWhenNoAccess = true)
        {
            action();
        }

        public void Invoke(Action action, bool onlyInvokeWhenNoAccess = true)
        {
            action();
        }

        public Task InvokeAsync(Action action)
        {
            action();

            return Task.CompletedTask;
        }

        public Task InvokeAsync(Delegate method, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<T> InvokeAsync<T>(Func<T> func)
        {
            var result = func();
            return Task.FromResult(result);
        }

        public Task InvokeTaskAsync(Func<Task> actionAsync)
        {
            return actionAsync();
        }

        public Task InvokeTaskAsync(Func<CancellationToken, Task> actionAsync, CancellationToken cancellationToken)
        {
            return actionAsync(cancellationToken);
        }

        public Task<T> InvokeTaskAsync<T>(Func<Task<T>> funcAsync)
        {
            var task = funcAsync();
            return task;
        }

        public Task<T> InvokeTaskAsync<T>(Func<CancellationToken, Task<T>> funcAsync, CancellationToken cancellationToken)
        {
            var task = funcAsync(cancellationToken);
            return task;
        }
    }
}
