// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskCommandTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.MVVM.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Catel.MVVM;
    using Catel.Threading;

    using NUnit.Framework;

    [TestFixture]
    public class TaskCommandFacts
    {
        #region Constants
        private static readonly TimeSpan TaskDelay = TimeSpan.FromSeconds(9);
        #endregion

        #region Methods
        [TestCase]
        public void TestCommandCancellation()
        {
            var taskCommand = new TaskCommand(TestExecute);

            Assert.IsFalse(taskCommand.IsExecuting);
            Assert.IsFalse(taskCommand.IsCancellationRequested);

            taskCommand.Execute();

            Assert.IsTrue(taskCommand.IsExecuting);

            ThreadHelper.Sleep(1000);

            taskCommand.Cancel();

            ThreadHelper.Sleep(1000);

            Assert.IsFalse(taskCommand.IsExecuting);
            Assert.IsFalse(taskCommand.IsCancellationRequested);
        }

        private static async Task TestExecute(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

#if NET40
            await TaskEx.Delay(TaskDelay, cancellationToken);
#else
            await Task.Delay(TaskDelay, cancellationToken);
#endif

            cancellationToken.ThrowIfCancellationRequested();
        }
        #endregion
    }

    public class PercentProgress : ITaskProgressReport
    {
        #region Constructors
        public PercentProgress(int percents, string status = null)
        {
            Percents = percents;
            Status = status;
        }
        #endregion

        #region Properties
        public int Percents { get; private set; }
        #endregion

        #region ITaskProgressReport Members
        public string Status { get; private set; }
        #endregion
    }
}

#endif