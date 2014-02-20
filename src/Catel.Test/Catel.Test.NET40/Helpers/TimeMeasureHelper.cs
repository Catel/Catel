// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeMeasureHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test
{
    using System;
    using System.Diagnostics;

    public static class TimeMeasureHelper
    {
        public static double MeasureAction(int timesToInvoke, string description, Action action, Action initializationAction = null)
        {
            Argument.IsNotNullOrWhitespace(() => description);
            Argument.IsNotNull(() => action);

            if (initializationAction != null)
            {
                initializationAction();
            }

            double totalMs = 0d;

            for (int i = 0; i < timesToInvoke; i++)
            {
#if !SILVERLIGHT
                var innerStopwatch = new Stopwatch();
                innerStopwatch.Start();
#endif

                action();

#if !SILVERLIGHT
                innerStopwatch.Stop();

                var elapsed = innerStopwatch.Elapsed.TotalMilliseconds;
                totalMs += elapsed;

                ConsoleHelper.Write("{0} => run {1} took {2} ms", description, i + 1, elapsed);
#endif
            }

            var averageMs = (totalMs / timesToInvoke);

            ConsoleHelper.Write("{0}: {1}ms (average), {2}ms (total)", description, averageMs, totalMs);

            return averageMs;
        }
    }
}