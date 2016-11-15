// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FastDateTime.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Fast implementation of the date/time retrieval.
    /// </summary>
    public static class FastDateTime
    {
#if !SILVERLIGHT		
        private static DateTime UtcStartTime;
        private static DateTime StartTime;
		
        private static readonly Stopwatch Stopwatch;

        static FastDateTime()
        {		
            var stopwatch = Stopwatch.StartNew();
            var now = DateTime.Now;

            Stopwatch = stopwatch;
            StartTime = now;
            UtcStartTime = now.ToUniversalTime();
        }
#endif

        /// <summary>
        /// Gets the current date/time.
        /// </summary>
        /// <value>
        /// The current date/time.
        /// </value>
        public static DateTime Now
        {
            get
            {
#if !SILVERLIGHT				
                var final = GetCurrent(ref StartTime);
                return final;
#else
				return DateTime.Now;
#endif
            }
        }

        /// <summary>
        /// Gets the current date/time in UTC.
        /// </summary>
        /// <value>
        /// The current date/time in UTC.
        /// </value>
        public static DateTime UtcNow
        {
            get
            {
#if !SILVERLIGHT					
                // Returning DateTime.Utc is faster, see Catel.Benchmarks
                return DateTime.UtcNow;

                //var final = GetCurrent(ref UtcStartTime);
                //return final;
#else
				return DateTime.UtcNow;
#endif				
            }
        }

#if !SILVERLIGHT			
        private static DateTime GetCurrent(ref DateTime startTime)
        {
            var elapsed = Stopwatch.ElapsedMilliseconds;

            var final = startTime.AddMilliseconds(elapsed);
            return final;
        }
#endif
    }
}