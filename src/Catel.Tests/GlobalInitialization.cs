using System.Diagnostics;
using System.Globalization;
using Catel.Logging;
using Catel.Reflection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

/// <summary>
/// Sets the current culture to <c>en-US</c> for all unit tests to prevent tests to fail
/// due to cultural string differences.
/// </summary>
[SetUpFixture]
public class GlobalInitialization
{
    [OneTimeSetUp]
    public static void SetUp()
    {
        LogManager.FallbackLoggerFactory = LoggerFactory.Create(x =>
        {
            if (Debugger.IsAttached)
            {
                x.AddFilter(x => x == LogLevel.Debug);

                x.AddDebug();
            }

            x.AddConsole();
        });

        //// For testing purposes, enable features we disabled for CTL-234
        //var modelEqualityComparer = ServiceLocator.Default.ResolveType<IModelEqualityComparer>();
        //modelEqualityComparer.CompareProperties = true;
        //modelEqualityComparer.CompareValues = true;
        //modelEqualityComparer.CompareCollections = true;

        var culture = new CultureInfo("en-US");
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

        // Required since we do multithreaded initialization
        TypeCache.InitializeTypes(allowMultithreadedInitialization: false);
    }
}
