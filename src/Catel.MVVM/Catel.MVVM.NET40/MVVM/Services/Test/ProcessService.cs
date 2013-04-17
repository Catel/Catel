// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services.Test
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Class representing the process result.
    /// </summary>
    public class ProcessServiceTestResult
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessServiceTestResult"/> class, with <c>0</c> as default process result code.
        /// </summary>
        /// <param name="result">if set to <c>true</c>, the process will succeed during the test.</param>
        public ProcessServiceTestResult(bool result)
            : this(result, 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessServiceTestResult"/> class.
        /// </summary>
        /// <param name="result">if set to <c>true</c>, the process will succeed during the test.</param>
        /// <param name="processResultCode">The process result code to return in case of a callback.</param>
        public ProcessServiceTestResult(bool result, int processResultCode)
        {
            Result = result;
            ProcessResultCode = processResultCode;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the process should be returned as successfull when running the process.
        /// </summary>
        /// <value><c>true</c> if the process should be returned as successfull; otherwise, <c>false</c>.</value>
        public bool Result { get; private set; }

        /// <summary>
        /// Gets or sets the process result code.
        /// </summary>
        /// <value>The process result code.</value>
        public int ProcessResultCode { get; private set; }
        #endregion
    }

    /// <summary>
    /// Test implementation of the <see cref="IProcessService"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// 
    /// Test.ProcessService service = (Test.ProcessService)GetService<IProcessService>();
    /// 
    /// // Queue the next expected result (next StartProcess will succeed to run app, 5 will be returned as exit code)
    /// service.ExpectedResults.Add(new ProcessServiceTestResult(true, 5));
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public class ProcessService : IProcessService
    {
        #region Properties
        /// <summary>
        /// Gets the queue of expected results for the <see cref="StartProcess(string,string,Catel.MVVM.Services.ProcessCompletedDelegate)"/> method.
        /// </summary>
        /// <value>The expected results.</value>
        public Queue<ProcessServiceTestResult> ExpectedResults { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Starts a process resource by specifying the name of an application and a set of command-line arguments.
        /// </summary>
        /// <param name="fileName">The name of an application file to run in the process.</param>
        /// <param name="arguments">Command-line arguments to pass when starting the process.</param>
        /// <param name="processCompletedCallback">The process completed callback, invoked only when the process is started successfully and completed.</param>
        /// <exception cref="ArgumentException">The <paramref name="fileName"/> is <c>null</c> or whitespace.</exception>
        public void StartProcess(string fileName, string arguments = "", ProcessCompletedDelegate processCompletedCallback = null)
        {
            Argument.IsNotNullOrWhitespace("fileName", fileName);

            if (ExpectedResults.Count == 0)
            {
                throw new Exception(ResourceHelper.GetString(typeof(ProcessService), "Exceptions", "NoExpectedResultsInQueueForUnitTest"));
            }

            var result = ExpectedResults.Dequeue();
            if (result.Result)
            {
                if (processCompletedCallback != null)
                {
                    processCompletedCallback(result.ProcessResultCode);
                }
            }
        }
        #endregion
    }
}
