namespace Catel.Tests;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catel.Threading;
using NUnit.Framework;

[TestFixture]
internal class AsyncLockFacts
{
    private readonly AsyncLock _asyncLock = new();

    private int _startedThreads;

    private readonly object _lock = new();

    private async Task MethodAsync()
    {
        var threadIndex = 0; 

        lock (_lock)
        {
            threadIndex = ++_startedThreads;
            Debug.WriteLine($"------started: {threadIndex}");
        }

        try
        {
            using (await _asyncLock.LockAsync())
            {
                Thread.Sleep(1);
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"ex: {exception}");

            throw;
        }
        finally
        {
            Debug.WriteLine($"finished: {threadIndex}");
        }
    }
    
    [Test]
    public async Task AsyncLockTaskTestFactsAsync()
    {
        Debug.Flush();

        var tasks = new List<Task>();
        for (var i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(MethodAsync));
        }

        Task.WaitAll(tasks.ToArray(), 2000);

        Assert.IsTrue(tasks.All(x => x.IsCompletedSuccessfully));
    }
}
