// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskCommandTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET45

namespace Catel.Test.MVVM.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Catel.MVVM;

    using NUnit.Framework;

    [TestFixture]
    public class TaskCommandFacts
    {
        #region Constants
        private static readonly TimeSpan TaskDelay = TimeSpan.FromSeconds(9);
        #endregion

        #region Methods
        [TestCase]
        public async Task TestCommandCancellation()
        {
            var taskCommand = new TaskCommand(TestExecute);

            Assert.IsFalse(taskCommand.IsExecuting);
            Assert.IsFalse(taskCommand.IsCancellationRequested);

            taskCommand.Execute();

            Assert.IsTrue(taskCommand.IsExecuting);
            await Task.Delay(TimeSpan.FromSeconds(1));

            taskCommand.Cancel();

            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.IsFalse(taskCommand.IsExecuting);
            Assert.IsFalse(taskCommand.IsCancellationRequested);
        }

        private static async Task TestExecute(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Delay(TaskDelay, cancellationToken);

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