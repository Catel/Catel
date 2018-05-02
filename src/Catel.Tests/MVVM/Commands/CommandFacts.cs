// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FastObservableCollectionFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.MVVM.Commands
{
    using System;

    using Catel.MVVM;

    using NUnit.Framework;

    public class CommandFacts
    {
        [TestFixture]
        public class TheExecuteCommand
        {
            [Test]
            public void ExecuteThrowsException()
            {
                var command = new Command(() => { throw new Exception(); }, () => true);
                ExceptionTester.CallMethodAndExpectException<Exception>(() => command.Execute());
            }

            [Test]
            public void CanExecuteThrowsException()
            {
                var command = new Command(() => { }, () => { throw new Exception(); });
                ExceptionTester.CallMethodAndExpectException<Exception>(() => command.Execute());
            }
        }
    }
}