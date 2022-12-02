namespace Catel.Tests
{
    using System;
    using System.Diagnostics;
    using System.Runtime;

    public static class TimeMeasureHelper
    {
        public static double MeasureAction(int timesToInvoke, string description, Action action, Action? initializationAction = null)
        {
            Argument.IsNotNullOrWhitespace(() => description);
            ArgumentNullException.ThrowIfNull(action);

            if (initializationAction is not null)
            {
                initializationAction();
            }

            var oldMode = GCSettings.LatencyMode;
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            var totalMs = 0d;

            for (var i = 0; i < timesToInvoke; i++)
            {
                var innerStopwatch = new Stopwatch();
                innerStopwatch.Start();

                action();

                innerStopwatch.Stop();

                var elapsed = innerStopwatch.Elapsed.TotalMilliseconds;
                totalMs += elapsed;

                //ConsoleHelper.Write("{0} => run {1} took {2} ms", description, i + 1, elapsed);
            }

            GCSettings.LatencyMode = oldMode;

            var averageMs = (totalMs / timesToInvoke);

            ConsoleHelper.Write("{0}: {1}ms (average), {2}ms (total)", description, averageMs, totalMs);

            return averageMs;
        }
    }
}
