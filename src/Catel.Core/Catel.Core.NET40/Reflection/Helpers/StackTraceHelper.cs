// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StackTraceHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    //using System;
    //using System.Collections.Generic;
    //using System.Diagnostics;
    //using System.Reflection;
    //using System.Reflection.Emit;
    //using System.Threading;

//    /// <summary>
//    /// Helper class for stack traces, for which the performance can be improved a lot for .NET 4 and .NET 4.5
//    /// </summary>
//    public static class StackTraceHelper
//    {
//#if NET
//        /// <summary>
//        /// The get stack trace method.
//        /// </summary>
//        private static readonly Func<object> GetStackFrameHelperMethod;
//#endif
        
//        /// <summary>
//        /// Initializes the static members of the <see cref="StackTraceHelper"/> class.
//        /// </summary>
//        static StackTraceHelper()
//        {
//#if NET
//            var stackFrameHelperType = typeof(object).Assembly.GetType("System.Diagnostics.StackFrameHelper");
//            var GetStackFramesInternal = Type.GetType("System.Diagnostics.StackTrace, mscorlib").GetMethod("GetStackFramesInternal", BindingFlags.Static | BindingFlags.NonPublic);

//            var method = new DynamicMethod("GetStackTraceFast", typeof(object), new Type[0], typeof(StackTrace), true);

//            var generator = method.GetILGenerator();
//            generator.DeclareLocal(stackFrameHelperType);
//            generator.Emit(OpCodes.Ldc_I4_0);
//            generator.Emit(OpCodes.Ldnull);
//            generator.Emit(OpCodes.Newobj, stackFrameHelperType.GetConstructor(new[] { typeof(bool), typeof(Thread) }));
//            generator.Emit(OpCodes.Stloc_0);
//            generator.Emit(OpCodes.Ldloc_0);
//            generator.Emit(OpCodes.Ldc_I4_0);
//            generator.Emit(OpCodes.Ldnull);
//            generator.Emit(OpCodes.Call, GetStackFramesInternal);
//            generator.Emit(OpCodes.Ldloc_0);
//            generator.Emit(OpCodes.Ret);

//            GetStackFrameHelperMethod = (Func<object>)method.CreateDelegate(typeof(Func<object>));
//#endif
//        }

//        #region Methods
//        /// <summary>
//        /// Returns the current stack trace. It is recommended to use this method because it is much faster than
//        /// the "regular" way to retrieve a stack trace.
//        /// </summary>
//        /// <returns>The <see cref="StackTrace"/>.</returns>
//        /// <remarks>
//        /// This code is originally found at http://ayende.com/blog/3879/reducing-the-cost-of-getting-a-stack-trace.
//        /// </remarks>
//        public static StackFrame[] GetStackTraceFrames()
//        {
//#if NET
//            //int numberOfFrames = stackFrameHelper.GetNumberOfFrames();
//            //skipFrames += StackTrace.CalculateFramesToSkip(sfh, numberOfFrames);
//            //if ((numberOfFrames - skipFrames) > 0)
//            //{
//            //    this.method = sfh.GetMethodBase(skipFrames);
//            //    this.offset = sfh.GetOffset(skipFrames);
//            //    this.ILOffset = sfh.GetILOffset(skipFrames);
//            //    if (fNeedFileInfo)
//            //    {
//            //        this.strFileName = sfh.GetFilename(skipFrames);
//            //        this.iLineNumber = sfh.GetLineNumber(skipFrames);
//            //        this.iColumnNumber = sfh.GetColumnNumber(skipFrames);
//            //    }
//            //}

//            var stackFrameHelper = GetStackFrameHelperMethod.Invoke();
//            var frames = new List<StackFrame>();
//            foreach (var frame in _)


//            return (StackTrace);
//#else
//            return new StackTrace().GetFrames();
//#endif
//        }
//        #endregion
//    }
}