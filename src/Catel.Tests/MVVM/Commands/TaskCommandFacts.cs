namespace Catel.Tests.MVVM.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;

using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

[TestFixture]
public class TaskCommandFacts
{
    private static readonly TimeSpan TaskDelay = TimeSpan.FromSeconds(5);

    [TestCase]
    public void Test_Command_Cancellation()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var taskCommand = new TaskCommand(serviceProvider, TestExecuteAsync);

        Assert.That(taskCommand.IsExecuting, Is.False);
        Assert.That(taskCommand.IsCancellationRequested, Is.False);

        taskCommand.Execute();

        Assert.That(taskCommand.IsExecuting, Is.True);

        ThreadHelper.Sleep(1000);

        taskCommand.Cancel();

        ThreadHelper.Sleep(1000);

        Assert.That(taskCommand.IsExecuting, Is.False);
        Assert.That(taskCommand.IsCancellationRequested, Is.False);
    }

    [TestCase]
    public async Task Test_Command_Exceptions_Swallow_Exceptions()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var taskCommand = new TaskCommand(serviceProvider, TestExecuteWithExceptionAsync)
        {
            SwallowExceptions = true
        };

        Assert.That(taskCommand.IsExecuting, Is.False);
        Assert.That(taskCommand.IsCancellationRequested, Is.False);

        try
        {
            taskCommand.Execute();

            Assert.That(taskCommand.IsExecuting, Is.True);

            await taskCommand.Task;
        }
        catch (Exception ex)
        {
            Assert.Fail($"No exception expected, should be swallowed, but got '{ex}'");
        }

        Assert.That(taskCommand.IsExecuting, Is.False);
    }

    [TestCase, Explicit]
    public async Task Test_Command_Exceptions_Dont_Swallow_Exceptions()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var taskCommand = new TaskCommand(serviceProvider, TestExecuteWithExceptionAsync)
        {
            SwallowExceptions = false
        };

        Assert.That(taskCommand.IsExecuting, Is.False);
        Assert.That(taskCommand.IsCancellationRequested, Is.False);

        try
        {
            taskCommand.Execute();

            await taskCommand.Task;

            Assert.Fail("Expected exception");
        }
        catch (Exception)
        {
        }

        Assert.That(taskCommand.IsExecuting, Is.False, "Command should not be executing");
    }

    //[TestCase]
    //public async Task TestCommandExceptions_DontSwallowExceptionsWithoutAwaitAsync()
    //{
    //    var taskCommand = new TaskCommand(TestExecuteWithExceptionAsync)
    //    {
    //        SwallowExceptions = false
    //    };

    //    Assert.IsFalse(taskCommand.IsExecuting);
    //    Assert.IsFalse(taskCommand.IsCancellationRequested);

    //    try
    //    {
    //        taskCommand.Execute();

    //        await Task.Delay(1500);

    //        Assert.Fail("Expected exception");
    //    }
    //    catch (Exception)
    //    {
    //    }

    //    Assert.IsFalse(taskCommand.IsExecuting, "Command should not be executing");
    //}

    private static async Task TestExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.Delay(TaskDelay, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
    }

    private static async Task TestExecuteWithExceptionAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.Delay(500, cancellationToken);

        throw new Exception("This is an expected exception");
    }
}

public class PercentProgress : ITaskProgressReport
{
    public PercentProgress(int percentage, string status = "")
    {
        Percentage = percentage;
        Status = status;
    }

    public int Percentage { get; private set; }

    public string Status { get; private set; }
}
