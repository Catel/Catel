using ApprovalTests.Reporters;

#if DEBUG
[assembly: UseReporter(typeof(BeyondCompareReporter))]
#else
[assembly: UseReporter(typeof(DiffReporter))]
#endif

