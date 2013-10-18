// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeMeasureHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test
{
    using System;
    using System.Diagnostics;

    public static class TimeMeasureHelper
    {
        public static void MeasureAction(int timesToInvoke, string description, Action action, Action initializationAction = null)
        {
            Argument.IsNotNullOrWhitespace(() => description);
            Argument.IsNotNull(() => action);

            if (initializationAction != null)
            {
                initializationAction();
            }

#if !SILVERLIGHT
            var stopwatch = new Stopwatch();
            stopwatch.Start();
#endif

            for (int i = 0; i < timesToInvoke; i++)
            {
#if !SILVERLIGHT
                var innerStopwatch = new Stopwatch();
                innerStopwatch.Start();
#endif

                action();

#if !SILVERLIGHT
                innerStopwatch.Stop();

                ConsoleHelper.Write("{0} => run {1} took {2} ms", description, i + 1, innerStopwatch.Elapsed.TotalMilliseconds);
#endif
            }

#if !SILVERLIGHT
            stopwatch.Stop();

            ConsoleHelper.Write("{0}: {1}ms (average)", description, (stopwatch.Elapsed.TotalMilliseconds / 5));
#endif
        }
    }
}