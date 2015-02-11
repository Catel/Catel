﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenServiceTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Services
{
    //using System;
    //using System.Threading;

    //using Catel.IoC;
    //using Catel.MVVM;
    //using Catel.Services;
    //using Catel.Services.Test;
    //using Catel.MVVM.Tasks;

    //using NUnit.Framework;

    //using Moq;

    //using UIVisualizerService = Catel.Services.UIVisualizerService;

    ///// <summary>
    ///// The splash screen service test.
    ///// </summary>
    //[TestFixture]
    //public class SplashScreenServiceTest
    //{
    //    #region Methods

    //    private static SplashScreenService CreateSplashScreenService(IMessageService messageService)
    //    {
    //        var dispatcherServiceMock = new Mock<IDispatcherService>();
    //        dispatcherServiceMock.Setup(service => service.Invoke(It.IsAny<Action>())).Callback((Action action) => action.Invoke());
    //        //ServiceLocator.Default.RegisterInstance<IDispatcherService>(dispatcherServiceMock.Object);

    //        return new SplashScreenService(dispatcherServiceMock.Object, messageService, new ViewModelFactory(TypeFactory.Default),
    //            TypeFactory.Default.CreateInstance<UIVisualizerService>(), TypeFactory.Default.CreateInstance<IPleaseWaitService>());
    //    }

    //    /// <summary>
    //    /// The progress notification through i please wait service.
    //    /// </summary>
    //    [TestCase]
    //    public void ProgressNotificationThroughIPleaseWaitService()
    //    {
    //        var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(1, 4, "Linking to satellite")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(2, 4, "Downloading original files from NASA servers")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(3, 4, "Replacing original files with fake ones")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(4, 4, "Closing satellite connections")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.Hide()).Verifiable();

    //        ServiceLocator.Default.RegisterInstance<IPleaseWaitService>(pleaseWaitServiceMock.Object);

    //        var splashScreenService = CreateSplashScreenService(new Catel.Services.Test.MessageService());
    //        splashScreenService.Enqueue(new FooTask("Linking to satellite"));
    //        splashScreenService.Enqueue(new FooTask("Downloading original files from NASA servers"));
    //        splashScreenService.Enqueue(new FooTask("Replacing original files with fake ones"));
    //        splashScreenService.Enqueue(new FooTask("Closing satellite connections"));

    //        splashScreenService.Commit();

    //        Assert.IsFalse(splashScreenService.IsRunning);
    //        pleaseWaitServiceMock.VerifyAll();
    //    }

    //    /// <summary>
    //    /// The invalid operation exception is raised if there are not task registered.
    //    /// </summary>
    //    [TestCase]
    //    public void InvalidOperationExceptionIsRaisedIfThereAreNotTaskRegisteredCommiting()
    //    {
    //        var splashScreenService = CreateSplashScreenService(new Catel.Services.Test.MessageService());
    //        ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => splashScreenService.Commit());
    //    }

    //    /// <summary>
    //    /// The invalid operation exception is raised if there are not task registered.
    //    /// </summary>
    //    [TestCase]
    //    public void InvalidOperationExceptionIsRaisedIfThereAreNotTaskRegisteredWhenCommitingAsycn()
    //    {
    //        var splashScreenService = CreateSplashScreenService(new Catel.Services.Test.MessageService());
    //        ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => splashScreenService.CommitAsync());
    //    }

    //    /// <summary>
    //    /// The invalid operation exception is raised if is commited during an execution.
    //    /// </summary>
    //    [TestCase]
    //    public void InvalidOperationExceptionIsRaisedIfIsCommitedWhenIsRunning()
    //    {
    //        var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(1, 4, "Linking to satellite")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(2, 4, "Downloading original files from NASA servers")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(3, 4, "Replacing original files with fake ones")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(4, 4, "Closing satellite connections")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.Hide()).Verifiable();

    //        ServiceLocator.Default.RegisterInstance<IPleaseWaitService>(pleaseWaitServiceMock.Object);

    //        var splashScreenService = CreateSplashScreenService(new Catel.Services.Test.MessageService());
    //        splashScreenService.Enqueue(new FooTask("Linking to satellite"));
    //        splashScreenService.Enqueue(new FooTask("Downloading original files from NASA servers"));
    //        splashScreenService.Enqueue(new FooTask("Replacing original files with fake ones"));
    //        splashScreenService.Enqueue(new FooTask("Closing satellite connections"));

    //        var @event = new AutoResetEvent(false);
    //        splashScreenService.CommitAsync(() => @event.Set());

    //        // Wait for running is state now is commiting
    //        while (!splashScreenService.IsRunning)
    //        {
    //            ThreadHelper.Sleep(10);
    //        }

    //        ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => splashScreenService.Commit());
    //        @event.WaitOne();
    //        Assert.IsFalse(splashScreenService.IsRunning);
    //        pleaseWaitServiceMock.VerifyAll();
    //    }

    //    /// <summary>
    //    /// The invalid operation exception is raised if is commited async when is running.
    //    /// </summary>
    //    [TestCase]
    //    public void InvalidOperationExceptionIsRaisedIfIsCommitedAsyncWhenIsRunning()
    //    {
    //        var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(1, 4, "Linking to satellite")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(2, 4, "Downloading original files from NASA servers")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(3, 4, "Replacing original files with fake ones")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(4, 4, "Closing satellite connections")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.Hide()).Verifiable();

    //        ServiceLocator.Default.RegisterInstance<IPleaseWaitService>(pleaseWaitServiceMock.Object);

    //        var splashScreenService = CreateSplashScreenService(new Catel.Services.Test.MessageService());
    //        splashScreenService.Enqueue(new FooTask("Linking to satellite"));
    //        splashScreenService.Enqueue(new FooTask("Downloading original files from NASA servers"));
    //        splashScreenService.Enqueue(new FooTask("Replacing original files with fake ones"));
    //        splashScreenService.Enqueue(new FooTask("Closing satellite connections"));

    //        var @event = new AutoResetEvent(false);
    //        splashScreenService.CommitAsync(() => @event.Set());

    //        // Wait for running is state now is commiting
    //        while (!splashScreenService.IsRunning)
    //        {
    //            ThreadHelper.Sleep(10);
    //        }

    //        ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => splashScreenService.CommitAsync());
    //        @event.WaitOne();
    //        Assert.IsFalse(splashScreenService.IsRunning);
    //        pleaseWaitServiceMock.VerifyAll();
    //    }

    //    /// <summary>
    //    /// The invalid operation exception is raised if is enqueue during an execution.
    //    /// </summary>
    //    [TestCase]
    //    public void InvalidOperationExceptionIsRaisedIfTaskIsEnqueueWhenIsRunning()
    //    {
    //        var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(1, 4, "Linking to satellite")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(2, 4, "Downloading original files from NASA servers")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(3, 4, "Replacing original files with fake ones")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(4, 4, "Closing satellite connections")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.Hide()).Verifiable();

    //        ServiceLocator.Default.RegisterInstance<IPleaseWaitService>(pleaseWaitServiceMock.Object);

    //        var splashScreenService = CreateSplashScreenService(new Catel.Services.Test.MessageService());
    //        splashScreenService.Enqueue(new FooTask("Linking to satellite"));
    //        splashScreenService.Enqueue(new FooTask("Downloading original files from NASA servers"));
    //        splashScreenService.Enqueue(new FooTask("Replacing original files with fake ones"));
    //        splashScreenService.Enqueue(new FooTask("Closing satellite connections"));

    //        var @event = new AutoResetEvent(false);
    //        splashScreenService.CommitAsync(() => @event.Set());

    //        // Wait for running is state now is commiting
    //        while (!splashScreenService.IsRunning)
    //        {
    //            ThreadHelper.Sleep(10);
    //        }

    //        ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => splashScreenService.Enqueue(new FooTask("The forgoten task")));
    //        @event.WaitOne();
    //        Assert.IsFalse(splashScreenService.IsRunning);
    //        pleaseWaitServiceMock.VerifyAll();
    //    }

    //    /// <summary>
    //    /// The invalid operation exception is raised if task is enqueue when is commiting.
    //    /// </summary>
    //    [TestCase]
    //    public void InvalidOperationExceptionIsRaisedIfTaskIsEnqueueWhenIsCommiting()
    //    {
    //        var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(1, 4, "Linking to satellite")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(2, 4, "Downloading original files from NASA servers")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(3, 4, "Replacing original files with fake ones")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.UpdateStatus(4, 4, "Closing satellite connections")).Verifiable();
    //        pleaseWaitServiceMock.Setup(service => service.Hide()).Verifiable();

    //        ServiceLocator.Default.RegisterInstance<IPleaseWaitService>(pleaseWaitServiceMock.Object);

    //        var splashScreenService = CreateSplashScreenService(new Catel.Services.Test.MessageService());
    //        splashScreenService.Enqueue(new FooTask("Linking to satellite"));
    //        splashScreenService.Enqueue(new FooTask("Downloading original files from NASA servers"));
    //        splashScreenService.Enqueue(new FooTask("Replacing original files with fake ones"));
    //        splashScreenService.Enqueue(new FooTask("Closing satellite connections"));

    //        var @event = new AutoResetEvent(false);
    //        splashScreenService.CommitAsync(() => @event.Set());
    //        Assert.IsFalse(splashScreenService.IsRunning);
    //        ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => splashScreenService.Enqueue(new FooTask("The forgoten task")));
    //        @event.WaitOne();
    //        Assert.IsFalse(splashScreenService.IsRunning);
    //        pleaseWaitServiceMock.VerifyAll();
    //    }

    //    /// <summary>
    //    /// The interation throught i message service and notify progress of execution and rollback throught i please wait service.
    //    /// </summary>
    //    [TestCase]
    //    public void InterationThroughtIMessageServiceAndNotifyProgressOfExecutionAndRollbackThroughtIPleaseWaitService()
    //    {
    //        var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
    //        var messageServiceMock = new Mock<IMessageService>();

    //        messageServiceMock.Setup(service => service.Show(It.IsRegex("Replacing original files with fake ones"), "Error", MessageButton.YesNoCancel, MessageImage.Error)).Returns(MessageResult.Cancel).Verifiable();

    //        ServiceLocator.Default.RegisterInstance<IPleaseWaitService>(pleaseWaitServiceMock.Object);

    //        var splashScreenService = CreateSplashScreenService(messageServiceMock.Object);
    //        splashScreenService.Enqueue(new FooTask("Linking to satellite"));
    //        splashScreenService.Enqueue(new FooTask("Downloading original files from NASA servers"));
    //        splashScreenService.Enqueue(new FooErrorTask("Replacing original files with fake ones"));
    //        splashScreenService.Enqueue(new FooTask("Closing satellite connections"));

    //        splashScreenService.Commit();

    //        pleaseWaitServiceMock.Verify(service => service.UpdateStatus(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.AtMost(6));
    //        pleaseWaitServiceMock.Verify(service => service.UpdateStatus(It.IsAny<int>(), It.IsAny<int>(), It.IsRegex("Rollback.+")), Times.AtMost(3));
    //        pleaseWaitServiceMock.Verify(service => service.UpdateStatus(It.IsAny<int>(), It.IsAny<int>(), It.IsRegex("Closing satellite connections")), Times.Never());

    //        Assert.IsFalse(splashScreenService.IsRunning);
    //        pleaseWaitServiceMock.VerifyAll();
    //    }

    //    #endregion

    //    #region Nested type: FooErrorTask

    //    /// <summary>
    //    /// The foo task.
    //    /// </summary>
    //    public class FooErrorTask : TaskBase
    //    {
    //        #region Constructors

    //        /// <summary>
    //        /// Initializes a new instance of the <see cref="FooErrorTask"/> class. 
    //        /// </summary>
    //        /// <param name="name">
    //        /// The name.
    //        /// </param>
    //        public FooErrorTask(string name)
    //            : base(name)
    //        {
    //        }

    //        #endregion

    //        #region Methods

    //        /// <summary>
    //        /// The execute.
    //        /// </summary>
    //        public override void Execute()
    //        {
    //            throw new Exception("Foo Error");
    //        }

    //        /// <summary>
    //        /// The rollback.
    //        /// </summary>
    //        public override void Rollback()
    //        {
    //            throw new Exception("Foo Error");
    //        }

    //        #endregion
    //    }
    //    #endregion

    //    #region Nested type: FooTask

    //    /// <summary>
    //    /// The foo task.
    //    /// </summary>
    //    public class FooTask : TaskBase
    //    {
    //        #region Constructors

    //        /// <summary>
    //        /// Initializes a new instance of the <see cref="FooTask"/> class.
    //        /// </summary>
    //        /// <param name="name">
    //        /// The name.
    //        /// </param>
    //        public FooTask(string name)
    //            : base(name)
    //        {
    //        }

    //        #endregion

    //        #region Methods

    //        /// <summary>
    //        /// The execute.
    //        /// </summary>
    //        public override void Execute()
    //        {
    //            ThreadHelper.Sleep(1000);
    //        }

    //        #endregion
    //    }
    //    #endregion
    //}
}