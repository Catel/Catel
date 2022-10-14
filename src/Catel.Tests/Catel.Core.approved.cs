[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Catel.MVVM")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Catel.Serialization.Json")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Catel.Tests")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v6.0", FrameworkDisplayName="")]
namespace Catel
{
    public static class Argument
    {
        public static void ImplementsInterface(string paramName, object instance, System.Type interfaceType) { }
        public static void ImplementsInterface(string paramName, System.Type type, System.Type interfaceType) { }
        public static void ImplementsInterface<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, System.Type interfaceType)
            where T :  class { }
        public static void ImplementsInterface<TInterface>(string paramName, object instance)
            where TInterface :  class { }
        public static void ImplementsOneOfTheInterfaces(string paramName, object instance, System.Type[] interfaceTypes) { }
        public static void ImplementsOneOfTheInterfaces(string paramName, System.Type type, System.Type[] interfaceTypes) { }
        public static void ImplementsOneOfTheInterfaces<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, System.Type[] interfaceTypes)
            where T :  class { }
        public static void InheritsFrom(string paramName, object instance, System.Type baseType) { }
        public static void InheritsFrom(string paramName, System.Type type, System.Type baseType) { }
        public static void InheritsFrom<TBase>(string paramName, object instance)
            where TBase :  class { }
        public static void IsMatch(System.Linq.Expressions.Expression<System.Func<string>> expression, string pattern, System.Text.RegularExpressions.RegexOptions regexOptions = 0) { }
        public static void IsMatch(string paramName, string paramValue, string pattern, System.Text.RegularExpressions.RegexOptions regexOptions = 0) { }
        public static void IsMaximum<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, T maximumValue)
            where T : System.IComparable { }
        public static void IsMaximum<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, T maximumValue, System.Func<T, T, bool> validation) { }
        public static void IsMaximum<T>(string paramName, T paramValue, T maximumValue)
            where T : System.IComparable { }
        public static void IsMaximum<T>(string paramName, T paramValue, T maximumValue, System.Func<T, T, bool> validation) { }
        public static void IsMinimal<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, T minimumValue)
            where T : System.IComparable { }
        public static void IsMinimal<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, T minimumValue, System.Func<T, T, bool> validation) { }
        public static void IsMinimal<T>(string paramName, T paramValue, T minimumValue)
            where T : System.IComparable { }
        public static void IsMinimal<T>(string paramName, T paramValue, T minimumValue, System.Func<T, T, bool> validation) { }
        public static void IsNotEmpty(System.Linq.Expressions.Expression<System.Func<System.Guid>> expression) { }
        public static void IsNotEmpty(string paramName, System.Guid paramValue) { }
        public static void IsNotMatch(System.Linq.Expressions.Expression<System.Func<string>> expression, string pattern, System.Text.RegularExpressions.RegexOptions regexOptions = 0) { }
        public static void IsNotMatch(string paramName, string paramValue, string pattern, System.Text.RegularExpressions.RegexOptions regexOptions = 0) { }
        public static void IsNotNull(string paramName, object paramValue) { }
        public static void IsNotNull<T>(System.Linq.Expressions.Expression<System.Func<T>> expression)
            where T :  class { }
        public static void IsNotNull<T>(string paramName, T paramValue) { }
        public static void IsNotNullOrEmpty(System.Linq.Expressions.Expression<System.Func<System.Guid?>> expression) { }
        public static void IsNotNullOrEmpty(System.Linq.Expressions.Expression<System.Func<string>> expression) { }
        public static void IsNotNullOrEmpty(string paramName, System.Guid? paramValue) { }
        public static void IsNotNullOrEmpty(string paramName, string paramValue) { }
        public static void IsNotNullOrEmptyArray(System.Linq.Expressions.Expression<System.Func<System.Array>> expression) { }
        public static void IsNotNullOrEmptyArray(string paramName, System.Array paramValue) { }
        public static void IsNotNullOrWhitespace(System.Linq.Expressions.Expression<System.Func<string>> expression) { }
        public static void IsNotNullOrWhitespace(string paramName, string paramValue) { }
        public static void IsNotOfOneOfTheTypes(string paramName, object instance, System.Type[] notRequiredTypes) { }
        public static void IsNotOfOneOfTheTypes(string paramName, System.Type type, System.Type[] notRequiredTypes) { }
        public static void IsNotOfOneOfTheTypes<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, System.Type[] notRequiredTypes)
            where T :  class { }
        public static void IsNotOfType(string paramName, object instance, System.Type notRequiredType) { }
        public static void IsNotOfType(string paramName, System.Type type, System.Type notRequiredType) { }
        public static void IsNotOfType<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, System.Type notRequiredType)
            where T :  class { }
        public static void IsNotOutOfRange<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, T minimumValue, T maximumValue)
            where T : System.IComparable { }
        public static void IsNotOutOfRange<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, T minimumValue, T maximumValue, System.Func<T, T, T, bool> validation) { }
        public static void IsNotOutOfRange<T>(string paramName, T paramValue, T minimumValue, T maximumValue)
            where T : System.IComparable { }
        public static void IsNotOutOfRange<T>(string paramName, T paramValue, T minimumValue, T maximumValue, System.Func<T, T, T, bool> validation) { }
        public static void IsOfOneOfTheTypes(string paramName, object instance, System.Type[] requiredTypes) { }
        public static void IsOfOneOfTheTypes(string paramName, System.Type type, System.Type[] requiredTypes) { }
        public static void IsOfOneOfTheTypes<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, System.Type[] requiredTypes)
            where T :  class { }
        public static void IsOfType(string paramName, object instance, System.Type requiredType) { }
        public static void IsOfType(string paramName, System.Type type, System.Type requiredType) { }
        public static void IsOfType<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, System.Type requiredType)
            where T :  class { }
        public static void IsSupported(bool isSupported, string errorFormat, params object[] args) { }
        public static void IsValid<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, Catel.Data.IValueValidator<T> validator) { }
        public static void IsValid<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, bool validation) { }
        public static void IsValid<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, System.Func<bool> validation) { }
        public static void IsValid<T>(System.Linq.Expressions.Expression<System.Func<T>> expression, System.Func<T, bool> validation) { }
        public static void IsValid<T>(string paramName, T paramValue, Catel.Data.IValueValidator<T> validator) { }
        public static void IsValid<T>(string paramName, T paramValue, bool validation) { }
        public static void IsValid<T>(string paramName, T paramValue, System.Func<bool> validation) { }
        public static void IsValid<T>(string paramName, T paramValue, System.Func<T, bool> validation) { }
    }
    public static class AsyncEventHandlerExtensions
    {
        public static System.Threading.Tasks.Task<bool> SafeInvokeAsync(this Catel.AsyncEventHandler<System.EventArgs>? handler, object sender, bool allowParallelExecution = true) { }
        public static System.Threading.Tasks.Task<bool> SafeInvokeAsync<TEventArgs>(this Catel.AsyncEventHandler<TEventArgs>? handler, object sender, TEventArgs e, bool allowParallelExecution = true)
            where TEventArgs : System.EventArgs { }
    }
    public delegate System.Threading.Tasks.Task AsyncEventHandler<TEventArgs>(object sender, TEventArgs e);
    public static class ByteArrayExtensions
    {
        public static string GetString(this byte[] data, System.Text.Encoding encoding) { }
        public static string GetUtf8String(this byte[] data) { }
    }
    public class CatelException : System.Exception
    {
        public CatelException() { }
        public CatelException(string message) { }
        public CatelException(string message, System.Exception innerException) { }
    }
    public class CoreModule : Catel.IoC.IServiceLocatorInitializer
    {
        public CoreModule() { }
        public void Initialize(Catel.IoC.IServiceLocator serviceLocator) { }
    }
    public abstract class Disposable : System.IDisposable
    {
        protected Disposable() { }
        public bool IsDisposed { get; }
        protected void CheckDisposed() { }
        public void Dispose() { }
        protected virtual void DisposeManaged() { }
        protected virtual void DisposeUnmanaged() { }
        protected override void Finalize() { }
    }
    public class DisposableToken : Catel.DisposableToken<object>
    {
        public DisposableToken(object instance, System.Action<Catel.IDisposableToken<object>> initialize, System.Action<Catel.IDisposableToken<object>> dispose, object? tag = null) { }
    }
    public class DisposableToken<T> : Catel.Disposable, Catel.IDisposableToken<T>, System.IDisposable
    {
        public DisposableToken(T instance, System.Action<Catel.IDisposableToken<T>> initialize, System.Action<Catel.IDisposableToken<T>> dispose, object? tag = null) { }
        public T Instance { get; }
        public object? Tag { get; }
        protected override void DisposeManaged() { }
    }
    public static class Enum<TEnum>
        where TEnum :  struct, System.IComparable, System.IFormattable
    {
        public static TEnum ConvertFromOtherEnumValue(object inputEnumValue) { }
        public static TEnum ConvertFromOtherEnumValue<TOtherEnum>(TOtherEnum inputEnumValue)
            where TOtherEnum :  struct, System.IComparable, System.IFormattable { }
        public static string? GetName(int value) { }
        public static string? GetName(long value) { }
        public static string[] GetNames() { }
        public static System.Collections.Generic.List<TEnum> GetValues() { }
        public static TEnum Parse(string input, bool ignoreCase = false) { }
        public static System.Collections.Generic.List<TEnum> ToList() { }
        public static string ToString(TEnum value) { }
        public static bool TryParse(string input, out TEnum result) { }
        public static bool TryParse(string input, out TEnum? result) { }
        public static bool TryParse(string input, bool ignoreCase, out TEnum result) { }
        public static bool TryParse(string input, bool ignoreCase, out TEnum? result) { }
        public static class DataBinding
        {
            public static System.Collections.Generic.IList<Catel.IBindableEnum<TEnum>> CreateList(Catel.Enum<TEnum>.DataBinding.FormatEnumName? formatName = null) { }
            public delegate string FormatEnumName<TEnum>(TEnum value);
        }
        public static class Flags
        {
            public static TEnum ClearFlag(int flags, int flagToClear) { }
            public static TEnum ClearFlag(int flags, TEnum flagToClear) { }
            public static TEnum ClearFlag(long flags, long flagToClear) { }
            public static TEnum ClearFlag(long flags, TEnum flagToClear) { }
            public static TEnum ClearFlag(TEnum flags, TEnum flagToClear) { }
            public static TEnum[] GetValues(TEnum flags) { }
            public static bool IsFlagSet(int flags, int flagToFind) { }
            public static bool IsFlagSet(int flags, TEnum flagToFind) { }
            public static bool IsFlagSet(long flags, long flagToFind) { }
            public static bool IsFlagSet(long flags, TEnum flagToFind) { }
            public static bool IsFlagSet(TEnum flags, TEnum flagToFind) { }
            public static TEnum SetFlag(int flags, int flagToSet) { }
            public static TEnum SetFlag(int flags, TEnum flagToSet) { }
            public static TEnum SetFlag(long flags, long flagToSet) { }
            public static TEnum SetFlag(long flags, TEnum flagToSet) { }
            public static TEnum SetFlag(TEnum flags, TEnum flagToSet) { }
            public static TEnum SwapFlag(int flags, int flagToSwap) { }
            public static TEnum SwapFlag(int flags, TEnum flagToSwap) { }
            public static TEnum SwapFlag(long flags, long flagToSwap) { }
            public static TEnum SwapFlag(long flags, TEnum flagToSwap) { }
            public static TEnum SwapFlag(TEnum flags, TEnum flagToSwap) { }
        }
    }
    public static class EnvironmentHelper
    {
        public static bool IsProcessHostedByExpressionBlend { get; }
        public static bool IsProcessHostedBySharpDevelop { get; }
        public static bool IsProcessHostedByTool { get; }
        public static bool IsProcessHostedByVisualStudio { get; }
        public static bool IsProcessCurrentlyHostedByExpressionBlend(bool checkParentProcesses = false) { }
        public static bool IsProcessCurrentlyHostedBySharpDevelop(bool checkParentProcesses = false) { }
        public static bool IsProcessCurrentlyHostedByTool(bool checkParentProcesses = false) { }
        public static bool IsProcessCurrentlyHostedByVisualStudio(bool checkParentProcesses = false) { }
    }
    public static class ExceptionExtensions
    {
        public static TException? Find<TException>(this System.Exception exception)
            where TException : System.Exception { }
        public static string Flatten(this System.Exception exception, string message = "", bool includeStackTrace = false) { }
        public static System.Collections.Generic.IEnumerable<System.Exception> GetAllInnerExceptions(this System.Exception exception) { }
        public static bool IsCritical(this System.Exception ex) { }
    }
    public static class ExceptionFactory
    {
        public static System.Exception? CreateException(System.Type exceptionType, object[] args) { }
        public static System.Exception? CreateException(System.Type exceptionType, string message, System.Exception? innerException = null) { }
        public static TException? CreateException<TException>(object[] args)
            where TException : System.Exception { }
        public static TException? CreateException<TException>(string message, System.Exception? innerException = null)
            where TException : System.Exception { }
    }
    public static class ExpressionHelper
    {
        public static object? GetOwner<TProperty>(System.Linq.Expressions.Expression<System.Func<TProperty>> propertyExpression) { }
        public static string GetPropertyName<TProperty>(System.Linq.Expressions.Expression<System.Func<TProperty>> propertyExpression) { }
        public static string GetPropertyName<TSource, TProperty>(System.Linq.Expressions.Expression<System.Func<TSource, TProperty>> propertyExpression) { }
    }
    public static class FastDateTime
    {
        public static System.DateTime Now { get; }
        public static System.DateTime UtcNow { get; }
    }
    public static class HashHelper
    {
        public static int CombineHash(params int[] hashCodes) { }
    }
    public interface IBindableEnum<TEnum> : System.IComparable<Catel.IBindableEnum<TEnum>>, System.IEquatable<Catel.IBindableEnum<TEnum>>
        where TEnum :  struct, System.IComparable, System.IFormattable
    {
        string Name { get; }
        TEnum Value { get; }
    }
    public interface IDisposableToken<T> : System.IDisposable
    {
        T Instance { get; }
        object? Tag { get; }
    }
    public interface IExecute
    {
        bool Execute();
    }
    public interface IExecuteWithObject
    {
        bool ExecuteWithObject(object parameter);
    }
    public interface IExecuteWithObject<TParameter> : Catel.IExecuteWithObject
    {
        bool ExecuteWithObject(TParameter parameter);
    }
    public interface IExecuteWithObject<TParameter, TResult>
    {
        bool ExecuteWithObject(TParameter parameter, out TResult result);
    }
    public interface IExecute<TResult>
    {
        bool Execute(out TResult result);
    }
    public interface IUniqueIdentifyable
    {
        int UniqueIdentifier { get; }
    }
    public interface IWeakAction : Catel.IExecute, Catel.IWeakReference
    {
        System.Delegate? Action { get; }
        string MethodName { get; }
    }
    public interface IWeakAction<TParameter> : Catel.IExecuteWithObject, Catel.IExecuteWithObject<TParameter>, Catel.IWeakReference
    {
        System.Delegate? Action { get; }
        string MethodName { get; }
        bool Execute(TParameter parameter);
    }
    public interface IWeakEventListener
    {
        System.Type EventArgsType { get; }
        bool IsSourceAlive { get; }
        bool IsStaticEvent { get; }
        bool IsStaticEventHandler { get; }
        bool IsTargetAlive { get; }
        object? Source { get; }
        System.Type? SourceType { get; }
        System.WeakReference? SourceWeakReference { get; }
        object? Target { get; }
        System.Type? TargetType { get; }
        System.WeakReference? TargetWeakReference { get; }
        void Detach();
    }
    public interface IWeakFunc<TResult> : Catel.IExecute<TResult>, Catel.IWeakReference
    {
        System.Delegate? Action { get; }
        string MethodName { get; }
    }
    public interface IWeakFunc<TParameter, TResult> : Catel.IExecuteWithObject<TParameter, TResult>, Catel.IWeakReference
    {
        System.Delegate? Action { get; }
        string MethodName { get; }
    }
    public interface IWeakReference
    {
        bool IsTargetAlive { get; }
        object? Target { get; }
    }
    public enum KnownPlatforms
    {
        Unknown = 0,
        NET = 1,
        NET6 = 2,
        NET7 = 3,
        NET8 = 4,
    }
    public static class LanguageHelper
    {
        public static string GetRequiredString(string resourceName) { }
        public static string GetRequiredString(string resourceName, System.Globalization.CultureInfo cultureInfo) { }
        public static string? GetString(string resourceName) { }
        public static string? GetString(string resourceName, System.Globalization.CultureInfo cultureInfo) { }
    }
    public class MustBeImplementedException : System.Exception
    {
        public MustBeImplementedException() { }
    }
    public class NotSupportedInPlatformException : System.Exception
    {
        public NotSupportedInPlatformException() { }
        public NotSupportedInPlatformException(string message) { }
        public NotSupportedInPlatformException(string featureFormat = "", params object[] args) { }
        public Catel.SupportedPlatforms Platform { get; }
        public string Reason { get; }
    }
    public static class ObjectHelper
    {
        public static bool AreEqual(object? object1, object? object2) { }
        public static bool AreEqualReferences(object? object1, object? object2) { }
        public static bool IsNull(object? obj) { }
    }
    public static class ObjectToStringHelper
    {
        public static System.Globalization.CultureInfo DefaultCulture { get; set; }
        public static string ToFullTypeString(object? instance) { }
        public static string ToString(object? instance) { }
        public static string ToString(object? instance, System.Globalization.CultureInfo cultureInfo) { }
        public static string ToTypeString(object? instance) { }
    }
    public delegate void OpenInstanceActionHandler<TTarget>(TTarget @this);
    public delegate void OpenInstanceEventHandler<TTarget, TEventArgs>(TTarget @this, object sender, TEventArgs e);
    public static class ParallelHelper
    {
        public static void ExecuteInParallel<T>(System.Collections.Generic.List<T> items, System.Action<T> actionToInvoke, int itemsPerBatch = 1000, string? taskName = null) { }
        public static System.Threading.Tasks.Task ExecuteInParallelAsync(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> tasks, int batchSize = 1000, string? taskName = null) { }
    }
    public static class Platforms
    {
        public static Catel.SupportedPlatforms CurrentPlatform { get; }
        public static bool IsPlatformSupported(Catel.KnownPlatforms platformToCheck) { }
        public static bool IsPlatformSupported(Catel.KnownPlatforms platformToCheck, Catel.SupportedPlatforms currentPlatform) { }
    }
    public class ProgressContext : Catel.Disposable
    {
        public ProgressContext(long totalCount, int numberOfRefreshes) { }
        public long CurrentCount { get; set; }
        public int CurrentRefreshNumber { get; }
        public bool IsRefreshRequired { get; }
        public int NumberOfRefreshes { get; }
        public double Percentage { get; }
        public long TotalCount { get; }
    }
    public static class ResourceHelper
    {
        public static string? ExtractEmbeddedResource(this System.Reflection.Assembly assembly, string relativeResourceName) { }
        public static void ExtractEmbeddedResource(this System.Reflection.Assembly assembly, string relativeResourceName, System.IO.Stream targetStream) { }
        public static void ExtractEmbeddedResource(this System.Reflection.Assembly assembly, string assemblyName, string relativeResourceName, System.IO.Stream targetStream) { }
        public static string? GetString(string resourceName) { }
        public static string? GetString(System.Type callingType, string resourceFile, string resourceName) { }
    }
    public static class StringExtensions
    {
        public static readonly System.Text.RegularExpressions.Regex SlugRegex;
        public static readonly System.Text.RegularExpressions.Regex WhiteSpaceRegex;
        public static bool ContainsIgnoreCase(this string str, string valueToCheck) { }
        public static bool EndsWithAny(this string str, params string[] values) { }
        public static bool EndsWithAnyIgnoreCase(this string str, params string[] values) { }
        public static bool EndsWithIgnoreCase(this string str, string valueToCheck) { }
        public static bool EqualsAny(this string str, params string[] values) { }
        public static bool EqualsAnyIgnoreCase(this string str, params string[] values) { }
        public static bool EqualsIgnoreCase(this string str, string valueToCheck) { }
        public static string GetSlug(this string input, string spaceReplacement = "", string dotReplacement = "", bool makeLowercase = true) { }
        public static int IndexOfIgnoreCase(this string str, string valueToCheck) { }
        public static string PrepareAsSearchFilter(this string filter) { }
        public static string RemoveDiacritics(this string value) { }
        public static string SplitCamelCase(this string value) { }
        public static bool StartsWithAny(this string str, params string[] values) { }
        public static bool StartsWithAnyIgnoreCase(this string str, params string[] values) { }
        public static bool StartsWithIgnoreCase(this string str, string valueToCheck) { }
    }
    public static class StringToObjectHelper
    {
        public static System.Globalization.CultureInfo DefaultCulture { get; set; }
        public static bool ToBool(string value) { }
        public static byte ToByte(string value) { }
        public static byte[] ToByteArray(string value) { }
        public static System.DateTime ToDateTime(string value) { }
        public static System.DateTime ToDateTime(string value, System.Globalization.CultureInfo cultureInfo) { }
        public static decimal ToDecimal(string value) { }
        public static decimal ToDecimal(string value, System.Globalization.CultureInfo cultureInfo) { }
        public static double ToDouble(string value) { }
        public static double ToDouble(string value, System.Globalization.CultureInfo cultureInfo) { }
        public static TEnumValue ToEnum<TEnumValue>(string value, TEnumValue defaultValue)
            where TEnumValue :  struct, System.IComparable, System.IFormattable { }
        public static float ToFloat(string value) { }
        public static float ToFloat(string value, System.Globalization.CultureInfo cultureInfo) { }
        public static System.Guid ToGuid(string value) { }
        public static int ToInt(string value) { }
        public static int ToInt(string value, System.Globalization.CultureInfo cultureInfo) { }
        public static long ToLong(string value) { }
        public static long ToLong(string value, System.Globalization.CultureInfo cultureInfo) { }
        public static object? ToRightType(System.Type targetType, string value) { }
        public static object? ToRightType(System.Type targetType, string value, System.Globalization.CultureInfo cultureInfo) { }
        public static short ToShort(string value) { }
        public static short ToShort(string value, System.Globalization.CultureInfo cultureInfo) { }
        public static string? ToString(string value) { }
        public static System.TimeSpan ToTimeSpan(string value) { }
        public static System.TimeSpan ToTimeSpan(string value, System.Globalization.CultureInfo cultureInfo) { }
        public static System.Type? ToType(string value) { }
        public static uint ToUInt(string value) { }
        public static uint ToUInt(string value, System.Globalization.CultureInfo cultureInfo) { }
        public static ulong ToULong(string value) { }
        public static ulong ToULong(string value, System.Globalization.CultureInfo cultureInfo) { }
        public static ushort ToUShort(string value) { }
        public static ushort ToUShort(string value, System.Globalization.CultureInfo cultureInfo) { }
        public static System.Uri? ToUri(string value) { }
    }
    public enum SupportedPlatforms
    {
        NET6 = 0,
        NET7 = 1,
        NET8 = 2,
    }
    public static class TagHelper
    {
        public static bool AreTagsEqual(object? firstTag, object? secondTag) { }
        public static string? ToString(object? tag) { }
    }
    public static class ThreadHelper
    {
        public static int GetCurrentThreadId() { }
        public static void Sleep(int millisecondsTimeout) { }
        public static void SpinWait(int iterations) { }
    }
    public static class UniqueIdentifierHelper
    {
        public static int GetUniqueIdentifier(System.Type type) { }
        public static int GetUniqueIdentifier<T>() { }
    }
    public static class UriExtensions
    {
        public static string GetSafeUriString(this System.Uri uri) { }
        public static bool IsAbsoluteUrl(this string url) { }
    }
    public class WeakAction : Catel.WeakActionBase, Catel.IExecute, Catel.IWeakAction, Catel.IWeakReference
    {
        public WeakAction(object target, System.Action action) { }
        public System.Delegate? Action { get; }
        public string MethodName { get; }
        public bool Execute() { }
        public delegate void OpenInstanceAction<TTarget>(TTarget @this);
    }
    public abstract class WeakActionBase : Catel.IWeakReference
    {
        protected WeakActionBase(object? target) { }
        public bool IsTargetAlive { get; }
        public object? Target { get; }
    }
    public class WeakAction<TParameter> : Catel.WeakActionBase, Catel.IExecuteWithObject, Catel.IExecuteWithObject<TParameter>, Catel.IWeakAction<TParameter>, Catel.IWeakReference
    {
        public WeakAction(object target, System.Action<TParameter> action) { }
        public System.Delegate? Action { get; }
        public string MethodName { get; }
        public bool Execute(TParameter parameter) { }
        public delegate void OpenInstanceGenericAction<TParameter, TTarget>(TTarget @this, TParameter parameter);
    }
    public static class WeakEventListener
    {
        public static Catel.IWeakEventListener? SubscribeToWeakCollectionChangedEvent(this object target, object source, System.Collections.Specialized.NotifyCollectionChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "CollectionChanged") { }
        public static Catel.IWeakEventListener? SubscribeToWeakEvent(this object target, object source, string eventName, System.Action handler, bool throwWhenSubscriptionFails = true) { }
        public static Catel.IWeakEventListener? SubscribeToWeakEvent(this object target, object source, string eventName, System.Delegate handler, bool throwWhenSubscriptionFails = true) { }
        public static Catel.IWeakEventListener? SubscribeToWeakGenericEvent<TEventArgs>(this object target, object source, string eventName, System.EventHandler<TEventArgs> handler, bool throwWhenSubscriptionFails = true)
            where TEventArgs : System.EventArgs { }
        public static Catel.IWeakEventListener? SubscribeToWeakPropertyChangedEvent(this object target, object source, System.ComponentModel.PropertyChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "PropertyChanged") { }
    }
    public static class WeakEventListener<TTarget, TSource>
        where TTarget :  class
        where TSource :  class
    {
        public static Catel.IWeakEventListener? SubscribeToWeakCollectionChangedEvent(TTarget target, TSource source, System.Collections.Specialized.NotifyCollectionChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "CollectionChanged") { }
        public static Catel.IWeakEventListener? SubscribeToWeakEvent(TTarget target, TSource source, string eventName, System.Delegate handler, bool throwWhenSubscriptionFails = true) { }
        public static Catel.IWeakEventListener? SubscribeToWeakEventWithExplicitSourceType<TExplicitSourceType>(TTarget target, TSource source, string eventName, System.Delegate handler, bool throwWhenSubscriptionFails = true) { }
        public static Catel.IWeakEventListener? SubscribeToWeakGenericEvent<TEventArgs>(TTarget target, TSource source, string eventName, System.EventHandler<TEventArgs> handler, bool throwWhenSubscriptionFails = true)
            where TEventArgs : System.EventArgs { }
        public static Catel.IWeakEventListener? SubscribeToWeakPropertyChangedEvent(TTarget target, TSource source, System.ComponentModel.PropertyChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "PropertyChanged") { }
    }
    public class WeakEventListener<TTarget, TSource, TEventArgs> : Catel.IWeakEventListener
        where TTarget :  class
        where TSource :  class
    {
        public System.Type EventArgsType { get; }
        public bool IsSourceAlive { get; }
        public bool IsStaticEvent { get; }
        public bool IsStaticEventHandler { get; }
        public bool IsTargetAlive { get; }
        public object? Source { get; }
        public System.Type? SourceType { get; }
        public System.WeakReference? SourceWeakReference { get; }
        public object? Target { get; }
        public System.Type? TargetType { get; }
        public System.WeakReference? TargetWeakReference { get; }
        public void Detach() { }
        public void OnEvent(object? source, TEventArgs eventArgs) { }
        public static Catel.IWeakEventListener? SubscribeToWeakCollectionChangedEvent(TTarget? target, TSource? source, System.Collections.Specialized.NotifyCollectionChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "CollectionChanged") { }
        public static Catel.IWeakEventListener? SubscribeToWeakEvent(TTarget? target, TSource? source, string eventName, System.Delegate handler, bool throwWhenSubscriptionFails = true) { }
        public static Catel.IWeakEventListener? SubscribeToWeakEventWithExplicitSourceType<TExplicitSourceType>(TTarget? target, TSource? source, string eventName, System.Delegate handler, bool throwWhenSubscriptionFails = true)
            where TExplicitSourceType :  class { }
        public static Catel.IWeakEventListener? SubscribeToWeakGenericEvent(TTarget? target, TSource? source, string eventName, System.EventHandler<TEventArgs> handler, bool throwWhenSubscriptionFails = true) { }
        public static Catel.IWeakEventListener? SubscribeToWeakPropertyChangedEvent(TTarget? target, TSource? source, System.ComponentModel.PropertyChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "PropertyChanged") { }
    }
    public class WeakFunc<TResult> : Catel.WeakActionBase, Catel.IExecute<TResult>, Catel.IWeakFunc<TResult>, Catel.IWeakReference
    {
        public WeakFunc(object target, System.Func<TResult> func) { }
        public System.Delegate? Action { get; }
        public string MethodName { get; }
        public bool Execute(out TResult result) { }
        public delegate TResult OpenInstanceAction<TResult, TTarget>(TTarget @this);
    }
    public class WeakFunc<TParameter, TResult> : Catel.WeakActionBase, Catel.IExecuteWithObject<TParameter, TResult>, Catel.IWeakFunc<TParameter, TResult>, Catel.IWeakReference
    {
        public WeakFunc(object target, System.Func<TParameter, TResult> func) { }
        public System.Delegate? Action { get; }
        public string MethodName { get; }
        public bool Execute(TParameter parameter, out TResult result) { }
        public delegate TResult OpenInstanceGenericAction<TParameter, TResult, TTarget>(TTarget @this, TParameter parameter);
    }
}
namespace Catel.Caching
{
    public class CacheStorage<TKey, TValue> : Catel.Caching.ICacheStorage<TKey, TValue>
        where TKey :  notnull
    {
        public CacheStorage(System.Func<Catel.Caching.Policies.ExpirationPolicy>? defaultExpirationPolicyInitCode = null, bool storeNullValues = false, System.Collections.Generic.IEqualityComparer<TKey>? equalityComparer = null) { }
        public bool DisposeValuesOnRemoval { get; set; }
        public System.TimeSpan ExpirationTimerInterval { get; set; }
        public TValue this[TKey key] { get; }
        public System.Collections.Generic.IEnumerable<TKey> Keys { get; }
        public event System.EventHandler<Catel.Caching.ExpiredEventArgs<TKey, TValue>>? Expired;
        public event System.EventHandler<Catel.Caching.ExpiringEventArgs<TKey, TValue>>? Expiring;
        public void Add(TKey key, TValue value, Catel.Caching.Policies.ExpirationPolicy? expirationPolicy, bool override = false) { }
        public void Add(TKey key, TValue value, bool override = false, System.TimeSpan expiration = default) { }
        public void Clear() { }
        public bool Contains(TKey key) { }
        public TValue? Get(TKey key) { }
        public TValue GetFromCacheOrFetch(TKey key, System.Func<TValue> code, Catel.Caching.Policies.ExpirationPolicy? expirationPolicy, bool override = false) { }
        public TValue GetFromCacheOrFetch(TKey key, System.Func<TValue> code, bool override = false, System.TimeSpan expiration = default) { }
        public System.Threading.Tasks.Task<TValue> GetFromCacheOrFetchAsync(TKey key, System.Func<System.Threading.Tasks.Task<TValue>> code, Catel.Caching.Policies.ExpirationPolicy? expirationPolicy, bool override = false) { }
        public System.Threading.Tasks.Task<TValue> GetFromCacheOrFetchAsync(TKey key, System.Func<System.Threading.Tasks.Task<TValue>> code, bool override = false, System.TimeSpan expiration = default) { }
        public void Remove(TKey key, System.Action? action = null) { }
    }
    public class ExpiredEventArgs<TKey, TValue> : System.EventArgs
        where TKey :  notnull
    {
        public ExpiredEventArgs(TKey key, TValue value, bool dispose) { }
        public bool Dispose { get; set; }
        public TKey Key { get; }
        public TValue Value { get; }
    }
    public class ExpiringEventArgs<TKey, TValue> : System.EventArgs
        where TKey :  notnull
    {
        public ExpiringEventArgs(TKey key, TValue? value, Catel.Caching.Policies.ExpirationPolicy? expirationPolicy) { }
        public bool Cancel { get; set; }
        public Catel.Caching.Policies.ExpirationPolicy? ExpirationPolicy { get; set; }
        public TKey Key { get; }
        public TValue Value { get; }
    }
    public interface ICacheStorage<TKey, TValue>
        where TKey :  notnull
    {
        bool DisposeValuesOnRemoval { get; set; }
        System.TimeSpan ExpirationTimerInterval { get; set; }
        TValue this[TKey key] { get; }
        System.Collections.Generic.IEnumerable<TKey> Keys { get; }
        event System.EventHandler<Catel.Caching.ExpiredEventArgs<TKey, TValue>>? Expired;
        event System.EventHandler<Catel.Caching.ExpiringEventArgs<TKey, TValue>>? Expiring;
        void Add(TKey key, TValue value, Catel.Caching.Policies.ExpirationPolicy? expirationPolicy, bool override = false);
        void Add(TKey key, TValue value, bool override = false, System.TimeSpan expiration = default);
        void Clear();
        bool Contains(TKey key);
        TValue? Get(TKey key);
        TValue GetFromCacheOrFetch(TKey key, System.Func<TValue> code, Catel.Caching.Policies.ExpirationPolicy? expirationPolicy, bool override = false);
        TValue GetFromCacheOrFetch(TKey key, System.Func<TValue> code, bool override = false, System.TimeSpan expiration = default);
        System.Threading.Tasks.Task<TValue> GetFromCacheOrFetchAsync(TKey key, System.Func<System.Threading.Tasks.Task<TValue>> code, Catel.Caching.Policies.ExpirationPolicy? expirationPolicy, bool override = false);
        System.Threading.Tasks.Task<TValue> GetFromCacheOrFetchAsync(TKey key, System.Func<System.Threading.Tasks.Task<TValue>> code, bool override = false, System.TimeSpan expiration = default);
        void Remove(TKey key, System.Action? action = null);
    }
}
namespace Catel.Caching.Policies
{
    public class AbsoluteExpirationPolicy : Catel.Caching.Policies.ExpirationPolicy
    {
        protected AbsoluteExpirationPolicy(System.DateTime absoluteExpirationDateTime, bool canReset) { }
        protected System.DateTime AbsoluteExpirationDateTime { get; set; }
        public override bool IsExpired { get; }
    }
    public sealed class CompositeExpirationPolicy : Catel.Caching.Policies.ExpirationPolicy
    {
        public CompositeExpirationPolicy(bool expiresOnlyIfAllPoliciesExpires = false) { }
        public override bool CanReset { get; }
        public override bool IsExpired { get; }
        public Catel.Caching.Policies.CompositeExpirationPolicy Add(Catel.Caching.Policies.ExpirationPolicy expirationPolicy) { }
        protected override void OnReset() { }
    }
    public sealed class CustomExpirationPolicy : Catel.Caching.Policies.ExpirationPolicy
    {
        public CustomExpirationPolicy(System.Func<bool>? isExpiredFunc = null, System.Action? resetAction = null) { }
        public override bool IsExpired { get; }
        protected override void OnReset() { }
    }
    public class DurationExpirationPolicy : Catel.Caching.Policies.AbsoluteExpirationPolicy
    {
        protected DurationExpirationPolicy(System.TimeSpan durationTimeSpan, bool canReset) { }
        protected System.TimeSpan DurationTimeSpan { get; set; }
    }
    public abstract class ExpirationPolicy
    {
        protected ExpirationPolicy(bool canReset = false) { }
        public bool CanReset { get; }
        public abstract bool IsExpired { get; }
        protected bool IsResting { get; }
        protected virtual void OnReset() { }
        public void Reset() { }
        public static Catel.Caching.Policies.ExpirationPolicy Absolute(System.DateTime absoluteExpirationDateTime) { }
        public static Catel.Caching.Policies.ExpirationPolicy Custom(System.Func<bool> isExpiredFunc, System.Action? resetAction = null) { }
        public static Catel.Caching.Policies.ExpirationPolicy Duration(System.TimeSpan durationTimeSpan) { }
        public static Catel.Caching.Policies.ExpirationPolicy Sliding(System.TimeSpan durationTimeSpan) { }
    }
    public sealed class SlidingExpirationPolicy : Catel.Caching.Policies.DurationExpirationPolicy
    {
        protected override void OnReset() { }
    }
}
namespace Catel.Collections
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this System.Collections.Generic.ICollection<T> collection, System.Collections.Generic.IEnumerable<T> range) { }
        public static System.Collections.ObjectModel.ReadOnlyCollection<T> AsReadOnly<T>(this System.Collections.Generic.IList<T> collection) { }
        public static bool CanMoveItemDown(this System.Collections.IList list, object item) { }
        public static bool CanMoveItemUp(this System.Collections.IList list, object item) { }
        public static void ForEach<TItem>(this System.Collections.Generic.IEnumerable<TItem> collection, System.Action<TItem> action) { }
        public static int IndexOf<T>(this System.Collections.Generic.IList<T> list, T item, int index) { }
        public static bool MoveItemDown(this System.Collections.IList list, object item) { }
        public static bool MoveItemDownByIndex(this System.Collections.IList list, int index) { }
        public static bool MoveItemUp(this System.Collections.IList list, object item) { }
        public static bool MoveItemUpByIndex(this System.Collections.IList list, int index) { }
        public static void RemoveFirst(this System.Collections.IList list) { }
        public static void RemoveLast(this System.Collections.IList list) { }
        public static void ReplaceRange<T>(this System.Collections.Generic.ICollection<T> collection, System.Collections.Generic.IEnumerable<T> range) { }
        public static void Sort<T>(this System.Collections.Generic.IList<T> existingSet, System.Func<T, T, int>? comparer = null) { }
        public static System.Collections.Generic.IEnumerable<T> SynchronizeCollection<T>(this System.Collections.Generic.ICollection<T> existingSet, System.Collections.Generic.IEnumerable<T> newSet, bool updateExistingSet = true) { }
        public static System.Collections.Generic.IEnumerable<T> SynchronizeCollection<T>(this System.Collections.Generic.IList<T> existingSet, System.Collections.Generic.IEnumerable<T> newSet, bool updateExistingSet = true) { }
        public static System.Array ToArray(this System.Collections.IEnumerable collection, System.Type elementType) { }
    }
    public static class CollectionHelper
    {
        public static bool IsEqualTo(System.Collections.IEnumerable listA, System.Collections.IEnumerable listB) { }
    }
    public static class DictionaryExtensions
    {
        public static void AddItemIfNotEmpty<TKey>(this System.Collections.Generic.Dictionary<TKey, string> dictionary, TKey key, string value)
            where TKey :  notnull { }
        public static void AddRange<TKey, TValue>(this System.Collections.Generic.Dictionary<TKey, TValue> target, System.Collections.Generic.Dictionary<TKey, TValue> source, bool overwriteExisting = true)
            where TKey :  notnull { }
        public static void AddRange<TKey, TValue>(this System.Collections.Generic.Dictionary<TKey, TValue> target, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> source, bool overwriteExisting = true)
            where TKey :  notnull { }
    }
    public static class EnumerableExtensions
    {
        public static Catel.Collections.FastObservableDictionary<TKey, TSource> ToObservableDictionary<TSource, TKey>(this System.Collections.Generic.IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector)
            where TKey :  notnull { }
        public static Catel.Collections.FastObservableDictionary<TKey, TSource> ToObservableDictionary<TSource, TKey>(this System.Collections.Generic.IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector, System.Collections.Generic.IEqualityComparer<TKey>? comparer)
            where TKey :  notnull { }
        public static Catel.Collections.FastObservableDictionary<TKey, TElement> ToObservableDictionary<TSource, TKey, TElement>(this System.Collections.Generic.IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector, System.Func<TSource, TElement> elementSelector)
            where TKey :  notnull { }
        public static Catel.Collections.FastObservableDictionary<TKey, TElement> ToObservableDictionary<TSource, TKey, TElement>(this System.Collections.Generic.IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector, System.Func<TSource, TElement> elementSelector, System.Collections.Generic.IEqualityComparer<TKey>? comparer)
            where TKey :  notnull { }
    }
    public class ExtendedSuspensionContext<T>
    {
        public ExtendedSuspensionContext(Catel.Collections.SuspensionMode mode) { }
        public int Count { get; set; }
        public System.Collections.Generic.List<System.Collections.Specialized.NotifyCollectionChangedAction> MixedActions { get; }
        public System.Collections.Generic.List<int> MixedItemIndices { get; }
        public System.Collections.Generic.List<T> MixedItems { get; }
        public Catel.Collections.SuspensionMode Mode { get; }
        public System.Collections.Generic.List<int> NewItemIndices { get; }
        public System.Collections.Generic.List<T> NewItems { get; }
        public System.Collections.Generic.List<int> OldItemIndices { get; }
        public System.Collections.Generic.List<T> OldItems { get; }
        public bool? TryRemoveItemFromNewItems(int index, T item) { }
        public bool TryRemoveItemFromOldItems(int index, T item) { }
    }
    [System.Serializable]
    public class FastBindingList<T> : System.ComponentModel.BindingList<T>, Catel.Collections.ISuspendChangeNotificationsCollection, System.Collections.ICollection, System.Collections.IEnumerable
    {
        public FastBindingList() { }
        public FastBindingList(System.Collections.Generic.IEnumerable<T> collection) { }
        public FastBindingList(System.Collections.IEnumerable collection) { }
        public bool AutomaticallyDispatchChangeNotifications { get; set; }
        public bool IsDirty { get; set; }
        protected override bool IsSortedCore { get; }
        public bool NotificationsSuspended { get; }
        protected override System.ComponentModel.ListSortDirection SortDirectionCore { get; }
        protected override System.ComponentModel.PropertyDescriptor? SortPropertyCore { get; }
        protected override bool SupportsSearchingCore { get; }
        protected override bool SupportsSortingCore { get; }
        public void AddItems(System.Collections.Generic.IEnumerable<T> collection) { }
        public void AddItems(System.Collections.IEnumerable collection) { }
        protected override void ApplySortCore(System.ComponentModel.PropertyDescriptor prop, System.ComponentModel.ListSortDirection direction) { }
        protected override void ClearItems() { }
        protected override int FindCore(System.ComponentModel.PropertyDescriptor prop, object key) { }
        protected override void InsertItem(int index, T item) { }
        public virtual void InsertItems(System.Collections.Generic.IEnumerable<T> collection, int index) { }
        public virtual void InsertItems(System.Collections.IEnumerable collection, int index) { }
        protected void NotifyChanges() { }
        protected override void OnListChanged(System.ComponentModel.ListChangedEventArgs e) { }
        protected override void RemoveItem(int index) { }
        public void RemoveItems(System.Collections.Generic.IEnumerable<T> collection) { }
        public void RemoveItems(System.Collections.IEnumerable collection) { }
        protected override void RemoveSortCore() { }
        public void Reset() { }
        protected override void SetItem(int index, T item) { }
        public System.IDisposable SuspendChangeNotifications() { }
        public System.IDisposable SuspendChangeNotifications(Catel.Collections.SuspensionMode mode) { }
    }
    [System.Serializable]
    public class FastObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>, Catel.Collections.ISuspendChangeNotificationsCollection, System.Collections.ICollection, System.Collections.IEnumerable
    {
        public FastObservableCollection() { }
        public FastObservableCollection(System.Collections.Generic.IEnumerable<T> collection) { }
        public FastObservableCollection(System.Collections.IEnumerable collection) { }
        public bool AutomaticallyDispatchChangeNotifications { get; set; }
        public bool IsDirty { get; set; }
        public bool NotificationsSuspended { get; }
        public void AddItems(System.Collections.Generic.IEnumerable<T> collection) { }
        public void AddItems(System.Collections.IEnumerable collection) { }
        public void AddItems(System.Collections.Generic.IEnumerable<T> collection, Catel.Collections.SuspensionMode mode) { }
        public void AddItems(System.Collections.IEnumerable collection, Catel.Collections.SuspensionMode mode) { }
        protected override void ClearItems() { }
        protected override void InsertItem(int index, T item) { }
        public virtual void InsertItems(System.Collections.Generic.IEnumerable<T> collection, int index) { }
        public virtual void InsertItems(System.Collections.IEnumerable collection, int index) { }
        public virtual void InsertItems(System.Collections.Generic.IEnumerable<T> collection, int index, Catel.Collections.SuspensionMode mode) { }
        public virtual void InsertItems(System.Collections.IEnumerable collection, int index, Catel.Collections.SuspensionMode mode) { }
        protected override void MoveItem(int oldIndex, int newIndex) { }
        protected void NotifyChanges() { }
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) { }
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) { }
        protected override void RemoveItem(int index) { }
        public void RemoveItems(System.Collections.Generic.IEnumerable<T> collection) { }
        public void RemoveItems(System.Collections.IEnumerable collection) { }
        public void RemoveItems(System.Collections.Generic.IEnumerable<T> collection, Catel.Collections.SuspensionMode mode) { }
        public void RemoveItems(System.Collections.IEnumerable collection, Catel.Collections.SuspensionMode mode) { }
        public void Reset() { }
        protected override void SetItem(int index, T item) { }
        public System.IDisposable SuspendChangeNotifications() { }
        public System.IDisposable SuspendChangeNotifications(Catel.Collections.SuspensionMode mode) { }
    }
    public class FastObservableDictionary<TKey, TValue> : Catel.Collections.ISuspendChangeNotificationsCollection, System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>, System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable, System.Collections.Specialized.INotifyCollectionChanged, System.ComponentModel.INotifyPropertyChanged, System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable
        where TKey :  notnull
    {
        protected readonly System.ComponentModel.PropertyChangedEventArgs _cachedCountArgs;
        protected readonly System.ComponentModel.PropertyChangedEventArgs _cachedIndexerArgs;
        protected readonly System.ComponentModel.PropertyChangedEventArgs _cachedKeysArgs;
        protected readonly System.Collections.Specialized.NotifyCollectionChangedEventArgs _cachedResetArgs;
        protected readonly System.ComponentModel.PropertyChangedEventArgs _cachedValuesArgs;
        public FastObservableDictionary() { }
        public FastObservableDictionary(System.Collections.Generic.IDictionary<TKey, TValue> dictionary) { }
        public FastObservableDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> originalDict) { }
        public FastObservableDictionary(System.Collections.Generic.IEqualityComparer<TKey>? comparer) { }
        public FastObservableDictionary(int capacity) { }
        public FastObservableDictionary(System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.IEqualityComparer<TKey>? comparer) { }
        public FastObservableDictionary(int capacity, System.Collections.Generic.IEqualityComparer<TKey>? comparer) { }
        protected FastObservableDictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public bool AutomaticallyDispatchChangeNotifications { get; set; }
        public System.Collections.Generic.IEqualityComparer<TKey> Comparer { get; }
        public int Count { get; }
        public bool IsDirty { get; }
        public bool IsFixedSize { get; }
        public bool IsReadOnly { get; }
        public bool IsSynchronized { get; }
        public TValue this[TKey key] { get; set; }
        public object? this[object key] { get; set; }
        public System.Collections.Generic.ICollection<TKey> Keys { get; }
        public bool NotificationsSuspended { get; }
        public object SyncRoot { get; }
        public System.Collections.Generic.ICollection<TValue> Values { get; }
        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler? CollectionChanged;
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        public void Add(System.Collections.Generic.KeyValuePair<TKey, TValue> item) { }
        public void Add(object key, object? value) { }
        public void Add(TKey key, TValue value) { }
        public System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> AsEnumerable() { }
        public Catel.Collections.FastObservableDictionary<TKey, TValue> AsReadOnly() { }
        public void Clear() { }
        public bool Contains(System.Collections.Generic.KeyValuePair<TKey, TValue> item) { }
        public bool Contains(object key) { }
        public bool ContainsKey(TKey key) { }
        public void CopyTo(System.Array array, int arrayIndex) { }
        public void CopyTo(System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int arrayIndex) { }
        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> GetEnumerator() { }
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public int IndexOf(System.Collections.Generic.KeyValuePair<TKey, TValue> item) { }
        public void Insert(int index, System.Collections.Generic.KeyValuePair<TKey, TValue> item) { }
        public virtual void InsertMultipleValues(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> newValues, bool checkKeyDuplication) { }
        public virtual void InsertMultipleValues(int startIndex, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> newValues, bool checkKeyDuplication) { }
        public virtual void InsertSingleValue(TKey key, TValue newValue, bool checkKeyDuplication) { }
        public virtual void InsertSingleValue(int index, TKey key, TValue newValue, bool checkKeyDuplication) { }
        protected virtual void InternalMoveItem(int oldIndex, int newIndex, TKey key, TValue element) { }
        public virtual void MoveItem(int oldIndex, int newIndex) { }
        protected void NotifyChanges() { }
        protected virtual void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs eventArgs) { }
        public void OnDeserialization(object? sender) { }
        protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs eventArgs) { }
        public bool Remove(System.Collections.Generic.KeyValuePair<TKey, TValue> item) { }
        public bool Remove(TKey keyToRemove) { }
        public virtual void RemoveAllItems() { }
        public void RemoveAt(int index) { }
        public virtual void RemoveMultipleValues(System.Collections.Generic.IEnumerable<TKey> keysToRemove) { }
        public virtual void RemoveMultipleValues(int startIndex, int count) { }
        public virtual void RemoveSingleValue(int index, out TValue value) { }
        public void Reset() { }
        public System.IDisposable SuspendChangeNotifications() { }
        public System.IDisposable SuspendChangeNotifications(Catel.Collections.SuspensionMode mode) { }
        public bool TryGetValue(TKey key, out TValue value) { }
        public virtual bool TryRemoveSingleValue(TKey keyToRemove, out TValue? value) { }
    }
    public static class HashSetExtensions
    {
        public static void AddRange<T>(this System.Collections.Generic.HashSet<T> hashSet, System.Collections.Generic.IEnumerable<T> items) { }
    }
    public interface ISuspendChangeNotificationsCollection : System.Collections.ICollection, System.Collections.IEnumerable
    {
        bool IsDirty { get; }
        bool NotificationsSuspended { get; }
        void Reset();
        System.IDisposable SuspendChangeNotifications();
        System.IDisposable SuspendChangeNotifications(Catel.Collections.SuspensionMode mode);
    }
    public class ListDictionary<TKey, TValue> : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.IEnumerable
    {
        public ListDictionary() { }
        public ListDictionary(System.Collections.Generic.IEqualityComparer<TKey> comparer) { }
        public int Count { get; }
        public bool IsReadOnly { get; }
        public TValue this[TKey key] { get; set; }
        public System.Collections.Generic.ICollection<TKey> Keys { get; }
        public System.Collections.Generic.ICollection<TValue> Values { get; }
        public void Add(System.Collections.Generic.KeyValuePair<TKey, TValue> item) { }
        public void Add(TKey key, TValue value) { }
        public void Clear() { }
        public bool Contains(System.Collections.Generic.KeyValuePair<TKey, TValue> item) { }
        public bool ContainsKey(TKey key) { }
        public void CopyTo(System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int arrayIndex) { }
        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> GetEnumerator() { }
        public bool Remove(System.Collections.Generic.KeyValuePair<TKey, TValue> item) { }
        public bool Remove(TKey key) { }
        public bool TryGetValue(TKey key, out TValue value) { }
    }
    public class NotifyListChangedEventArgs : System.ComponentModel.ListChangedEventArgs
    {
        public NotifyListChangedEventArgs(System.ComponentModel.ListChangedType listChangedType) { }
        public NotifyListChangedEventArgs(System.ComponentModel.ListChangedType listChangedType, System.ComponentModel.PropertyDescriptor? propDesc) { }
        public NotifyListChangedEventArgs(System.ComponentModel.ListChangedType listChangedType, int newIndex) { }
        public NotifyListChangedEventArgs(System.ComponentModel.ListChangedType listChangedType, int newIndex, System.ComponentModel.PropertyDescriptor? propDesc) { }
        public NotifyListChangedEventArgs(System.ComponentModel.ListChangedType listChangedType, int newIndex, int oldIndex) { }
        public NotifyListChangedEventArgs(System.ComponentModel.ListChangedType listChangedType, int newIndex, object? newItem) { }
        public NotifyListChangedEventArgs(System.ComponentModel.ListChangedType listChangedType, int newIndex, object? newItem, System.ComponentModel.PropertyDescriptor? propDesc) { }
        public NotifyListChangedEventArgs(System.ComponentModel.ListChangedType listChangedType, int newIndex, object? newItem, int oldIndex, object? oldItem) { }
        public object? NewItem { get; }
        public object? OldItem { get; }
    }
    public class NotifyRangedCollectionChangedEventArgs : System.Collections.Specialized.NotifyCollectionChangedEventArgs
    {
        public NotifyRangedCollectionChangedEventArgs() { }
        public NotifyRangedCollectionChangedEventArgs(System.Collections.IList changedItems, System.Collections.Generic.IList<int> indices, Catel.Collections.SuspensionMode mode) { }
        public NotifyRangedCollectionChangedEventArgs(System.Collections.IList changedItems, System.Collections.Generic.IList<int> indices, System.Collections.Generic.IList<System.Collections.Specialized.NotifyCollectionChangedAction> mixedActions) { }
        public NotifyRangedCollectionChangedEventArgs(System.Collections.IList changedItems, System.Collections.Generic.IList<int> indices, Catel.Collections.SuspensionMode mode, System.Collections.Specialized.NotifyCollectionChangedAction action) { }
        public System.Collections.IList? ChangedItems { get; }
        public System.Collections.Generic.IList<int>? Indices { get; }
        public System.Collections.Generic.IList<System.Collections.Specialized.NotifyCollectionChangedAction>? MixedActions { get; }
        public Catel.Collections.SuspensionMode SuspensionMode { get; }
    }
    public enum NotifyRangedListChangedAction
    {
        Add = 0,
        Remove = 1,
        Reset = 2,
    }
    public class NotifyRangedListChangedEventArgs : Catel.Collections.NotifyListChangedEventArgs
    {
        public NotifyRangedListChangedEventArgs(Catel.Collections.NotifyRangedListChangedAction action) { }
        public NotifyRangedListChangedEventArgs(Catel.Collections.NotifyRangedListChangedAction action, System.Collections.IList changedItems, System.Collections.Generic.IList<int> indices) { }
        public Catel.Collections.NotifyRangedListChangedAction Action { get; }
        public System.Collections.Generic.IList<int>? Indices { get; }
        public System.Collections.IList? NewItems { get; }
        public int NewStartingIndex { get; }
        public System.Collections.IList? OldItems { get; }
        public int OldStartingIndex { get; }
    }
    public static class SuspensionContextExtensions
    {
        public static System.Collections.Generic.ICollection<Catel.Collections.NotifyRangedCollectionChangedEventArgs> CreateAddingEvents<T>(this Catel.Collections.SuspensionContext<T> suspensionContext) { }
        public static System.Collections.Generic.ICollection<Catel.Collections.NotifyRangedCollectionChangedEventArgs> CreateEvents<T>(this Catel.Collections.SuspensionContext<T> suspensionContext) { }
        public static System.Collections.Generic.ICollection<Catel.Collections.NotifyRangedCollectionChangedEventArgs> CreateMixedBashEvents<T>(this Catel.Collections.SuspensionContext<T> suspensionContext) { }
        public static System.Collections.Generic.ICollection<Catel.Collections.NotifyRangedCollectionChangedEventArgs> CreateMixedConsolidateEvents<T>(this Catel.Collections.SuspensionContext<T> suspensionContext) { }
        public static System.Collections.Generic.ICollection<Catel.Collections.NotifyRangedCollectionChangedEventArgs> CreateMixedEvents<T>(this Catel.Collections.SuspensionContext<T> suspensionContext) { }
        public static System.Collections.Generic.ICollection<Catel.Collections.NotifyRangedCollectionChangedEventArgs> CreateNoneEvents<T>(this Catel.Collections.SuspensionContext<T> suspensionContext) { }
        public static System.Collections.Generic.ICollection<Catel.Collections.NotifyRangedCollectionChangedEventArgs> CreateRemovingEvents<T>(this Catel.Collections.SuspensionContext<T> suspensionContext) { }
        public static System.Collections.Generic.ICollection<Catel.Collections.NotifyRangedCollectionChangedEventArgs> CreateSilentEvents<T>(this Catel.Collections.SuspensionContext<T> suspensionContext) { }
        public static bool IsMixedMode<T>(this Catel.Collections.SuspensionContext<T> suspensionContext) { }
    }
    public class SuspensionContext<T>
    {
        public SuspensionContext(Catel.Collections.SuspensionMode mode) { }
        public System.Collections.Generic.List<int> ChangedItemIndices { get; }
        public System.Collections.Generic.List<T> ChangedItems { get; }
        public int Count { get; set; }
        public System.Collections.Generic.List<System.Collections.Specialized.NotifyCollectionChangedAction> MixedActions { get; }
        public Catel.Collections.SuspensionMode Mode { get; }
        public static System.Lazy<System.Collections.Generic.Dictionary<Catel.Collections.SuspensionMode, System.Func<Catel.Collections.SuspensionContext<T>, System.Collections.Generic.ICollection<Catel.Collections.NotifyRangedCollectionChangedEventArgs>>>> EventsGeneratorsRegistry { get; }
    }
    public enum SuspensionMode
    {
        None = 0,
        Adding = 1,
        Removing = 2,
        Mixed = 3,
        MixedBash = 4,
        MixedConsolidate = 5,
        Silent = 6,
    }
    public static class SuspensionModeExtensions
    {
        public static bool IsMixedMode(this Catel.Collections.SuspensionMode suspensionMode) { }
    }
}
namespace Catel.ComponentModel
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Enum | System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field | System.AttributeTargets.Event)]
    public class DisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
    {
        public DisplayNameAttribute(string resourceName) { }
        public override string DisplayName { get; }
        public Catel.Services.ILanguageService LanguageService { get; set; }
        public string ResourceName { get; }
    }
}
namespace Catel.Configuration
{
    public class ConfigurationChangedEventArgs : System.EventArgs
    {
        public ConfigurationChangedEventArgs(Catel.Configuration.ConfigurationContainer container, string key, object? newValue) { }
        public Catel.Configuration.ConfigurationContainer Container { get; }
        public string Key { get; }
        public object? NewValue { get; }
    }
    public enum ConfigurationContainer
    {
        Local = 0,
        Roaming = 1,
    }
    public static class ConfigurationExtensions
    {
        public static TSection? GetSection<TSection>(this System.Configuration.Configuration @this, string sectionName, string? sectionGroupName = null)
            where TSection : System.Configuration.ConfigurationSection { }
        public static bool IsConfigurationKey(this Catel.Configuration.ConfigurationChangedEventArgs eventArgs, string expectedKey) { }
        public static bool IsConfigurationKey(this string key, string expectedKey) { }
    }
    public class ConfigurationService : Catel.Configuration.IConfigurationService
    {
        public ConfigurationService(Catel.Services.IObjectConverterService objectConverterService, Catel.Runtime.Serialization.ISerializer serializer, Catel.Services.IAppDataService appDataService) { }
        public ConfigurationService(Catel.Services.IObjectConverterService objectConverterService, Catel.Runtime.Serialization.Xml.IXmlSerializer serializer, Catel.Services.IAppDataService appDataService) { }
        public event System.EventHandler<Catel.Configuration.ConfigurationChangedEventArgs>? ConfigurationChanged;
        protected virtual string GetConfigurationFileName(Catel.IO.ApplicationDataTarget applicationDataTarget) { }
        protected virtual string GetFinalKey(string key) { }
        protected Catel.Threading.AsyncLock GetLockObject(Catel.Configuration.ConfigurationContainer container) { }
        protected virtual double GetSaveSettingsSchedulerIntervalInMilliseconds() { }
        protected virtual Catel.Configuration.DynamicConfiguration GetSettingsContainer(Catel.Configuration.ConfigurationContainer container) { }
        public virtual T GetValue<T>(Catel.Configuration.ConfigurationContainer container, string key, T defaultValue = default) { }
        protected virtual string GetValueFromStore(Catel.Configuration.ConfigurationContainer container, string key) { }
        public virtual void InitializeValue(Catel.Configuration.ConfigurationContainer container, string key, object? defaultValue) { }
        public virtual bool IsValueAvailable(Catel.Configuration.ConfigurationContainer container, string key) { }
        public virtual System.Threading.Tasks.Task LoadAsync(Catel.Configuration.ConfigurationContainer configuration) { }
        protected virtual System.Threading.Tasks.Task<Catel.Configuration.DynamicConfiguration> LoadConfigurationAsync(string source) { }
        protected void RaiseConfigurationChanged(Catel.Configuration.ConfigurationContainer container, string key, object? value) { }
        public virtual System.Threading.Tasks.Task SaveAsync(Catel.Configuration.ConfigurationContainer configuration) { }
        protected virtual System.Threading.Tasks.Task SaveConfigurationAsync(Catel.Configuration.ConfigurationContainer container, Catel.Configuration.DynamicConfiguration configuration, string fileName) { }
        protected void ScheduleLocalConfigurationSave() { }
        protected void ScheduleRoamingConfigurationSave() { }
        protected virtual void ScheduleSaveConfiguration(Catel.Configuration.ConfigurationContainer container) { }
        public virtual System.Threading.Tasks.Task SetLocalConfigFilePathAsync(string filePath) { }
        public virtual System.Threading.Tasks.Task SetRoamingConfigFilePathAsync(string filePath) { }
        public virtual void SetValue(Catel.Configuration.ConfigurationContainer container, string key, object? value) { }
        protected virtual void SetValueToStore(Catel.Configuration.ConfigurationContainer container, string key, string value) { }
        public System.IDisposable SuspendNotifications() { }
        protected virtual bool ValueExists(Catel.Configuration.ConfigurationContainer container, string key) { }
    }
    [Catel.Runtime.Serialization.SerializerModifier(typeof(Catel.Configuration.DynamicConfigurationSerializerModifier))]
    public class DynamicConfiguration : Catel.Data.ModelBase, Catel.Runtime.Serialization.Xml.ICustomXmlSerializable
    {
        protected static readonly System.Collections.Generic.HashSet<string> DynamicProperties;
        public DynamicConfiguration() { }
        protected override Catel.Data.IPropertyBag CreatePropertyBag() { }
        public virtual void Deserialize(System.Xml.XmlReader xmlReader) { }
        public virtual object? GetConfigurationValue(string name) { }
        protected virtual Catel.Runtime.Serialization.Xml.IXmlSerializer GetXmlSerializer() { }
        public virtual bool IsConfigurationValueSet(string name) { }
        public virtual void MarkConfigurationValueAsSet(string name) { }
        public virtual void RegisterConfigurationKey(string name) { }
        public virtual void Serialize(System.Xml.XmlWriter xmlWriter) { }
        public virtual void SetConfigurationValue(string name, object? value) { }
    }
    public static class DynamicConfigurationExtensions
    {
        public static TValue GetConfigurationValue<TValue>(this Catel.Configuration.DynamicConfiguration dynamicConfiguration, string name, TValue defaultValue) { }
    }
    public class DynamicConfigurationSerializerModifier : Catel.Runtime.Serialization.SerializerModifierBase
    {
        public DynamicConfigurationSerializerModifier(Catel.Runtime.Serialization.ISerializationManager serializationManager) { }
        public override void OnSerializing(Catel.Runtime.Serialization.ISerializationContext context, object model) { }
    }
    public interface IConfigurationService
    {
        event System.EventHandler<Catel.Configuration.ConfigurationChangedEventArgs>? ConfigurationChanged;
        T GetValue<T>(Catel.Configuration.ConfigurationContainer container, string key, T defaultValue = default);
        void InitializeValue(Catel.Configuration.ConfigurationContainer container, string key, object? defaultValue);
        bool IsValueAvailable(Catel.Configuration.ConfigurationContainer container, string key);
        System.Threading.Tasks.Task LoadAsync(Catel.Configuration.ConfigurationContainer container);
        System.Threading.Tasks.Task SaveAsync(Catel.Configuration.ConfigurationContainer container);
        System.Threading.Tasks.Task SetLocalConfigFilePathAsync(string filePath);
        System.Threading.Tasks.Task SetRoamingConfigFilePathAsync(string filePath);
        void SetValue(Catel.Configuration.ConfigurationContainer container, string key, object? value);
        System.IDisposable SuspendNotifications();
    }
    public static class IConfigurationServiceExtensions
    {
        public static T GetLocalValue<T>(this Catel.Configuration.IConfigurationService configurationService, string key, T defaultValue = default) { }
        public static T GetRoamingValue<T>(this Catel.Configuration.IConfigurationService configurationService, string key, T defaultValue = default) { }
        public static void InitializeLocalValue(this Catel.Configuration.IConfigurationService configurationService, string key, object? defaultValue) { }
        public static void InitializeRoamingValue(this Catel.Configuration.IConfigurationService configurationService, string key, object? defaultValue) { }
        public static bool IsLocalValueAvailable(this Catel.Configuration.IConfigurationService configurationService, string key) { }
        public static bool IsRoamingValueAvailable(this Catel.Configuration.IConfigurationService configurationService, string key) { }
        public static System.Threading.Tasks.Task LoadAsync(this Catel.Configuration.IConfigurationService configurationService) { }
        public static System.Threading.Tasks.Task SaveAsync(this Catel.Configuration.IConfigurationService configurationService) { }
        public static void SetLocalValue(this Catel.Configuration.IConfigurationService configurationService, string key, object? value) { }
        public static void SetRoamingValue(this Catel.Configuration.IConfigurationService configurationService, string key, object? value) { }
    }
}
namespace Catel.Core
{
    public static class ModuleInitializer
    {
        public static void Initialize() { }
    }
}
namespace Catel.Data
{
    public class AttributeValidatorProvider : Catel.Data.ValidatorProviderBase
    {
        public AttributeValidatorProvider() { }
        protected override Catel.Data.IValidator? GetValidator(System.Type targetType) { }
    }
    public static class BoxingCache
    {
        public static object? GetBoxedValue(bool value) { }
        public static object? GetBoxedValue(byte value) { }
        public static object? GetBoxedValue(char value) { }
        public static object? GetBoxedValue(System.Char? value) { }
        public static object? GetBoxedValue(System.DateTime value) { }
        public static object? GetBoxedValue(System.DateTime? value) { }
        public static object? GetBoxedValue(decimal value) { }
        public static object? GetBoxedValue(double value) { }
        public static object? GetBoxedValue(short value) { }
        public static object? GetBoxedValue(int value) { }
        public static object? GetBoxedValue(long value) { }
        public static object? GetBoxedValue(sbyte value) { }
        public static object? GetBoxedValue(float value) { }
        public static object? GetBoxedValue(string value) { }
        public static object? GetBoxedValue(ushort value) { }
        public static object? GetBoxedValue(uint value) { }
        public static object? GetBoxedValue(ulong value) { }
        public static object? GetBoxedValue(bool? value) { }
        public static object? GetBoxedValue(byte? value) { }
        public static object? GetBoxedValue(decimal? value) { }
        public static object? GetBoxedValue(double? value) { }
        public static object? GetBoxedValue(float? value) { }
        public static object? GetBoxedValue(int? value) { }
        public static object? GetBoxedValue(long? value) { }
        public static object? GetBoxedValue(object? value) { }
        public static object? GetBoxedValue(sbyte? value) { }
        public static object? GetBoxedValue(short? value) { }
        public static object? GetBoxedValue(uint? value) { }
        public static object? GetBoxedValue(ulong? value) { }
        public static object? GetBoxedValue(ushort? value) { }
        public static object? GetBoxedValue<TValue>(TValue value)
            where TValue :  notnull { }
    }
    public class BoxingCache<T>
        where T :  notnull
    {
        public BoxingCache() { }
        public System.TimeSpan CleanUpInterval { get; set; }
        public static Catel.Data.BoxingCache<T> Default { get; }
        protected T AddBoxedValue(object? boxedValue) { }
        protected object? AddUnboxedValue(T value) { }
        public void CleanUp() { }
        public object? GetBoxedValue(T value) { }
        public T GetUnboxedValue(object boxedValue) { }
    }
    public class BusinessRuleValidationResult : Catel.Data.ValidationResult, Catel.Data.IBusinessRuleValidationResult, Catel.Data.IValidationResult
    {
        public BusinessRuleValidationResult(Catel.Data.ValidationResultType validationResultType, string messageFormat, params object[] args) { }
        public override string ToString() { }
        public static Catel.Data.BusinessRuleValidationResult CreateError(string messageFormat, params object[] args) { }
        public static Catel.Data.BusinessRuleValidationResult CreateErrorWithTag(string message, object tag) { }
        public static Catel.Data.BusinessRuleValidationResult CreateWarning(string messageFormat, params object[] args) { }
        public static Catel.Data.BusinessRuleValidationResult CreateWarningWithTag(string message, object tag) { }
    }
    public class CatelTypeInfo
    {
        public CatelTypeInfo(System.Type type) { }
        public bool IsRegisterPropertiesCalled { get; }
        public System.Type Type { get; }
        public System.Collections.Generic.IDictionary<string, Catel.Data.IPropertyData> GetCatelProperties() { }
        public System.Collections.Generic.IDictionary<string, Catel.Reflection.CachedPropertyInfo> GetNonCatelProperties() { }
        public Catel.Data.IPropertyData GetPropertyData(string name) { }
        public bool IsPropertyRegistered(string name) { }
        public void RegisterProperties() { }
        public void RegisterProperty(string name, Catel.Data.IPropertyData propertyData) { }
        public void UnregisterProperty(string name) { }
    }
    public class ChangeNotificationWrapper
    {
        public ChangeNotificationWrapper(object value) { }
        public bool IsObjectAlive { get; }
        public bool SupportsNotifyCollectionChanged { get; }
        public bool SupportsNotifyPropertyChanged { get; }
        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler? CollectionChanged;
        public event System.ComponentModel.PropertyChangedEventHandler? CollectionItemPropertyChanged;
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        public void OnObjectCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) { }
        public void OnObjectCollectionItemPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) { }
        public void OnObjectPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) { }
        public void SubscribeNotifyChangedEvents(object? value, System.Collections.ICollection? parentCollection) { }
        public void UnsubscribeFromAllEvents() { }
        public void UnsubscribeNotifyChangedEvents(object? value, System.Collections.ICollection? parentCollection) { }
        public void UpdateCollectionSubscriptions(System.Collections.ICollection collection) { }
        public static bool IsUsefulForObject(object? obj) { }
    }
    public abstract class ChildAwareModelBase : Catel.Data.ValidatableModelBase
    {
        protected ChildAwareModelBase() { }
        protected bool DisableEventSubscriptionsOfChildValues { get; set; }
        [System.ComponentModel.Browsable(false)]
        protected bool HandlePropertyAndCollectionChanges { get; set; }
        public static bool DefaultDisableEventSubscriptionsOfChildValuesValue { get; set; }
        protected virtual void OnPropertyObjectCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) { }
        protected virtual void OnPropertyObjectCollectionItemPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) { }
        protected virtual void OnPropertyObjectPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) { }
        protected override void SetValueToPropertyBag<TValue>(string propertyName, TValue value) { }
    }
    public abstract class ComparableModelBase : Catel.Data.ModelBase
    {
        protected ComparableModelBase() { }
        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        protected Catel.Data.IModelEqualityComparer EqualityComparer { get; set; }
        public override bool Equals(object? obj) { }
        public override int GetHashCode() { }
        public static bool operator !=(Catel.Data.ComparableModelBase firstObject, Catel.Data.ComparableModelBase secondObject) { }
        public static bool operator ==(Catel.Data.ComparableModelBase firstObject, Catel.Data.ComparableModelBase secondObject) { }
    }
    public sealed class CompositeValidator : Catel.Data.IValidator
    {
        public CompositeValidator() { }
        public void Add(Catel.Data.IValidator validator) { }
        public void AfterValidateBusinessRules(object instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> validationResults) { }
        public void AfterValidateFields(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> validationResults) { }
        public void AfterValidation(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> fieldValidationResults, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> businessRuleValidationResults) { }
        public void BeforeValidateBusinessRules(object instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> previousValidationResults) { }
        public void BeforeValidateFields(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> previousValidationResults) { }
        public void BeforeValidation(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> previousFieldValidationResults, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> previousBusinessRuleValidationResults) { }
        public bool Contains(Catel.Data.IValidator validator) { }
        public void Remove(Catel.Data.IValidator validator) { }
        public void Validate(object instance, Catel.Data.ValidationContext validationContext) { }
        public void ValidateBusinessRules(object instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> validationResults) { }
        public void ValidateFields(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> validationResults) { }
    }
    public class CompositeValidatorProvider : Catel.Data.ValidatorProviderBase
    {
        public CompositeValidatorProvider() { }
        public void Add(Catel.Data.IValidatorProvider validatorProvider) { }
        public bool Contains(Catel.Data.IValidatorProvider validatorProvider) { }
        protected override Catel.Data.IValidator? GetValidator(System.Type targetType) { }
        public void Remove(Catel.Data.IValidatorProvider validatorProvider) { }
    }
    public static class EditableObjectHelper
    {
        public static void BeginEditObject(object obj) { }
        public static void CancelEditObject(object obj) { }
        public static void EndEditObject(object obj) { }
    }
    public enum EventChangeType
    {
        Property = 0,
        Collection = 1,
    }
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class ExcludeFromValidationAttribute : System.Attribute
    {
        public ExcludeFromValidationAttribute() { }
    }
    public class ExpressionTreeObjectAdapter : Catel.Data.IObjectAdapter
    {
        public ExpressionTreeObjectAdapter() { }
        protected virtual Catel.Reflection.IFastMemberInvoker GetFastMemberInvoker(System.Type modelType) { }
        public virtual bool TryGetMemberValue<TValue>(object instance, string memberName, out TValue? value) { }
        public virtual bool TrySetMemberValue<TValue>(object instance, string memberName, TValue? value) { }
    }
    public class FieldValidationResult : Catel.Data.ValidationResult, Catel.Data.IFieldValidationResult, Catel.Data.IValidationResult
    {
        public FieldValidationResult(Catel.Data.IPropertyData property, Catel.Data.ValidationResultType validationResultType, string messageFormat, params object[] args) { }
        public FieldValidationResult(string propertyName, Catel.Data.ValidationResultType validationResultType, string messageFormat, params object[] args) { }
        public string PropertyName { get; }
        public override string ToString() { }
        public static Catel.Data.FieldValidationResult CreateError(Catel.Data.IPropertyData propertyData, string messageFormat, params object[] args) { }
        public static Catel.Data.FieldValidationResult CreateError(string propertyName, string messageFormat, params object[] args) { }
        public static Catel.Data.FieldValidationResult CreateError<TProperty>(System.Linq.Expressions.Expression<System.Func<TProperty>> propertyExpression, string messageFormat, params object[] args) { }
        public static Catel.Data.FieldValidationResult CreateErrorWithTag(Catel.Data.IPropertyData propertyData, string message, object tag) { }
        public static Catel.Data.FieldValidationResult CreateErrorWithTag(string propertyName, string message, object tag) { }
        public static Catel.Data.FieldValidationResult CreateErrorWithTag<TProperty>(System.Linq.Expressions.Expression<System.Func<TProperty>> propertyExpression, string message, object tag) { }
        public static Catel.Data.FieldValidationResult CreateWarning(Catel.Data.IPropertyData propertyData, string messageFormat, params object[] args) { }
        public static Catel.Data.FieldValidationResult CreateWarning(string propertyName, string messageFormat, params object[] args) { }
        public static Catel.Data.FieldValidationResult CreateWarning<TProperty>(System.Linq.Expressions.Expression<System.Func<TProperty>> propertyExpression, string messageFormat, params object[] args) { }
        public static Catel.Data.FieldValidationResult CreateWarningWithTag(Catel.Data.IPropertyData propertyData, string message, object tag) { }
        public static Catel.Data.FieldValidationResult CreateWarningWithTag(string propertyName, string message, object tag) { }
        public static Catel.Data.FieldValidationResult CreateWarningWithTag<TProperty>(System.Linq.Expressions.Expression<System.Func<TProperty>> propertyExpression, string message, object tag) { }
    }
    public interface IBusinessRuleValidationResult : Catel.Data.IValidationResult { }
    public interface IFieldValidationResult : Catel.Data.IValidationResult
    {
        string PropertyName { get; }
    }
    public interface IFreezable
    {
        bool IsFrozen { get; }
        void Freeze();
        void Unfreeze();
    }
    public interface IModel : Catel.Data.IFreezable, Catel.Data.IModelEditor, Catel.Data.IModelSerialization, Catel.Runtime.Serialization.ISerializable, System.ComponentModel.IAdvancedEditableObject, System.ComponentModel.IEditableObject, System.ComponentModel.INotifyPropertyChanged, System.Xml.Serialization.IXmlSerializable
    {
        bool IsDirty { get; }
        bool IsInEditSession { get; }
        string KeyName { get; }
        object? GetDefaultValue(string name);
        TValue GetDefaultValue<TValue>(string name);
        System.Type GetPropertyType(string name);
    }
    public interface IModelEditor
    {
        TValue GetValue<TValue>(string propertyName);
        TValue GetValueFastButUnsecure<TValue>(string propertyName);
        bool IsPropertyRegistered(string propertyName);
        void SetValue<TValue>(string propertyName, TValue value);
        void SetValueFastButUnsecure<TValue>(string propertyName, TValue value);
    }
    public interface IModelEqualityComparer : System.Collections.IEqualityComparer
    {
        bool CompareCollections { get; set; }
        bool CompareProperties { get; set; }
        bool CompareValues { get; set; }
    }
    public static class IModelExtensions
    {
        public static void ClearIsDirtyOnAllChildren(this Catel.Data.IModel model, bool suspendNotifications = false) { }
        public static byte[] ToByteArray(this Catel.Data.IModel model, Catel.Runtime.Serialization.ISerializer serializer, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public static System.Xml.Linq.XDocument ToXml(this Catel.Data.IModel model, Catel.Runtime.Serialization.ISerializer serializer, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
    }
    public interface IModelSerialization : Catel.Runtime.Serialization.ISerializable, System.Xml.Serialization.IXmlSerializable { }
    public interface IObjectAdapter
    {
        bool TryGetMemberValue<TValue>(object instance, string memberName, out TValue? value);
        bool TrySetMemberValue<TValue>(object instance, string memberName, TValue? value);
    }
    public interface IPropertyBag : System.ComponentModel.INotifyPropertyChanged
    {
        string[] GetAllNames();
        System.Collections.Generic.Dictionary<string, object?> GetAllProperties();
        TValue GetValue<TValue>(string name, TValue defaultValue = default);
        bool IsAvailable(string name);
        void SetValue<TValue>(string name, TValue value);
    }
    public interface IPropertyData
    {
        bool IncludeInBackup { get; }
        bool IncludeInSerialization { get; }
        bool IsCalculatedProperty { get; set; }
        bool IsModelBaseProperty { get; }
        bool IsSerializable { get; }
        string Name { get; }
        System.EventHandler<System.ComponentModel.PropertyChangedEventArgs>? PropertyChangedEventHandler { get; }
        System.Type Type { get; }
        object? GetDefaultValue();
        TValue GetDefaultValue<TValue>();
        Catel.Reflection.CachedPropertyInfo? GetPropertyInfo(System.Type containingType);
    }
    public interface ISavableModel : Catel.Data.IFreezable, Catel.Data.IModel, Catel.Data.IModelEditor, Catel.Data.IModelSerialization, Catel.Runtime.Serialization.ISerializable, System.ComponentModel.IAdvancedEditableObject, System.ComponentModel.IEditableObject, System.ComponentModel.INotifyPropertyChanged, System.Xml.Serialization.IXmlSerializable
    {
        void Save(System.IO.Stream stream, Catel.Runtime.Serialization.ISerializer serializer, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
    }
    public static class ISavableModelExtensions
    {
        public static void Save(this Catel.Data.ISavableModel model, string fileName, Catel.Runtime.Serialization.ISerializer serializer, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
    }
    public interface IValidatable : System.ComponentModel.IDataErrorInfo, System.ComponentModel.IDataWarningInfo, System.ComponentModel.INotifyDataErrorInfo, System.ComponentModel.INotifyDataWarningInfo
    {
        bool IsHidingValidationResults { get; }
        bool IsValidated { get; }
        Catel.Data.IValidationContext ValidationContext { get; }
        Catel.Data.IValidator? Validator { get; set; }
        event System.EventHandler<Catel.Data.ValidationEventArgs>? Validated;
        event System.EventHandler<Catel.Data.ValidationEventArgs>? Validating;
        void Validate(bool force = false);
    }
    public static class IValidatableExtensions
    {
        public static void Add(this Catel.Data.IValidatable validatable, Catel.Data.IBusinessRuleValidationResult businessRuleValidationResult, bool validate = false) { }
        public static void Add(this Catel.Data.IValidatable validatable, Catel.Data.IFieldValidationResult fieldValidationResult, bool validate = false) { }
        public static string GetBusinessRuleErrors(this Catel.Data.IValidatable validatable) { }
        public static string GetBusinessRuleWarnings(this Catel.Data.IValidatable validatable) { }
        public static string GetErrorMessage(this Catel.Data.IValidatable validatable, string? userFriendlyObjectName = null) { }
        public static string GetFieldErrors(this Catel.Data.IValidatable validatable, string columnName) { }
        public static string GetFieldWarnings(this Catel.Data.IValidatable validatable, string columnName) { }
        public static Catel.Data.IValidationContext GetValidationContext(this Catel.Data.IValidatable validatable) { }
        public static string GetWarningMessage(this Catel.Data.IValidatable validatable, string? userFriendlyObjectName = null) { }
    }
    public interface IValidatableModel : Catel.Data.IFreezable, Catel.Data.IModel, Catel.Data.IModelEditor, Catel.Data.IModelSerialization, Catel.Data.IValidatable, Catel.Runtime.Serialization.ISerializable, System.ComponentModel.IAdvancedEditableObject, System.ComponentModel.IDataErrorInfo, System.ComponentModel.IDataWarningInfo, System.ComponentModel.IEditableObject, System.ComponentModel.INotifyDataErrorInfo, System.ComponentModel.INotifyDataWarningInfo, System.ComponentModel.INotifyPropertyChanged, System.Xml.Serialization.IXmlSerializable { }
    public static class IValidatableModelExtensions
    {
        public static Catel.Data.IValidationContext GetValidationContextForObjectGraph(this Catel.Data.IValidatableModel model) { }
    }
    public interface IValidationContext
    {
        bool HasErrors { get; }
        bool HasWarnings { get; }
        System.DateTime LastModified { get; }
        long LastModifiedTicks { get; }
        void Add(Catel.Data.IBusinessRuleValidationResult businessRuleValidationResult);
        void Add(Catel.Data.IFieldValidationResult fieldValidationResult);
        int GetBusinessRuleErrorCount();
        int GetBusinessRuleErrorCount(object? tag);
        System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleErrors();
        System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleErrors(object? tag);
        int GetBusinessRuleValidationCount();
        int GetBusinessRuleValidationCount(object? tag);
        System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleValidations();
        System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleValidations(object? tag);
        int GetBusinessRuleWarningCount();
        int GetBusinessRuleWarningCount(object? tag);
        System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleWarnings();
        System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleWarnings(object? tag);
        int GetErrorCount();
        int GetErrorCount(object? tag);
        System.Collections.Generic.List<Catel.Data.IValidationResult> GetErrors();
        System.Collections.Generic.List<Catel.Data.IValidationResult> GetErrors(object? tag);
        int GetFieldErrorCount();
        int GetFieldErrorCount(object? tag);
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldErrors();
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldErrors(string propertyName);
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldErrors(object? tag);
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldErrors(string propertyName, object? tag);
        int GetFieldValidationCount();
        int GetFieldValidationCount(object? tag);
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldValidations();
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldValidations(string propertyName);
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldValidations(object? tag);
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldValidations(string propertyName, object? tag);
        int GetFieldWarningCount();
        int GetFieldWarningCount(object? tag);
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldWarnings();
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldWarnings(string propertyName);
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldWarnings(object? tag);
        System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldWarnings(string propertyName, object? tag);
        int GetValidationCount();
        int GetValidationCount(object? tag);
        System.Collections.Generic.List<Catel.Data.IValidationResult> GetValidations();
        System.Collections.Generic.List<Catel.Data.IValidationResult> GetValidations(object? tag);
        int GetWarningCount();
        int GetWarningCount(object? tag);
        System.Collections.Generic.List<Catel.Data.IValidationResult> GetWarnings();
        System.Collections.Generic.List<Catel.Data.IValidationResult> GetWarnings(object? tag);
        void Remove(Catel.Data.IBusinessRuleValidationResult businessRuleValidationResult);
        void Remove(Catel.Data.IFieldValidationResult fieldValidationResult);
    }
    public static class IValidationContextExtensions
    {
        public static string GetValidationsAsStringList(this Catel.Data.IValidationContext validationContext, Catel.Data.ValidationResultType validationResult) { }
        public static bool HasWarningsOrErrors(this Catel.Data.IValidationContext validationContext) { }
    }
    public interface IValidationResult
    {
        string Message { get; set; }
        object? Tag { get; set; }
        Catel.Data.ValidationResultType ValidationResultType { get; }
    }
    public interface IValidationSummary
    {
        System.Collections.ObjectModel.ReadOnlyCollection<Catel.Data.IBusinessRuleValidationResult> BusinessRuleErrors { get; }
        System.Collections.ObjectModel.ReadOnlyCollection<Catel.Data.IBusinessRuleValidationResult> BusinessRuleWarnings { get; }
        System.Collections.ObjectModel.ReadOnlyCollection<Catel.Data.IFieldValidationResult> FieldErrors { get; }
        System.Collections.ObjectModel.ReadOnlyCollection<Catel.Data.IFieldValidationResult> FieldWarnings { get; }
        bool HasBusinessRuleErrors { get; }
        bool HasBusinessRuleWarnings { get; }
        bool HasErrors { get; }
        bool HasFieldErrors { get; }
        bool HasFieldWarnings { get; }
        bool HasWarnings { get; }
        System.DateTime LastModified { get; }
        long LastModifiedTicks { get; }
    }
    public interface IValidator
    {
        void AfterValidateBusinessRules(object instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> validationResults);
        void AfterValidateFields(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> validationResults);
        void AfterValidation(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> fieldValidationResults, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> businessRuleValidationResults);
        void BeforeValidateBusinessRules(object instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> previousValidationResults);
        void BeforeValidateFields(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> previousValidationResults);
        void BeforeValidation(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> previousFieldValidationResults, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> previousBusinessRuleValidationResults);
        void Validate(object instance, Catel.Data.ValidationContext validationContext);
        void ValidateBusinessRules(object instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> validationResults);
        void ValidateFields(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> validationResults);
    }
    public interface IValidatorProvider
    {
        Catel.Data.IValidator? GetValidator(System.Type targetType);
        Catel.Data.IValidator? GetValidator<TTargetType>();
    }
    public interface IValueValidator<in TValue>
    {
        bool IsValid(TValue value);
    }
    public class InvalidPropertyException : System.Exception
    {
        public InvalidPropertyException(string propertyName) { }
        public string PropertyName { get; }
    }
    public class InvalidPropertyValueException : System.Exception
    {
        public InvalidPropertyValueException(string propertyName, System.Type expectedType, System.Type actualType) { }
        public System.Type ActualType { get; }
        public System.Type ExpectedType { get; }
        public string PropertyName { get; }
    }
    [System.Serializable]
    public abstract class ModelBase : Catel.Data.ObservableObject, Catel.Data.IFreezable, Catel.Data.IModel, Catel.Data.IModelEditor, Catel.Data.IModelSerialization, Catel.Runtime.Serialization.ISerializable, System.ComponentModel.IAdvancedEditableObject, System.ComponentModel.IEditableObject, System.ComponentModel.INotifyPropertyChanged, System.Xml.Serialization.IXmlSerializable
    {
        public static readonly Catel.Data.IPropertyData IsDirtyProperty;
        public static readonly Catel.Data.IPropertyData IsReadOnlyProperty;
        protected ModelBase() { }
        [System.ComponentModel.Browsable(false)]
        protected bool AlwaysInvokeNotifyChanged { get; set; }
        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public bool IsDirty { get; set; }
        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public bool IsReadOnly { get; set; }
        protected virtual Catel.Data.IPropertyBag CreatePropertyBag() { }
        protected Catel.Data.IPropertyData GetPropertyData(string name) { }
        protected System.Func<object, TValue>? GetPropertyGetterExpression<TValue>(string propertyName) { }
        protected virtual Catel.Runtime.Serialization.ISerializer GetSerializerForIEditableObject() { }
        protected TValue GetValue<TValue>(Catel.Data.IPropertyData property) { }
        protected TValue GetValue<TValue>(string name) { }
        protected virtual T GetValueFromPropertyBag<T>(string propertyName) { }
        protected virtual void InitializeCustomProperties() { }
        protected void InitializePropertyAfterConstruction(Catel.Data.IPropertyData property) { }
        protected bool IsModelBaseProperty(string name) { }
        public bool IsPropertyRegistered(string name) { }
        protected virtual void OnBeginEdit(System.ComponentModel.BeginEditEventArgs e) { }
        protected virtual void OnCancelEdit(System.ComponentModel.EditEventArgs e) { }
        protected virtual void OnCancelEditCompleted(System.ComponentModel.CancelEditCompletedEventArgs e) { }
        protected virtual void OnDeserialized() { }
        protected virtual void OnDeserializing() { }
        protected virtual void OnEndEdit(System.ComponentModel.EditEventArgs e) { }
        protected virtual void OnSerialized() { }
        protected virtual void OnSerializing() { }
        protected override void RaisePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) { }
        protected void RaisePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e, bool updateIsDirty, bool isRefreshCallOnly) { }
        protected void SetDefaultValueToPropertyBag(Catel.Data.IPropertyData propertyData) { }
        protected virtual void SetDirty(string propertyName) { }
        protected void SetValue<TValue>(Catel.Data.IPropertyData property, TValue value, bool notifyOnChange = true) { }
        protected void SetValue<TValue>(string name, TValue value, bool notifyOnChange = true) { }
        protected virtual void SetValueToPropertyBag<TValue>(string propertyName, TValue value) { }
        protected virtual bool ShouldPropertyChangeUpdateIsDirty(string propertyName) { }
        public System.IDisposable SuspendChangeCallbacks() { }
        public System.IDisposable SuspendChangeNotifications(bool raiseOnResume = true) { }
        public override string? ToString() { }
        protected static object? GetObjectValue<TValue>(TValue value) { }
        protected static bool IsPropertyRegistered(System.Type type, string name) { }
        public static Catel.Data.IPropertyData RegisterProperty<TValue>(string name, System.Func<TValue>? createDefaultValue = null, System.EventHandler<System.ComponentModel.PropertyChangedEventArgs>? propertyChangedEventHandler = null, bool includeInSerialization = true, bool includeInBackup = true) { }
        public static Catel.Data.IPropertyData RegisterProperty<TValue>(string name, TValue defaultValue, System.EventHandler<System.ComponentModel.PropertyChangedEventArgs>? propertyChangedEventHandler = null, bool includeInSerialization = true, bool includeInBackup = true) { }
        public static Catel.Data.IPropertyData RegisterProperty<TModel, TValue>(System.Linq.Expressions.Expression<System.Func<TModel, TValue>> propertyExpression, System.Func<TValue>? createDefaultValue = null, System.Action<TModel, System.ComponentModel.PropertyChangedEventArgs>? propertyChangedEventHandler = null, bool includeInSerialization = true, bool includeInBackup = true) { }
        public static Catel.Data.IPropertyData RegisterProperty<TModel, TValue>(System.Linq.Expressions.Expression<System.Func<TModel, TValue>> propertyExpression, TValue defaultValue, System.Action<TModel, System.ComponentModel.PropertyChangedEventArgs>? propertyChangedEventHandler = null, bool includeInSerialization = true, bool includeInBackup = true) { }
        public static Catel.Data.IPropertyData RegisterPropertyNonGeneric(string name, System.Type type, System.Func<object?>? createDefaultValue, System.EventHandler<System.ComponentModel.PropertyChangedEventArgs>? propertyChangedEventHandler = null, bool includeInSerialization = true, bool includeInBackup = true, bool isModelBaseProperty = false) { }
        public static Catel.Data.IPropertyData RegisterPropertyNonGeneric(string name, System.Type type, object? defaultValue = null, System.EventHandler<System.ComponentModel.PropertyChangedEventArgs>? propertyChangedEventHandler = null, bool includeInSerialization = true, bool includeInBackup = true, bool isModelBaseProperty = false) { }
        protected static void UnregisterProperty(System.Type modelType, string name) { }
    }
    public static class ModelBaseExtensions
    {
        public static void Save(this Catel.Data.ModelBase model, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializer serializer) { }
        public static void Save(this Catel.Data.ModelBase model, string filePath, Catel.Runtime.Serialization.ISerializer serializer) { }
        public static void SaveAsXml(this Catel.Data.ModelBase model, System.IO.Stream stream) { }
        public static void SaveAsXml(this Catel.Data.ModelBase model, string filePath) { }
    }
    public class ModelEqualityComparer : System.Collections.Generic.EqualityComparer<Catel.Data.ModelBase>, Catel.Data.IModelEqualityComparer, System.Collections.IEqualityComparer
    {
        public ModelEqualityComparer() { }
        public bool CompareCollections { get; set; }
        public bool CompareProperties { get; set; }
        public bool CompareValues { get; set; }
        public override bool Equals(Catel.Data.ModelBase? x, Catel.Data.ModelBase? y) { }
        public override int GetHashCode(Catel.Data.ModelBase obj) { }
    }
    [System.Serializable]
    public class ObservableObject : System.ComponentModel.INotifyPropertyChanged
    {
        public ObservableObject() { }
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) { }
        protected void RaisePropertyChanged(string propertyName) { }
        protected virtual void RaisePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) { }
        protected void RaisePropertyChanged<TProperty>(System.Linq.Expressions.Expression<System.Func<TProperty>> propertyExpression) { }
        protected void RaisePropertyChangedDirect(object? sender, System.ComponentModel.PropertyChangedEventArgs e) { }
    }
    public static class ObservableObjectExtensions
    {
        public static void RaiseAllPropertiesChanged(this Catel.Data.ObservableObject sender) { }
    }
    public class PropertyAlreadyRegisteredException : System.Exception
    {
        public PropertyAlreadyRegisteredException(string propertyName, System.Type propertyType) { }
        public string PropertyName { get; }
        public System.Type PropertyType { get; }
    }
    public class PropertyBag : Catel.Data.PropertyBagBase
    {
        public PropertyBag() { }
        public PropertyBag(System.Collections.Generic.IDictionary<string, object?> propertyDictionary) { }
        public object this[string name] { get; set; }
        public override string[] GetAllNames() { }
        public override System.Collections.Generic.Dictionary<string, object?> GetAllProperties() { }
        public override TValue GetValue<TValue>(string name, TValue defaultValue = default) { }
        public void Import(System.Collections.Generic.Dictionary<string, object?> propertiesToImport) { }
        public override bool IsAvailable(string name) { }
        public void SetValue(string propertyName, bool value) { }
        public void SetValue(string propertyName, byte value) { }
        public void SetValue(string propertyName, char value) { }
        public void SetValue(string propertyName, System.DateTime value) { }
        public void SetValue(string propertyName, decimal value) { }
        public void SetValue(string propertyName, double value) { }
        public void SetValue(string propertyName, short value) { }
        public void SetValue(string propertyName, int value) { }
        public void SetValue(string propertyName, long value) { }
        public void SetValue(string propertyName, sbyte value) { }
        public void SetValue(string propertyName, float value) { }
        public void SetValue(string propertyName, string value) { }
        public void SetValue(string propertyName, ushort value) { }
        public void SetValue(string propertyName, uint value) { }
        public void SetValue(string propertyName, ulong value) { }
        public override void SetValue<TValue>(string name, TValue value) { }
        public void UpdatePropertyValue<TValue>(string propertyName, System.Func<TValue, TValue> update) { }
    }
    public abstract class PropertyBagBase : Catel.Data.IPropertyBag, System.ComponentModel.INotifyPropertyChanged
    {
        protected PropertyBagBase() { }
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        public abstract string[] GetAllNames();
        public abstract System.Collections.Generic.Dictionary<string, object?> GetAllProperties();
        public abstract TValue GetValue<TValue>(string name, TValue defaultValue = default);
        public abstract bool IsAvailable(string name);
        protected void RaisePropertyChanged(string propertyName) { }
        public abstract void SetValue<TValue>(string name, TValue value);
    }
    public class PropertyData : Catel.Data.PropertyData<object> { }
    public class PropertyDataManager
    {
        public PropertyDataManager() { }
        public static Catel.Data.PropertyDataManager Default { get; }
        public Catel.Data.CatelTypeInfo GetCatelTypeInfo(System.Type type) { }
        public Catel.Data.IPropertyData GetPropertyData(System.Type type, string name) { }
        public bool IsPropertyNameMappedToXmlAttribute(System.Type type, string propertyName) { }
        public bool IsPropertyNameMappedToXmlElement(System.Type type, string propertyName) { }
        public bool IsPropertyRegistered(System.Type type, string name) { }
        public bool IsXmlAttributeNameMappedToProperty(System.Type type, string xmlName) { }
        public bool IsXmlElementNameMappedToProperty(System.Type type, string xmlName) { }
        public string MapPropertyNameToXmlAttributeName(System.Type type, string propertyName) { }
        public string MapPropertyNameToXmlElementName(System.Type type, string propertyName) { }
        public string MapXmlAttributeNameToPropertyName(System.Type type, string xmlName) { }
        public string MapXmlElementNameToPropertyName(System.Type type, string xmlName) { }
        public Catel.Data.CatelTypeInfo RegisterProperties(System.Type type) { }
        public void RegisterProperty(System.Type type, string name, Catel.Data.IPropertyData propertyData) { }
        public void UnregisterProperty(System.Type type, string name) { }
    }
    public class PropertyData<T> : Catel.Data.IPropertyData
    {
        [System.Xml.Serialization.XmlIgnore]
        public bool IncludeInBackup { get; }
        [System.Xml.Serialization.XmlIgnore]
        public bool IncludeInSerialization { get; }
        [System.Xml.Serialization.XmlIgnore]
        public bool IsCalculatedProperty { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public bool IsModelBaseProperty { get; }
        [System.Xml.Serialization.XmlIgnore]
        public bool IsSerializable { get; }
        public string Name { get; }
        [System.Xml.Serialization.XmlIgnore]
        public System.EventHandler<System.ComponentModel.PropertyChangedEventArgs>? PropertyChangedEventHandler { get; }
        [System.Xml.Serialization.XmlIgnore]
        public System.Type Type { get; }
        public object? GetDefaultValue() { }
        public TValue GetDefaultValue<TValue>() { }
        public Catel.Reflection.CachedPropertyInfo? GetPropertyInfo(System.Type containingType) { }
        public override string ToString() { }
    }
    public class PropertyNotNullableException : System.Exception
    {
        public PropertyNotNullableException(string propertyName, System.Type propertyType) { }
        public string PropertyName { get; }
        public System.Type PropertyType { get; }
    }
    public class PropertyNotRegisteredException : System.Exception
    {
        public PropertyNotRegisteredException(string propertyName, System.Type objectType) { }
        public System.Type ObjectType { get; }
        public string PropertyName { get; }
    }
    [System.Serializable]
    public class PropertyValue : System.Runtime.Serialization.ISerializable
    {
        public PropertyValue() { }
        public PropertyValue(Catel.Data.IPropertyData propertyData, System.Collections.Generic.KeyValuePair<string, object> keyValuePair) { }
        public PropertyValue(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public PropertyValue(Catel.Data.IPropertyData propertyData, string name, object value) { }
        [System.Xml.Serialization.XmlIgnore]
        public int GraphId { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public int GraphRefId { get; set; }
        public string? Name { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public Catel.Data.IPropertyData? PropertyData { get; }
        public object? Value { get; set; }
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public class ReflectionObjectAdapter : Catel.Data.IObjectAdapter
    {
        public ReflectionObjectAdapter() { }
        protected virtual bool TryGetFieldValue<TValue>(object instance, string memberName, out TValue? value) { }
        public virtual bool TryGetMemberValue<TValue>(object instance, string memberName, out TValue? value) { }
        protected virtual bool TryGetPropertyValue<TValue>(object instance, string memberName, out TValue? value) { }
        protected virtual bool TrySetFieldValue<TValue>(object instance, string memberName, TValue value) { }
        public virtual bool TrySetMemberValue<TValue>(object instance, string memberName, TValue? value) { }
        protected virtual bool TrySetPropertyValue<TValue>(object instance, string memberName, TValue value) { }
    }
    [System.Serializable]
    public abstract class SavableModelBase<T> : Catel.Data.ModelBase, Catel.Data.IFreezable, Catel.Data.IModel, Catel.Data.IModelEditor, Catel.Data.IModelSerialization, Catel.Data.ISavableModel, Catel.Runtime.Serialization.ISerializable, System.ComponentModel.IAdvancedEditableObject, System.ComponentModel.IEditableObject, System.ComponentModel.INotifyPropertyChanged, System.Xml.Serialization.IXmlSerializable
        where T :  class
    {
        protected SavableModelBase() { }
        public void Save(System.IO.Stream stream, Catel.Runtime.Serialization.ISerializer serializer, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public static T? Load(System.IO.Stream stream, Catel.Runtime.Serialization.ISerializer serializer, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public static Catel.Data.IModel? Load(System.Type type, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializer serializer, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
    }
    public class SuspensionContext
    {
        public SuspensionContext() { }
        public int Counter { get; }
        public System.Collections.Generic.IEnumerable<string> Properties { get; }
        public void Add(string propertyName) { }
        public void Decrement() { }
        public void Increment() { }
    }
    public class TypedPropertyBag : Catel.Data.PropertyBagBase
    {
        public TypedPropertyBag() { }
        public override string[] GetAllNames() { }
        public override System.Collections.Generic.Dictionary<string, object> GetAllProperties() { }
        protected System.Collections.Generic.Dictionary<string, bool> GetBooleanStorage() { }
        protected System.Collections.Generic.Dictionary<string, byte> GetByteStorage() { }
        protected System.Collections.Generic.Dictionary<string, char> GetCharStorage() { }
        protected System.Collections.Generic.Dictionary<string, System.DateTime> GetDateTimeStorage() { }
        protected System.Collections.Generic.Dictionary<string, decimal> GetDecimalStorage() { }
        protected System.Collections.Generic.Dictionary<string, double> GetDoubleStorage() { }
        protected System.Collections.Generic.Dictionary<string, short> GetInt16Storage() { }
        protected System.Collections.Generic.Dictionary<string, int> GetInt32Storage() { }
        protected System.Collections.Generic.Dictionary<string, long> GetInt64Storage() { }
        protected System.Collections.Generic.Dictionary<string, object> GetObjectStorage() { }
        protected System.Collections.Generic.Dictionary<string, sbyte> GetSByteStorage() { }
        protected System.Collections.Generic.Dictionary<string, float> GetSingleStorage() { }
        protected System.Collections.Generic.Dictionary<string, string> GetStringStorage() { }
        protected System.Collections.Generic.Dictionary<string, ushort> GetUInt16Storage() { }
        protected System.Collections.Generic.Dictionary<string, uint> GetUInt32Storage() { }
        protected System.Collections.Generic.Dictionary<string, ulong> GetUInt64Storage() { }
        public override TValue GetValue<TValue>(string name, TValue defaultValue = default) { }
        public override bool IsAvailable(string name) { }
        public override void SetValue<TValue>(string name, TValue value) { }
    }
    public abstract class ValidatableModelBase : Catel.Data.ModelBase, Catel.Data.IFreezable, Catel.Data.IModel, Catel.Data.IModelEditor, Catel.Data.IModelSerialization, Catel.Data.IValidatable, Catel.Data.IValidatableModel, Catel.Runtime.Serialization.ISerializable, System.ComponentModel.IAdvancedEditableObject, System.ComponentModel.IDataErrorInfo, System.ComponentModel.IDataWarningInfo, System.ComponentModel.IEditableObject, System.ComponentModel.INotifyDataErrorInfo, System.ComponentModel.INotifyDataWarningInfo, System.ComponentModel.INotifyPropertyChanged, System.Xml.Serialization.IXmlSerializable
    {
        protected static readonly System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.HashSet<string>> PropertiesNotCausingValidation;
        protected ValidatableModelBase() { }
        [System.ComponentModel.Browsable(false)]
        protected bool AutomaticallyValidateOnPropertyChanged { get; set; }
        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public virtual bool HasErrors { get; }
        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        public virtual bool HasWarnings { get; }
        [System.ComponentModel.Browsable(false)]
        protected bool HideValidationResults { get; set; }
        [System.ComponentModel.Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        protected bool IsValidating { get; }
        protected virtual bool IsValidationSuspended { get; }
        protected Catel.Data.IObjectAdapter? ObjectAdapter { get; set; }
        protected bool ValidateUsingDataAnnotations { get; set; }
        public static bool DefaultValidateUsingDataAnnotationsValue { get; set; }
        protected event System.EventHandler? ValidatedBusinessRules;
        protected event System.EventHandler? ValidatedFields;
        protected event System.EventHandler? ValidatingBusinessRules;
        protected event System.EventHandler? ValidatingFields;
        protected virtual string GetBusinessRuleErrors() { }
        protected virtual string GetBusinessRuleWarnings() { }
        protected virtual string GetFieldErrors(string columnName) { }
        protected virtual string GetFieldWarnings(string columnName) { }
        protected virtual bool IsValidationProperty(string? propertyName) { }
        protected void NotifyValidationResult(Catel.Data.IValidationResult validationResult, bool notifyGlobal) { }
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) { }
        protected virtual void OnValidated(Catel.Data.IValidationContext validationContext) { }
        protected virtual void OnValidatedBusinessRules(Catel.Data.IValidationContext validationContext) { }
        protected virtual void OnValidatedFields(Catel.Data.IValidationContext validationContext) { }
        protected virtual void OnValidating(Catel.Data.IValidationContext validationContext) { }
        protected virtual void OnValidatingBusinessRules(Catel.Data.IValidationContext validationContext) { }
        protected virtual void OnValidatingFields(Catel.Data.IValidationContext validationContext) { }
        protected override bool ShouldPropertyChangeUpdateIsDirty(string? propertyName) { }
        public System.IDisposable SuspendValidations(bool validateOnResume = true) { }
        public virtual void Validate(bool force = false) { }
        protected virtual void ValidateBusinessRules(System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> validationResults) { }
        protected virtual void ValidateFields(System.Collections.Generic.List<Catel.Data.IFieldValidationResult> validationResults) { }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple=true)]
    public class ValidateModelAttribute : System.Attribute
    {
        public ValidateModelAttribute(System.Type validatorType) { }
        public System.Type ValidatorType { get; }
    }
    public class ValidationContext : Catel.Data.IValidationContext
    {
        public ValidationContext() { }
        public ValidationContext(System.Collections.Generic.IEnumerable<Catel.Data.IFieldValidationResult>? fieldValidationResults, System.Collections.Generic.IEnumerable<Catel.Data.IBusinessRuleValidationResult>? businessRuleValidationResults) { }
        public ValidationContext(System.Collections.Generic.IEnumerable<Catel.Data.IFieldValidationResult>? fieldValidationResults, System.Collections.Generic.IEnumerable<Catel.Data.IBusinessRuleValidationResult>? businessRuleValidationResults, System.DateTime lastModified) { }
        public bool HasErrors { get; }
        public bool HasWarnings { get; }
        public System.DateTime LastModified { get; }
        public long LastModifiedTicks { get; }
        public void Add(Catel.Data.IBusinessRuleValidationResult businessRuleValidationResult) { }
        public void Add(Catel.Data.IFieldValidationResult fieldValidationResult) { }
        public int GetBusinessRuleErrorCount() { }
        public int GetBusinessRuleErrorCount(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleErrors() { }
        public System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleErrors(object? tag) { }
        public int GetBusinessRuleValidationCount() { }
        public int GetBusinessRuleValidationCount(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleValidations() { }
        public System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleValidations(object? tag) { }
        public int GetBusinessRuleWarningCount() { }
        public int GetBusinessRuleWarningCount(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleWarnings() { }
        public System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> GetBusinessRuleWarnings(object? tag) { }
        public int GetErrorCount() { }
        public int GetErrorCount(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IValidationResult> GetErrors() { }
        public System.Collections.Generic.List<Catel.Data.IValidationResult> GetErrors(object? tag) { }
        public int GetFieldErrorCount() { }
        public int GetFieldErrorCount(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldErrors() { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldErrors(string propertyName) { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldErrors(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldErrors(string propertyName, object? tag) { }
        public int GetFieldValidationCount() { }
        public int GetFieldValidationCount(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldValidations() { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldValidations(string propertyName) { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldValidations(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldValidations(string propertyName, object? tag) { }
        public int GetFieldWarningCount() { }
        public int GetFieldWarningCount(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldWarnings() { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldWarnings(string propertyName) { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldWarnings(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IFieldValidationResult> GetFieldWarnings(string propertyName, object? tag) { }
        public int GetValidationCount() { }
        public int GetValidationCount(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IValidationResult> GetValidations() { }
        public System.Collections.Generic.List<Catel.Data.IValidationResult> GetValidations(object? tag) { }
        public int GetWarningCount() { }
        public int GetWarningCount(object? tag) { }
        public System.Collections.Generic.List<Catel.Data.IValidationResult> GetWarnings() { }
        public System.Collections.Generic.List<Catel.Data.IValidationResult> GetWarnings(object? tag) { }
        public void Remove(Catel.Data.IBusinessRuleValidationResult businessRuleValidationResult) { }
        public void Remove(Catel.Data.IFieldValidationResult fieldValidationResult) { }
        public override string ToString() { }
    }
    public class ValidationContextChange
    {
        public ValidationContextChange(Catel.Data.IValidationResult validationResult, Catel.Data.ValidationContextChangeType changeType) { }
        public Catel.Data.ValidationContextChangeType ChangeType { get; }
        public Catel.Data.IValidationResult ValidationResult { get; }
    }
    public enum ValidationContextChangeType
    {
        Added = 0,
        Removed = 1,
    }
    public static class ValidationContextHelper
    {
        public static System.Collections.Generic.List<Catel.Data.ValidationContextChange> GetChanges(Catel.Data.IValidationContext firstContext, Catel.Data.IValidationContext secondContext) { }
    }
    public class ValidationEventArgs : System.EventArgs
    {
        public ValidationEventArgs(Catel.Data.IValidationContext validationContext) { }
        public Catel.Data.IValidationContext ValidationContext { get; }
    }
    public static class ValidationExtensions
    {
        public static Catel.Data.IValidationSummary GetValidationSummary(this Catel.Data.IValidationContext validationContext, object? tag = null) { }
        public static System.Collections.Generic.List<Catel.Data.ValidationContextChange> SynchronizeWithContext(this Catel.Data.ValidationContext validationContext, Catel.Data.IValidationContext additionalValidationContext, bool onlyAddValidation = false) { }
    }
    public abstract class ValidationResult : Catel.Data.IValidationResult
    {
        protected ValidationResult(Catel.Data.ValidationResultType validationResultType, string message) { }
        public string Message { get; set; }
        public object? Tag { get; set; }
        public Catel.Data.ValidationResultType ValidationResultType { get; }
    }
    public enum ValidationResultType
    {
        Warning = 0,
        Error = 1,
    }
    public class ValidationSummary : Catel.Data.IValidationSummary
    {
        public ValidationSummary(Catel.Data.IValidationContext validationContext) { }
        public ValidationSummary(Catel.Data.IValidationContext validationContext, object? tag) { }
        public System.Collections.ObjectModel.ReadOnlyCollection<Catel.Data.IBusinessRuleValidationResult> BusinessRuleErrors { get; }
        public System.Collections.ObjectModel.ReadOnlyCollection<Catel.Data.IBusinessRuleValidationResult> BusinessRuleWarnings { get; }
        public System.Collections.ObjectModel.ReadOnlyCollection<Catel.Data.IFieldValidationResult> FieldErrors { get; }
        public System.Collections.ObjectModel.ReadOnlyCollection<Catel.Data.IFieldValidationResult> FieldWarnings { get; }
        public bool HasBusinessRuleErrors { get; }
        public bool HasBusinessRuleWarnings { get; }
        public bool HasErrors { get; }
        public bool HasFieldErrors { get; }
        public bool HasFieldWarnings { get; }
        public bool HasWarnings { get; }
        public System.DateTime LastModified { get; }
        public long LastModifiedTicks { get; }
        public override string ToString() { }
    }
    public abstract class ValidatorBase<TTargetType> : Catel.Data.IValidator
        where TTargetType :  class
    {
        protected ValidatorBase() { }
        public void AfterValidateBusinessRules(object instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> validationResults) { }
        protected virtual void AfterValidateBusinessRules(TTargetType instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> validationResults) { }
        public void AfterValidateFields(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> validationResults) { }
        protected virtual void AfterValidateFields(TTargetType instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> validationResults) { }
        public void AfterValidation(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> fieldValidationResults, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> businessRuleValidationResults) { }
        protected virtual void AfterValidation(TTargetType instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> fieldValidationResults, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> businessRuleValidationResults) { }
        public void BeforeValidateBusinessRules(object instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> previousValidationResults) { }
        protected virtual void BeforeValidateBusinessRules(TTargetType instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> previousValidationResults) { }
        public void BeforeValidateFields(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> previousValidationResults) { }
        protected virtual void BeforeValidateFields(TTargetType instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> previousValidationResults) { }
        public void BeforeValidation(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> previousFieldValidationResults, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> previousBusinessRuleValidationResults) { }
        protected virtual void BeforeValidation(TTargetType instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> previousFieldValidationResults, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> previousBusinessRuleValidationResults) { }
        public void Validate(object instance, Catel.Data.ValidationContext validationContext) { }
        protected virtual void Validate(TTargetType instance, Catel.Data.ValidationContext validationContext) { }
        public void ValidateBusinessRules(object instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> validationResults) { }
        protected virtual void ValidateBusinessRules(TTargetType instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> validationResults) { }
        public void ValidateFields(object instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> validationResults) { }
        protected virtual void ValidateFields(TTargetType instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> validationResults) { }
    }
    public abstract class ValidatorProviderBase : Catel.Data.IValidatorProvider
    {
        protected ValidatorProviderBase() { }
        public bool UseCache { get; set; }
        protected abstract Catel.Data.IValidator? GetValidator(System.Type targetType);
    }
    public class XmlNameMapper<T>
    {
        public bool IsPropertyNameMappedToXmlName(System.Type type, string propertyName) { }
        public bool IsXmlNameMappedToProperty(System.Type type, string xmlName) { }
        public string MapPropertyNameToXmlName(System.Type type, string propertyName) { }
        public string MapXmlNameToPropertyName(System.Type type, string xmlName) { }
    }
}
namespace Catel.IO
{
    public enum ApplicationDataTarget
    {
        UserLocal = 0,
        UserRoaming = 1,
        Machine = 2,
    }
    public static class Path
    {
        public static string AppendTrailingSlash(string path) { }
        public static string AppendTrailingSlash(string path, char slash) { }
        public static string CombineUrls(params string[] urls) { }
        public static string GetApplicationDataDirectory() { }
        public static string GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget applicationDataTarget) { }
        public static string GetApplicationDataDirectory(string productName) { }
        public static string GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget applicationDataTarget, string productName) { }
        public static string GetApplicationDataDirectory(string companyName, string productName) { }
        public static string GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget applicationDataTarget, string companyName, string productName) { }
        public static string GetApplicationDataDirectoryForAllUsers() { }
        public static string GetApplicationDataDirectoryForAllUsers(string productName) { }
        public static string GetApplicationDataDirectoryForAllUsers(string companyName, string productName) { }
        public static string GetDirectoryName(string path) { }
        public static string GetFileName(string path) { }
        public static string GetFullPath(string relativePath, string basePath) { }
        public static string GetParentDirectory(string path) { }
        public static string GetRelativePath(string fullPath, string? basePath = null) { }
    }
    public static class StreamExtensions
    {
        public static string GetString(this System.IO.Stream stream, System.Text.Encoding encoding) { }
        public static string GetUtf8String(this System.IO.Stream stream) { }
        public static byte[] ToByteArray(this System.IO.Stream stream) { }
    }
}
namespace Catel.IoC
{
    public class CatelDependencyResolver : Catel.IoC.IDependencyResolver
    {
        public CatelDependencyResolver(Catel.IoC.IServiceLocator serviceLocator) { }
        public bool CanResolve(System.Type type, object? tag = null) { }
        public bool CanResolveMultiple(System.Type[] types) { }
        public object? Resolve(System.Type type, object? tag = null) { }
        public object[] ResolveMultiple(System.Type[] types, object? tag = null) { }
    }
    public class CircularDependencyException : System.Exception
    {
        public Catel.IoC.TypeRequestInfo DuplicateRequestInfo { get; }
        public Catel.IoC.ITypeRequestPath TypePath { get; }
    }
    public static class DependencyResolverExtensions
    {
        public static bool CanResolve<T>(this Catel.IoC.IDependencyResolver dependencyResolver, object? tag = null) { }
        public static T? Resolve<T>(this Catel.IoC.IDependencyResolver dependencyResolver, object? tag = null) { }
        public static object ResolveRequired(this Catel.IoC.IDependencyResolver dependencyResolver, System.Type type, object? tag = null) { }
        public static T ResolveRequired<T>(this Catel.IoC.IDependencyResolver dependencyResolver, object? tag = null) { }
    }
    public class DependencyResolverManager : Catel.IoC.IDependencyResolverManager
    {
        public DependencyResolverManager() { }
        public Catel.IoC.IDependencyResolver DefaultDependencyResolver { get; set; }
        public static Catel.IoC.IDependencyResolverManager Default { get; set; }
        public virtual Catel.IoC.IDependencyResolver GetDependencyResolverForInstance(object instance) { }
        public virtual Catel.IoC.IDependencyResolver GetDependencyResolverForType(System.Type type) { }
        public virtual void RegisterDependencyResolverForInstance(object instance, Catel.IoC.IDependencyResolver dependencyResolver) { }
        public virtual void RegisterDependencyResolverForType(System.Type type, Catel.IoC.IDependencyResolver dependencyResolver) { }
    }
    public interface IDependencyResolver
    {
        bool CanResolve(System.Type type, object? tag = null);
        bool CanResolveMultiple(System.Type[] types);
        object? Resolve(System.Type type, object? tag = null);
        object[] ResolveMultiple(System.Type[] types, object? tag = null);
    }
    public interface IDependencyResolverManager
    {
        Catel.IoC.IDependencyResolver DefaultDependencyResolver { get; set; }
        Catel.IoC.IDependencyResolver GetDependencyResolverForInstance(object instance);
        Catel.IoC.IDependencyResolver GetDependencyResolverForType(System.Type type);
        void RegisterDependencyResolverForInstance(object instance, Catel.IoC.IDependencyResolver dependencyResolver);
        void RegisterDependencyResolverForType(System.Type type, Catel.IoC.IDependencyResolver dependencyResolver);
    }
    public interface INeedCustomInitialization
    {
        void Initialize();
    }
    public interface IServiceLocator : System.IDisposable, System.IServiceProvider
    {
        bool CanResolveNonAbstractTypesWithoutRegistration { get; set; }
        event System.EventHandler<Catel.IoC.MissingTypeEventArgs>? MissingType;
        event System.EventHandler<Catel.IoC.TypeInstantiatedEventArgs>? TypeInstantiated;
        event System.EventHandler<Catel.IoC.TypeRegisteredEventArgs>? TypeRegistered;
        event System.EventHandler<Catel.IoC.TypeUnregisteredEventArgs>? TypeUnregistered;
        bool AreMultipleTypesRegistered(params System.Type[] types);
        Catel.IoC.RegistrationInfo? GetRegistrationInfo(System.Type serviceType, object? tag = null);
        bool IsTypeRegistered(System.Type serviceType, object? tag = null);
        bool IsTypeRegisteredAsSingleton(System.Type serviceType, object? tag = null);
        bool IsTypeRegisteredWithOrWithoutTag(System.Type serviceType);
        void RegisterInstance(System.Type serviceType, object instance, object? tag = null);
        void RegisterType(System.Type serviceType, System.Func<Catel.IoC.ITypeFactory, Catel.IoC.ServiceLocatorRegistration, object> createServiceFunc, object? tag = null, Catel.IoC.RegistrationType registrationType = 0, bool registerIfAlreadyRegistered = true);
        void RegisterType(System.Type serviceType, System.Type serviceImplementationType, object? tag = null, Catel.IoC.RegistrationType registrationType = 0, bool registerIfAlreadyRegistered = true);
        bool RemoveAllTypes(System.Type serviceType);
        bool RemoveType(System.Type serviceType, object? tag = null);
        object?[] ResolveMultipleTypes(params System.Type[] types);
        object? ResolveType(System.Type serviceType, object? tag = null);
        object? ResolveTypeUsingFactory(Catel.IoC.ITypeFactory typeFactory, System.Type serviceType, object? tag = null);
        System.Collections.Generic.IEnumerable<object> ResolveTypes(System.Type serviceType);
        System.Collections.Generic.IEnumerable<object> ResolveTypesUsingFactory(Catel.IoC.ITypeFactory typeFactory, System.Type serviceType);
    }
    public interface IServiceLocatorInitializer
    {
        void Initialize(Catel.IoC.IServiceLocator serviceLocator);
    }
    public interface ITypeFactory : System.IDisposable
    {
        void ClearCache();
        object? CreateInstance(System.Type typeToConstruct);
        object? CreateInstanceWithParameters(System.Type typeToConstruct, params object?[] parameters);
        object? CreateInstanceWithParametersAndAutoCompletion(System.Type typeToConstruct, params object?[] parameters);
        object? CreateInstanceWithParametersAndAutoCompletionWithTag(System.Type typeToConstruct, object? tag, params object?[] parameters);
        object? CreateInstanceWithParametersWithTag(System.Type typeToConstruct, object? tag, params object?[] parameters);
        object? CreateInstanceWithTag(System.Type typeToConstruct, object? tag);
    }
    public interface ITypeRequestPath
    {
        System.Collections.Generic.IEnumerable<Catel.IoC.TypeRequestInfo> AllTypes { get; }
        Catel.IoC.TypeRequestInfo? FirstType { get; }
        Catel.IoC.TypeRequestInfo? LastType { get; }
    }
    public static class IoCConfiguration
    {
        public static Catel.IoC.IDependencyResolver DefaultDependencyResolver { get; }
        public static Catel.IoC.IServiceLocator DefaultServiceLocator { get; }
        public static Catel.IoC.ITypeFactory DefaultTypeFactory { get; }
        public static void UpdateDefaultComponents() { }
    }
    public static class IoCFactory
    {
        public static System.Func<Catel.IoC.IServiceLocator, Catel.IoC.IDependencyResolver> CreateDependencyResolverFunc { get; set; }
        public static System.Func<Catel.IoC.IServiceLocator> CreateServiceLocatorFunc { get; set; }
        public static System.Func<Catel.IoC.IServiceLocator, Catel.IoC.ITypeFactory> CreateTypeFactoryFunc { get; set; }
        public static Catel.IoC.IServiceLocator CreateServiceLocator(bool initializeServiceLocator = true) { }
    }
    public sealed class LateBoundImplementation { }
    public class MissingTypeEventArgs : System.EventArgs
    {
        public MissingTypeEventArgs(System.Type interfaceType) { }
        public MissingTypeEventArgs(System.Type interfaceType, object? tag) { }
        public object? ImplementingInstance { get; set; }
        public System.Type? ImplementingType { get; set; }
        public System.Type InterfaceType { get; }
        public Catel.IoC.RegistrationType RegistrationType { get; set; }
        public object? Tag { get; set; }
    }
    public static class ObjectExtensions
    {
        public static Catel.IoC.IDependencyResolver GetDependencyResolver(this object obj) { }
        public static Catel.IoC.IServiceLocator GetServiceLocator(this object obj) { }
        public static Catel.IoC.ITypeFactory GetTypeFactory(this object obj) { }
    }
    public class RegistrationInfo
    {
        public System.Type DeclaringType { get; }
        public System.Type ImplementingType { get; }
        public bool IsLateBoundRegistration { get; }
        public bool IsTypeInstantiatedForSingleton { get; }
        public Catel.IoC.RegistrationType RegistrationType { get; }
    }
    public enum RegistrationType
    {
        Singleton = 0,
        Transient = 1,
    }
    public class ServiceLocator : Catel.IoC.IServiceLocator, System.IDisposable, System.IServiceProvider
    {
        public ServiceLocator() { }
        public ServiceLocator(Catel.IoC.IServiceLocator serviceLocator) { }
        public bool CanResolveNonAbstractTypesWithoutRegistration { get; set; }
        public static Catel.IoC.IServiceLocator Default { get; }
        public event System.EventHandler<Catel.IoC.MissingTypeEventArgs>? MissingType;
        public event System.EventHandler<Catel.IoC.TypeInstantiatedEventArgs>? TypeInstantiated;
        public event System.EventHandler<Catel.IoC.TypeRegisteredEventArgs>? TypeRegistered;
        public event System.EventHandler<Catel.IoC.TypeUnregisteredEventArgs>? TypeUnregistered;
        public bool AreMultipleTypesRegistered(params System.Type[] types) { }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public Catel.IoC.RegistrationInfo? GetRegistrationInfo(System.Type serviceType, object? tag = null) { }
        public bool IsTypeRegistered(System.Type serviceType, object? tag = null) { }
        public bool IsTypeRegisteredAsSingleton(System.Type serviceType, object? tag = null) { }
        public bool IsTypeRegisteredWithOrWithoutTag(System.Type serviceType) { }
        public void RegisterInstance(System.Type serviceType, object instance, object? tag = null) { }
        public void RegisterType(System.Type serviceType, System.Func<Catel.IoC.ITypeFactory, Catel.IoC.ServiceLocatorRegistration, object?> createServiceFunc, object? tag = null, Catel.IoC.RegistrationType registrationType = 0, bool registerIfAlreadyRegistered = true) { }
        public void RegisterType(System.Type serviceType, System.Type serviceImplementationType, object? tag = null, Catel.IoC.RegistrationType registrationType = 0, bool registerIfAlreadyRegistered = true) { }
        public bool RemoveAllTypes(System.Type serviceType) { }
        public bool RemoveType(System.Type serviceType, object? tag = null) { }
        public object?[] ResolveMultipleTypes(params System.Type[] types) { }
        public virtual object? ResolveType(System.Type serviceType, object? tag = null) { }
        public virtual object? ResolveTypeUsingFactory(Catel.IoC.ITypeFactory typeFactory, System.Type serviceType, object? tag = null) { }
        public System.Collections.Generic.IEnumerable<object> ResolveTypes(System.Type serviceType) { }
        public System.Collections.Generic.IEnumerable<object> ResolveTypesUsingFactory(Catel.IoC.ITypeFactory typeFactory, System.Type serviceType) { }
    }
    public static class ServiceLocatorExtensions
    {
        public static bool IsTypeRegistered<TService>(this Catel.IoC.IServiceLocator serviceLocator, object? tag = null)
            where TService :  notnull { }
        public static bool IsTypeRegisteredAsSingleton<TService>(this Catel.IoC.IServiceLocator serviceLocator, object? tag = null)
            where TService :  notnull { }
        public static void RegisterInstance<TService>(this Catel.IoC.IServiceLocator serviceLocator, TService instance, object? tag = null)
            where TService :  notnull { }
        public static void RegisterType<TServiceImplementation>(this Catel.IoC.IServiceLocator serviceLocator, Catel.IoC.RegistrationType registrationType = 0) { }
        public static void RegisterType<TService>(this Catel.IoC.IServiceLocator serviceLocator, System.Func<Catel.IoC.ITypeFactory, Catel.IoC.ServiceLocatorRegistration, TService> createServiceFunc, Catel.IoC.RegistrationType registrationType = 0, bool registerIfAlreadyRegistered = true)
            where TService :  notnull { }
        public static void RegisterType<TService, TServiceImplementation>(this Catel.IoC.IServiceLocator serviceLocator, Catel.IoC.RegistrationType registrationType = 0, bool registerIfAlreadyRegistered = true)
            where TService :  notnull
            where TServiceImplementation : TService { }
        public static TServiceImplementation? RegisterTypeAndInstantiate<TServiceImplementation>(this Catel.IoC.IServiceLocator serviceLocator)
            where TServiceImplementation :  notnull { }
        public static TService? RegisterTypeAndInstantiate<TService, TServiceImplementation>(this Catel.IoC.IServiceLocator serviceLocator)
            where TService :  notnull
            where TServiceImplementation : TService { }
        public static void RegisterTypeIfNotYetRegistered(this Catel.IoC.IServiceLocator serviceLocator, System.Type serviceType, System.Type serviceImplementationType, Catel.IoC.RegistrationType registrationType = 0) { }
        public static void RegisterTypeIfNotYetRegistered<TService, TServiceImplementation>(this Catel.IoC.IServiceLocator serviceLocator, Catel.IoC.RegistrationType registrationType = 0)
            where TService :  notnull
            where TServiceImplementation : TService { }
        public static void RegisterTypeIfNotYetRegisteredWithTag(this Catel.IoC.IServiceLocator serviceLocator, System.Type serviceType, System.Type serviceImplementationType, object? tag = null, Catel.IoC.RegistrationType registrationType = 0) { }
        public static void RegisterTypeIfNotYetRegisteredWithTag<TService, TServiceImplementation>(this Catel.IoC.IServiceLocator serviceLocator, object? tag = null, Catel.IoC.RegistrationType registrationType = 0)
            where TService :  notnull
            where TServiceImplementation : TService { }
        public static void RegisterTypeWithTag<TServiceImplementation>(this Catel.IoC.IServiceLocator serviceLocator, object? tag = null, Catel.IoC.RegistrationType registrationType = 0) { }
        public static void RegisterTypeWithTag<TService>(this Catel.IoC.IServiceLocator serviceLocator, System.Func<Catel.IoC.ITypeFactory, Catel.IoC.ServiceLocatorRegistration, TService> createServiceFunc, object? tag = null, Catel.IoC.RegistrationType registrationType = 0, bool registerIfAlreadyRegistered = true)
            where TService :  notnull { }
        public static void RegisterTypeWithTag<TService, TServiceImplementation>(this Catel.IoC.IServiceLocator serviceLocator, object? tag = null, Catel.IoC.RegistrationType registrationType = 0, bool registerIfAlreadyRegistered = true)
            where TService :  notnull
            where TServiceImplementation : TService { }
        public static void RemoveType<TService>(this Catel.IoC.IServiceLocator serviceLocator, object? tag = null)
            where TService :  notnull { }
        public static object ResolveRequiredType(this Catel.IoC.IServiceLocator serviceLocator, System.Type serviceType, object? tag = null) { }
        public static TService ResolveRequiredType<TService>(this Catel.IoC.IServiceLocator serviceLocator, object? tag = null)
            where TService :  notnull { }
        public static TService ResolveRequiredTypeUsingFactory<TService>(this Catel.IoC.IServiceLocator serviceLocator, Catel.IoC.ITypeFactory typeFactory, object? tag = null)
            where TService :  notnull { }
        public static object ResolveRequiredTypeUsingParameters(this Catel.IoC.IServiceLocator serviceLocator, System.Type serviceType, object[] parameters, object? tag = null) { }
        public static T ResolveRequiredTypeUsingParameters<T>(this Catel.IoC.IServiceLocator serviceLocator, object[] parameters, object? tag = null)
            where T :  notnull { }
        public static TService? ResolveType<TService>(this Catel.IoC.IServiceLocator serviceLocator, object? tag = null)
            where TService :  notnull { }
        public static TService? ResolveTypeUsingFactory<TService>(this Catel.IoC.IServiceLocator serviceLocator, Catel.IoC.ITypeFactory typeFactory, object? tag = null)
            where TService :  notnull { }
        public static object? ResolveTypeUsingParameters(this Catel.IoC.IServiceLocator serviceLocator, System.Type serviceType, object[] parameters, object? tag = null) { }
        public static T? ResolveTypeUsingParameters<T>(this Catel.IoC.IServiceLocator serviceLocator, object[] parameters, object? tag = null)
            where T :  notnull { }
        public static System.Collections.Generic.IEnumerable<TService> ResolveTypes<TService>(this Catel.IoC.IServiceLocator serviceLocator)
            where TService :  notnull { }
    }
    [System.Diagnostics.DebuggerDisplay("{DeclaringType} => {ImplementingType} ({RegistrationType})")]
    public class ServiceLocatorRegistration
    {
        public ServiceLocatorRegistration(System.Type declaringType, System.Type implementingType, object? tag, Catel.IoC.RegistrationType registrationType, System.Func<Catel.IoC.ITypeFactory, Catel.IoC.ServiceLocatorRegistration, object> createServiceFunc) { }
        public System.Func<Catel.IoC.ITypeFactory, Catel.IoC.ServiceLocatorRegistration, object> CreateServiceFunc { get; }
        public System.Type DeclaringType { get; }
        public string? DeclaringTypeName { get; }
        public System.Type ImplementingType { get; }
        public string? ImplementingTypeName { get; }
        public Catel.IoC.RegistrationType RegistrationType { get; }
        public object? Tag { get; }
    }
    public class ServiceLocatorRegistrationGroup
    {
        public ServiceLocatorRegistrationGroup(Catel.IoC.ServiceLocatorRegistration entryRegistration) { }
        public Catel.IoC.ServiceLocatorRegistration EntryRegistration { get; }
    }
    public class TypeFactory : Catel.IoC.ITypeFactory, System.IDisposable
    {
        public TypeFactory(Catel.IoC.IServiceLocator serviceLocator) { }
        public static Catel.IoC.ITypeFactory Default { get; }
        public void ClearCache() { }
        public object? CreateInstance(System.Type typeToConstruct) { }
        public object? CreateInstanceWithParameters(System.Type typeToConstruct, params object?[] parameters) { }
        public object? CreateInstanceWithParametersAndAutoCompletion(System.Type typeToConstruct, params object?[] parameters) { }
        public object? CreateInstanceWithParametersAndAutoCompletionWithTag(System.Type typeToConstruct, object? tag, params object?[] parameters) { }
        public object? CreateInstanceWithParametersWithTag(System.Type typeToConstruct, object? tag, params object?[] parameters) { }
        public object? CreateInstanceWithTag(System.Type typeToConstruct, object? tag) { }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
    }
    public static class TypeFactoryExtensions
    {
        public static T? CreateInstance<T>(this Catel.IoC.ITypeFactory typeFactory) { }
        public static T? CreateInstanceWithParameters<T>(this Catel.IoC.ITypeFactory typeFactory, params object[] parameters) { }
        public static T? CreateInstanceWithParametersAndAutoCompletion<T>(this Catel.IoC.ITypeFactory typeFactory, params object[] parameters) { }
        public static T? CreateInstanceWithParametersAndAutoCompletionWithTag<T>(this Catel.IoC.ITypeFactory typeFactory, object tag, params object[] parameters) { }
        public static T? CreateInstanceWithParametersWithTag<T>(this Catel.IoC.ITypeFactory typeFactory, object tag, params object[] parameters) { }
        public static T? CreateInstanceWithTag<T>(this Catel.IoC.ITypeFactory typeFactory, object tag) { }
        public static object CreateRequiredInstance(this Catel.IoC.ITypeFactory typeFactory, System.Type typeToConstruct) { }
        public static T CreateRequiredInstance<T>(this Catel.IoC.ITypeFactory typeFactory) { }
        public static object CreateRequiredInstanceWithParameters(this Catel.IoC.ITypeFactory typeFactory, System.Type typeToConstruct, params object?[] parameters) { }
        public static T CreateRequiredInstanceWithParameters<T>(this Catel.IoC.ITypeFactory typeFactory, params object?[] parameters) { }
        public static object CreateRequiredInstanceWithParametersAndAutoCompletion(this Catel.IoC.ITypeFactory typeFactory, System.Type typeToConstruct, params object?[] parameters) { }
        public static T CreateRequiredInstanceWithParametersAndAutoCompletion<T>(this Catel.IoC.ITypeFactory typeFactory, params object?[] parameters) { }
    }
    public class TypeInstantiatedEventArgs : System.EventArgs
    {
        public TypeInstantiatedEventArgs(System.Type serviceType, System.Type serviceImplementationType, object? tag, Catel.IoC.RegistrationType registrationType, object instance) { }
        public object Instance { get; }
        public Catel.IoC.RegistrationType RegistrationType { get; }
        public System.Type ServiceImplementationType { get; }
        public System.Type ServiceType { get; }
        public object? Tag { get; }
    }
    public class TypeNotRegisteredException : System.Exception
    {
        public TypeNotRegisteredException(System.Type requestedType, string message) { }
        public System.Type RequestedType { get; }
    }
    public class TypeRegisteredEventArgs : System.EventArgs
    {
        public TypeRegisteredEventArgs(System.Type serviceType, System.Type serviceImplementationType, object? tag, Catel.IoC.RegistrationType registrationType) { }
        public Catel.IoC.RegistrationType RegistrationType { get; }
        public System.Type ServiceImplementationType { get; }
        public System.Type ServiceType { get; }
        public object? Tag { get; }
    }
    public class TypeRequestInfo
    {
        public TypeRequestInfo(System.Type type, object? tag = null) { }
        public object? Tag { get; }
        public System.Type Type { get; }
        public override bool Equals(object? obj) { }
        public override int GetHashCode() { }
        public override string ToString() { }
        public static bool operator !=(Catel.IoC.TypeRequestInfo firstObject, Catel.IoC.TypeRequestInfo secondObject) { }
        public static bool operator ==(Catel.IoC.TypeRequestInfo firstObject, Catel.IoC.TypeRequestInfo secondObject) { }
    }
    public class TypeRequestPath : Catel.IoC.ITypeRequestPath
    {
        public System.Collections.Generic.IEnumerable<Catel.IoC.TypeRequestInfo> AllTypes { get; }
        public Catel.IoC.TypeRequestInfo? FirstType { get; }
        public Catel.IoC.TypeRequestInfo? LastType { get; }
        public string Name { get; }
        public int TypeCount { get; }
        public override string ToString() { }
        public static Catel.IoC.TypeRequestPath Branch(Catel.IoC.TypeRequestPath parent, Catel.IoC.TypeRequestInfo typeRequestInfo) { }
        public static Catel.IoC.TypeRequestPath Root(string name) { }
    }
    public class TypeUnregisteredEventArgs : System.EventArgs
    {
        public TypeUnregisteredEventArgs(System.Type serviceType, System.Type serviceImplementationType, object tag, Catel.IoC.RegistrationType registrationType) { }
        public TypeUnregisteredEventArgs(System.Type serviceType, System.Type serviceImplementationType, object? tag, Catel.IoC.RegistrationType registrationType, object? instance) { }
        public object? Instance { get; }
        public Catel.IoC.RegistrationType RegistrationType { get; }
        public System.Type ServiceImplementationType { get; }
        public System.Type ServiceType { get; }
        public object? Tag { get; }
    }
}
namespace Catel.Linq
{
    public static class EnumerableExtensions
    {
        public static System.Collections.IEnumerable AsReadOnly(this System.Collections.IEnumerable instance, System.Type type) { }
        public static System.Collections.IEnumerable Cast(this System.Collections.IEnumerable instance, System.Type type) { }
        public static System.Collections.IEnumerable ToList(this System.Collections.IEnumerable instance, System.Type type) { }
        public static System.Collections.IEnumerable ToSystemArray(this System.Collections.IEnumerable instance, System.Type type) { }
    }
}
namespace Catel.Linq.Expressions
{
    public static class ExpressionBuilder
    {
        public static System.Linq.Expressions.Expression<System.Func<T, object>>? CreateFieldGetter<T>(System.Reflection.FieldInfo fieldInfo) { }
        public static System.Linq.Expressions.Expression<System.Func<T, object>>? CreateFieldGetter<T>(string fieldName) { }
        public static System.Linq.Expressions.Expression<System.Func<object, TField>>? CreateFieldGetter<TField>(System.Type modelType, string fieldName) { }
        public static System.Linq.Expressions.Expression<System.Func<T, TField>>? CreateFieldGetter<T, TField>(System.Reflection.FieldInfo fieldInfo) { }
        public static System.Linq.Expressions.Expression<System.Func<T, TField>>? CreateFieldGetter<T, TField>(string fieldName) { }
        public static System.Collections.Generic.IReadOnlyDictionary<string, System.Linq.Expressions.Expression<System.Func<T, object>>> CreateFieldGetters<T>() { }
        public static System.Collections.Generic.IReadOnlyDictionary<string, System.Linq.Expressions.Expression<System.Func<T, TField>>> CreateFieldGetters<T, TField>() { }
        public static System.Linq.Expressions.Expression<System.Action<T, object>>? CreateFieldSetter<T>(System.Reflection.FieldInfo fieldInfo) { }
        public static System.Linq.Expressions.Expression<System.Action<T, object>>? CreateFieldSetter<T>(string fieldName) { }
        public static System.Linq.Expressions.Expression<System.Action<object, TField>>? CreateFieldSetter<TField>(System.Type modelType, string fieldName) { }
        public static System.Linq.Expressions.Expression<System.Action<T, TField>>? CreateFieldSetter<T, TField>(System.Reflection.FieldInfo fieldInfo) { }
        public static System.Linq.Expressions.Expression<System.Action<T, TField>>? CreateFieldSetter<T, TField>(string fieldName) { }
        public static System.Collections.Generic.IReadOnlyDictionary<string, System.Linq.Expressions.Expression<System.Action<T, object>>> CreateFieldSetters<T>() { }
        public static System.Collections.Generic.IReadOnlyDictionary<string, System.Linq.Expressions.Expression<System.Action<T, TField>>> CreateFieldSetters<T, TField>() { }
        public static System.Linq.Expressions.Expression<System.Func<T, object>>? CreatePropertyGetter<T>(System.Reflection.PropertyInfo propertyInfo) { }
        public static System.Linq.Expressions.Expression<System.Func<T, object>>? CreatePropertyGetter<T>(string propertyName) { }
        public static System.Linq.Expressions.Expression<System.Func<object, TProperty>>? CreatePropertyGetter<TProperty>(System.Type modelType, string propertyName) { }
        public static System.Linq.Expressions.Expression<System.Func<T, TProperty>>? CreatePropertyGetter<T, TProperty>(System.Reflection.PropertyInfo propertyInfo) { }
        public static System.Linq.Expressions.Expression<System.Func<T, TProperty>>? CreatePropertyGetter<T, TProperty>(string propertyName) { }
        public static System.Collections.Generic.IReadOnlyDictionary<string, System.Linq.Expressions.Expression<System.Func<T, object>>> CreatePropertyGetters<T>() { }
        public static System.Collections.Generic.IReadOnlyDictionary<string, System.Linq.Expressions.Expression<System.Func<T, TProperty>>> CreatePropertyGetters<T, TProperty>() { }
        public static System.Linq.Expressions.Expression<System.Action<T, object>>? CreatePropertySetter<T>(System.Reflection.PropertyInfo propertyInfo) { }
        public static System.Linq.Expressions.Expression<System.Action<T, object>>? CreatePropertySetter<T>(string propertyName) { }
        public static System.Linq.Expressions.Expression<System.Action<object, TProperty>>? CreatePropertySetter<TProperty>(System.Type modelType, string propertyName) { }
        public static System.Linq.Expressions.Expression<System.Action<T, TProperty>>? CreatePropertySetter<T, TProperty>(System.Reflection.PropertyInfo propertyInfo) { }
        public static System.Linq.Expressions.Expression<System.Action<T, TProperty>>? CreatePropertySetter<T, TProperty>(string propertyName) { }
        public static System.Collections.Generic.IReadOnlyDictionary<string, System.Linq.Expressions.Expression<System.Action<T, object>>> CreatePropertySetters<T>() { }
        public static System.Collections.Generic.IReadOnlyDictionary<string, System.Linq.Expressions.Expression<System.Action<T, TProperty>>> CreatePropertySetters<T, TProperty>() { }
    }
}
namespace Catel.Logging
{
    public abstract class BatchLogListenerBase : Catel.Logging.LogListenerBase, Catel.Logging.IBatchLogListener
    {
        public BatchLogListenerBase(int maxBatchCount = 100) { }
        public BatchLogListenerBase(System.TimeSpan interval, int maxBatchCount = 100) { }
        protected System.TimeSpan Interval { get; set; }
        public int MaximumBatchCount { get; }
        public System.Threading.Tasks.Task FlushAsync() { }
        protected override void Write(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
        protected virtual System.Threading.Tasks.Task WriteBatchAsync(System.Collections.Generic.List<Catel.Logging.LogBatchEntry> batchEntries) { }
    }
    public class ConsoleLogListener : Catel.Logging.LogListenerBase
    {
        public ConsoleLogListener() { }
        protected override void Write(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
    }
    public class DebugLogListener : Catel.Logging.LogListenerBase
    {
        public DebugLogListener() { }
        protected override void Write(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
    }
    public class FileLogListener : Catel.Logging.BatchLogListenerBase
    {
        public FileLogListener(System.Reflection.Assembly? assembly = null) { }
        public FileLogListener(string filePath, int maxSizeInKiloBytes, System.Reflection.Assembly? assembly = null) { }
        public string FilePath { get; set; }
        public int MaxSizeInKiloBytes { get; set; }
        protected virtual string DetermineFilePath(string filePath) { }
        protected override System.Threading.Tasks.Task WriteBatchAsync(System.Collections.Generic.List<Catel.Logging.LogBatchEntry> batchEntries) { }
        public static class FilePathKeyword
        {
            public const string AppData = "{AppData}";
            public const string AppDataLocal = "{AppDataLocal}";
            public const string AppDataMachine = "{AppDataMachine}";
            public const string AppDataRoaming = "{AppDataRoaming}";
            public const string AppDir = "{AppDir}";
            public const string AssemblyCompany = "{AssemblyCompany}";
            public const string AssemblyName = "{AssemblyName}";
            public const string AssemblyProduct = "{AssemblyProduct}";
            public const string AutoLogFileName = "{AutoLogFileName}";
            public const string Date = "{Date}";
            public const string ProcessId = "{ProcessId}";
            public const string Time = "{Time}";
            public const string WorkDir = "{WorkDir}";
        }
    }
    public interface IBatchLogListener
    {
        System.Threading.Tasks.Task FlushAsync();
    }
    public static class IBatchLogListenerExtensions { }
    public interface ILog
    {
        int IndentLevel { get; set; }
        int IndentSize { get; set; }
        bool IsCatelLogging { get; }
        string Name { get; }
        object? Tag { get; set; }
        System.Type TargetType { get; }
        event System.EventHandler<Catel.Logging.LogMessageEventArgs>? LogMessage;
        void Indent();
        void Unindent();
        void WriteWithData(string message, Catel.Logging.LogData? logData, Catel.Logging.LogEvent logEvent);
        void WriteWithData(string message, object? extraData, Catel.Logging.LogEvent logEvent);
    }
    public interface ILogListener
    {
        bool IgnoreCatelLogging { get; set; }
        bool IsDebugEnabled { get; set; }
        bool IsErrorEnabled { get; set; }
        bool IsInfoEnabled { get; set; }
        bool IsStatusEnabled { get; set; }
        bool IsWarningEnabled { get; set; }
        Catel.Logging.TimeDisplay TimeDisplay { get; set; }
        event System.EventHandler<Catel.Logging.LogMessageEventArgs>? LogMessage;
        void Write(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object? extraData, Catel.Logging.LogData? logData, System.DateTime time);
    }
    public class Log : Catel.Logging.ILog
    {
        public Log(string name) { }
        public Log(System.Type targetType) { }
        public Log(string name, System.Type targetType) { }
        public int IndentLevel { get; set; }
        public int IndentSize { get; set; }
        public virtual bool IsCatelLogging { get; }
        public string Name { get; }
        public object? Tag { get; set; }
        public System.Type TargetType { get; }
        public event System.EventHandler<Catel.Logging.LogMessageEventArgs>? LogMessage;
        public void Indent() { }
        protected virtual bool ShouldIgnoreIfCatelLoggingIsDisabled() { }
        public void Unindent() { }
        public void WriteWithData(string message, Catel.Logging.LogData? logData, Catel.Logging.LogEvent logEvent) { }
        public void WriteWithData(string message, object? extraData, Catel.Logging.LogEvent logEvent) { }
    }
    public class LogBatchEntry : Catel.Logging.LogEntry
    {
        public LogBatchEntry(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
    }
    public class LogData : System.Collections.Generic.Dictionary<string, object>
    {
        public LogData() { }
        public LogData(System.Collections.Generic.IDictionary<string, object> values) { }
    }
    public class LogEntry
    {
        public LogEntry(Catel.Logging.LogMessageEventArgs eventArgs) { }
        public LogEntry(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
        public Catel.Logging.LogData Data { get; }
        public object? ExtraData { get; }
        public Catel.Logging.ILog Log { get; }
        public Catel.Logging.LogEvent LogEvent { get; }
        public string Message { get; }
        public System.DateTime Time { get; }
        public override string ToString() { }
    }
    [System.Flags]
    public enum LogEvent
    {
        Debug = 1,
        Info = 2,
        Warning = 4,
        Error = 8,
        Status = 16,
    }
    public static class LogExtensions
    {
        public static void Debug(this Catel.Logging.ILog log) { }
        public static void Debug(this Catel.Logging.ILog log, System.Exception exception) { }
        public static void Debug(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void Debug(this Catel.Logging.ILog log, string messageFormat, object s1) { }
        public static void Debug(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1) { }
        public static void Debug(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, params object[] args) { }
        public static void Debug(this Catel.Logging.ILog log, string messageFormat, object s1, object s2) { }
        public static void Debug(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2) { }
        public static void Debug(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3) { }
        public static void Debug(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3) { }
        public static void Debug(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3, object s4) { }
        public static void Debug(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3, object arg4) { }
        public static void Debug(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3, object arg4, object arg5) { }
        public static void Debug(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3, object s4, object s5, params object[] others) { }
        public static void DebugAndStatus(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void DebugWithData(this Catel.Logging.ILog log, string message, Catel.Logging.LogData logData) { }
        public static void DebugWithData(this Catel.Logging.ILog log, string message, object extraData = null) { }
        public static void DebugWithData(this Catel.Logging.ILog log, System.Exception exception, string message, object extraData = null) { }
        public static void Error(this Catel.Logging.ILog log) { }
        public static void Error(this Catel.Logging.ILog log, System.Exception exception) { }
        public static void Error(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void Error(this Catel.Logging.ILog log, string messageFormat, object s1) { }
        public static void Error(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1) { }
        public static void Error(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, params object[] args) { }
        public static void Error(this Catel.Logging.ILog log, string messageFormat, object s1, object s2) { }
        public static void Error(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2) { }
        public static void Error(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3) { }
        public static void Error(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3) { }
        public static void Error(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3, object s4) { }
        public static void Error(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3, object arg4) { }
        public static void Error(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3, object arg4, object arg5) { }
        public static void Error(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3, object s4, object s5, params object[] others) { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, string messageFormat, params object[] args)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, string messageFormat, object? arg1)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, System.Exception innerException, string messageFormat, object? arg0)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, System.Exception? innerException, string messageFormat, params object?[] args)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, System.Func<string, TException> createExceptionCallback, string messageFormat, object? arg1)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, System.Func<string, TException> createExceptionCallback, string messageFormat, params object?[] args)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, string messageFormat, object? arg1, object? arg2)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, System.Exception innerException, string messageFormat, object? arg0, object? arg1)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, System.Exception? innerException, System.Func<string, TException> createExceptionCallback, string messageFormat, params object?[] args)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, System.Func<string, TException> createExceptionCallback, string messageFormat, object? arg1, object? arg2)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, string messageFormat, object? arg1, object? arg2, object arg3)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, System.Exception innerException, string messageFormat, object? arg0, object? arg1, object? arg2)
            where TException : System.Exception { }
        public static System.Exception ErrorAndCreateException<TException>(this Catel.Logging.ILog log, System.Func<string, TException> createExceptionCallback, string messageFormat, object? arg1, object? arg2, object? arg3)
            where TException : System.Exception { }
        public static void ErrorAndStatus(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void ErrorWithData(this Catel.Logging.ILog log, string message, Catel.Logging.LogData logData) { }
        public static void ErrorWithData(this Catel.Logging.ILog log, string message, object extraData = null) { }
        public static void ErrorWithData(this Catel.Logging.ILog log, System.Exception exception, string message, object extraData = null) { }
        public static void Info(this Catel.Logging.ILog log) { }
        public static void Info(this Catel.Logging.ILog log, System.Exception exception) { }
        public static void Info(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void Info(this Catel.Logging.ILog log, string messageFormat, object s1) { }
        public static void Info(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1) { }
        public static void Info(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, params object[] args) { }
        public static void Info(this Catel.Logging.ILog log, string messageFormat, object s1, object s2) { }
        public static void Info(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2) { }
        public static void Info(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3) { }
        public static void Info(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3) { }
        public static void Info(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3, object s4) { }
        public static void Info(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3, object arg4) { }
        public static void Info(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3, object arg4, object arg5) { }
        public static void Info(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3, object s4, object s5, params object[] others) { }
        public static void InfoAndStatus(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void InfoWithData(this Catel.Logging.ILog log, string message, Catel.Logging.LogData logData) { }
        public static void InfoWithData(this Catel.Logging.ILog log, string message, object extraData = null) { }
        public static void InfoWithData(this Catel.Logging.ILog log, System.Exception exception, string message, object extraData = null) { }
        public static bool IsCatelLoggingAndCanBeIgnored(this Catel.Logging.ILog log) { }
        public static void LogDebugHeading(this Catel.Logging.ILog log, string headingContent, string messageFormat, params object[] args) { }
        public static void LogDebugHeading1(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void LogDebugHeading2(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void LogDebugHeading3(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void LogDeviceInfo(this Catel.Logging.ILog log) { }
        public static void LogErrorHeading(this Catel.Logging.ILog log, string headingContent, string messageFormat, params object[] args) { }
        public static void LogErrorHeading1(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void LogErrorHeading2(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void LogErrorHeading3(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void LogHeading(this Catel.Logging.ILog log, Catel.Logging.LogEvent logEvent, string headingContent, string messageFormat, params object[] args) { }
        public static void LogInfoHeading(this Catel.Logging.ILog log, string headingContent, string messageFormat, params object[] args) { }
        public static void LogInfoHeading1(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void LogInfoHeading2(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void LogInfoHeading3(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void LogProductInfo(this Catel.Logging.ILog log) { }
        public static void LogWarningHeading(this Catel.Logging.ILog log, string headingContent, string messageFormat, params object[] args) { }
        public static void LogWarningHeading1(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void LogWarningHeading2(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void LogWarningHeading3(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void Status(this Catel.Logging.ILog log) { }
        public static void Status(this Catel.Logging.ILog log, System.Exception exception) { }
        public static void Status(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void Status(this Catel.Logging.ILog log, string messageFormat, object s1) { }
        public static void Status(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1) { }
        public static void Status(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, params object[] args) { }
        public static void Status(this Catel.Logging.ILog log, string messageFormat, object s1, object s2) { }
        public static void Status(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2) { }
        public static void Status(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3) { }
        public static void Status(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3) { }
        public static void Status(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3, object s4) { }
        public static void Status(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3, object arg4) { }
        public static void Status(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3, object arg4, object arg5) { }
        public static void Status(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3, object s4, object s5, params object[] others) { }
        public static void StatusAndStatus(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void StatusWithData(this Catel.Logging.ILog log, string message, Catel.Logging.LogData logData) { }
        public static void StatusWithData(this Catel.Logging.ILog log, string message, object extraData = null) { }
        public static void StatusWithData(this Catel.Logging.ILog log, System.Exception exception, string message, object extraData = null) { }
        public static void Warning(this Catel.Logging.ILog log) { }
        public static void Warning(this Catel.Logging.ILog log, System.Exception exception) { }
        public static void Warning(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void Warning(this Catel.Logging.ILog log, string messageFormat, object s1) { }
        public static void Warning(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1) { }
        public static void Warning(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, params object[] args) { }
        public static void Warning(this Catel.Logging.ILog log, string messageFormat, object s1, object s2) { }
        public static void Warning(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2) { }
        public static void Warning(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3) { }
        public static void Warning(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3) { }
        public static void Warning(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3, object s4) { }
        public static void Warning(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3, object arg4) { }
        public static void Warning(this Catel.Logging.ILog log, System.Exception exception, string messageFormat, object arg1, object arg2, object arg3, object arg4, object arg5) { }
        public static void Warning(this Catel.Logging.ILog log, string messageFormat, object s1, object s2, object s3, object s4, object s5, params object[] others) { }
        public static void WarningAndStatus(this Catel.Logging.ILog log, string messageFormat, params object[] args) { }
        public static void WarningWithData(this Catel.Logging.ILog log, string message, Catel.Logging.LogData logData) { }
        public static void WarningWithData(this Catel.Logging.ILog log, string message, object extraData = null) { }
        public static void WarningWithData(this Catel.Logging.ILog log, System.Exception exception, string message, object extraData = null) { }
        public static void Write(this Catel.Logging.ILog log, Catel.Logging.LogEvent logEvent, string messageFormat, params object[] args) { }
        public static void Write(this Catel.Logging.ILog log, Catel.Logging.LogEvent logEvent, string messageFormat, object s1) { }
        public static void Write(this Catel.Logging.ILog log, Catel.Logging.LogEvent logEvent, System.Exception exception, string messageFormat, params object[] args) { }
        public static void Write(this Catel.Logging.ILog log, Catel.Logging.LogEvent logEvent, string messageFormat, object s1, object s2) { }
        public static void Write(this Catel.Logging.ILog log, Catel.Logging.LogEvent logEvent, string messageFormat, object s1, object s2, object s3) { }
        public static void Write(this Catel.Logging.ILog log, Catel.Logging.LogEvent logEvent, string messageFormat, object s1, object s2, object s3, object s4) { }
        public static void Write(this Catel.Logging.ILog log, Catel.Logging.LogEvent logEvent, string messageFormat, object s1, object s2, object s3, object s4, object s5, params object[] others) { }
        public static void WriteWithData(this Catel.Logging.ILog log, System.Exception exception, string message, object? extraData, Catel.Logging.LogEvent logEvent) { }
    }
    public abstract class LogListenerBase : Catel.Logging.ILogListener
    {
        protected static readonly System.Collections.Generic.Dictionary<Catel.Logging.LogEvent, string> LogEventStrings;
        protected LogListenerBase(bool ignoreCatelLogging = false) { }
        public bool IgnoreCatelLogging { get; set; }
        public bool IsDebugEnabled { get; set; }
        public bool IsErrorEnabled { get; set; }
        public bool IsInfoEnabled { get; set; }
        public bool IsStatusEnabled { get; set; }
        public bool IsWarningEnabled { get; set; }
        public Catel.Logging.TimeDisplay TimeDisplay { get; set; }
        public event System.EventHandler<Catel.Logging.LogMessageEventArgs>? LogMessage;
        protected virtual void Debug(Catel.Logging.ILog log, string message, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
        protected virtual void Error(Catel.Logging.ILog log, string message, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
        protected virtual string FormatLogEvent(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
        protected virtual void Info(Catel.Logging.ILog log, string message, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
        protected void RaiseLogMessage(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
        protected virtual bool ShouldIgnoreLogMessage(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
        protected virtual void Status(Catel.Logging.ILog log, string message, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
        protected virtual void Warning(Catel.Logging.ILog log, string message, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
        protected virtual void Write(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
    }
    public sealed class LogListenerConfiguration : System.Configuration.ConfigurationElement
    {
        public LogListenerConfiguration() { }
        [System.Configuration.ConfigurationProperty("type", IsRequired=true)]
        public string Type { get; set; }
        public Catel.Logging.ILogListener GetLogListener(System.Reflection.Assembly? assembly = null) { }
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value) { }
    }
    public sealed class LogListenerConfigurationCollection : System.Configuration.ConfigurationElementCollection
    {
        public LogListenerConfigurationCollection() { }
        public override System.Configuration.ConfigurationElementCollectionType CollectionType { get; }
        protected override System.Configuration.ConfigurationElement CreateNewElement() { }
        protected override object GetElementKey(System.Configuration.ConfigurationElement element) { }
        protected override bool IsElementName(string elementName) { }
    }
    public static class LogManager
    {
        public static bool? IgnoreCatelLogging { get; set; }
        public static bool? IgnoreDuplicateExceptionLogging { get; set; }
        public static bool? IsDebugEnabled { get; set; }
        public static bool? IsErrorEnabled { get; set; }
        public static bool? IsInfoEnabled { get; set; }
        public static bool? IsStatusEnabled { get; set; }
        public static bool? IsWarningEnabled { get; set; }
        public  static  event System.EventHandler<Catel.Logging.LogMessageEventArgs>? LogMessage;
        public static Catel.Logging.ILogListener AddDebugListener(bool ignoreCatelLogging = false) { }
        public static void AddListener(Catel.Logging.ILogListener listener) { }
        public static void ClearListeners() { }
        public static System.Threading.Tasks.Task FlushAllAsync() { }
        public static Catel.Logging.ILog GetCurrentClassLogger() { }
        public static System.Collections.Generic.IEnumerable<Catel.Logging.ILogListener> GetListeners() { }
        public static Catel.Logging.ILog GetLogger(string name) { }
        public static Catel.Logging.ILog GetLogger(System.Type type) { }
        public static Catel.Logging.ILog GetLogger(string name, System.Type type) { }
        public static Catel.Logging.ILog GetLogger<T>() { }
        public static bool IsListenerRegistered(Catel.Logging.ILogListener listener) { }
        public static void LoadListenersFromConfiguration(System.Configuration.Configuration configuration, System.Reflection.Assembly? assembly = null) { }
        public static void LoadListenersFromConfigurationFile(string configurationFilePath, System.Reflection.Assembly? assembly = null) { }
        public static void RemoveListener(Catel.Logging.ILogListener listener) { }
        public static class LogInfo
        {
            public static bool IgnoreCatelLogging { get; }
            public static bool IgnoreDuplicateExceptionLogging { get; }
            public static bool IsDebugEnabled { get; }
            public static bool IsEnabled { get; }
            public static bool IsErrorEnabled { get; }
            public static bool IsInfoEnabled { get; }
            public static bool IsStatusEnabled { get; }
            public static bool IsWarningEnabled { get; }
            public static bool IsLogEventEnabled(Catel.Logging.LogEvent logEvent) { }
        }
    }
    public class LogMessageEventArgs : System.EventArgs
    {
        public LogMessageEventArgs(Catel.Logging.ILog log, string message, object? extraData, Catel.Logging.LogData? logData, Catel.Logging.LogEvent logEvent) { }
        public LogMessageEventArgs(Catel.Logging.ILog log, string message, object? extraData, Catel.Logging.LogData? logData, Catel.Logging.LogEvent logEvent, System.DateTime time) { }
        public object? ExtraData { get; }
        public Catel.Logging.ILog Log { get; }
        public Catel.Logging.LogData LogData { get; }
        public Catel.Logging.LogEvent LogEvent { get; }
        public string Message { get; }
        public object? Tag { get; }
        public System.DateTime Time { get; }
    }
    public sealed class LoggingConfigurationSection : System.Configuration.ConfigurationSection
    {
        public LoggingConfigurationSection() { }
        [System.Configuration.ConfigurationProperty("listeners", IsDefaultCollection=false)]
        public Catel.Logging.LogListenerConfigurationCollection LogListenerConfigurationCollection { get; }
        public System.Collections.Generic.IEnumerable<Catel.Logging.ILogListener> GetLogListeners(System.Reflection.Assembly? assembly = null) { }
    }
    public class RollingInMemoryLogListener : Catel.Logging.LogListenerBase
    {
        public RollingInMemoryLogListener() { }
        public int MaximumNumberOfErrorLogEntries { get; set; }
        public int MaximumNumberOfLogEntries { get; set; }
        public int MaximumNumberOfWarningLogEntries { get; set; }
        public System.Collections.Generic.IEnumerable<Catel.Logging.LogEntry> GetErrorLogEntries() { }
        public System.Collections.Generic.IEnumerable<Catel.Logging.LogEntry> GetLogEntries() { }
        public System.Collections.Generic.IEnumerable<Catel.Logging.LogEntry> GetWarningLogEntries() { }
        protected override void Write(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object? extraData, Catel.Logging.LogData? logData, System.DateTime time) { }
    }
    public class StatusLogListener : Catel.Logging.LogListenerBase
    {
        public StatusLogListener() { }
    }
    public enum TimeDisplay
    {
        Time = 0,
        DateTime = 1,
    }
}
namespace Catel.Messaging
{
    public class CombinedMessage : Catel.Messaging.MessageBase<Catel.Messaging.CombinedMessage, bool>
    {
        public CombinedMessage() { }
        public System.Exception? Exception { get; }
        public static void SendWith(bool data, System.Exception exception, object? tag = null) { }
    }
    public interface IMessage { }
    public interface IMessageMediator
    {
        void CleanUp();
        bool IsMessageRegistered(System.Type messageType, object? tag = null);
        bool Register<TMessage>(object recipient, System.Action<TMessage> handler, object? tag = null)
            where TMessage :  notnull;
        bool SendMessage<TMessage>(TMessage message, object? tag = null)
            where TMessage :  notnull;
        bool Unregister<TMessage>(object recipient, System.Action<TMessage> handler, object? tag = null)
            where TMessage :  notnull;
        bool UnregisterRecipient(object recipient, object? tag = null);
        bool UnregisterRecipientAndIgnoreTags(object recipient);
    }
    public static class IMessageMediatorExtensions
    {
        public static bool IsMessageRegistered<TMessage>(this Catel.Messaging.IMessageMediator messageMediator, object? tag = null) { }
    }
    public abstract class MessageBase : Catel.Messaging.IMessage
    {
        protected MessageBase() { }
    }
    public abstract class MessageBase<TMessage, TData> : Catel.Messaging.MessageBase
        where TMessage : Catel.Messaging.MessageBase<TMessage, TData>, new ()
    {
        protected MessageBase() { }
        protected MessageBase(TData data) { }
        public TData Data { get; set; }
        public static void Register(object recipient, System.Action<TMessage> handler, object? tag = null) { }
        protected static void Send(TMessage message, object? tag = null) { }
        public static void SendWith(TData data, object? tag = null) { }
        public static void SendWith(TData data, System.Action<TMessage>? initializer, object? tag = null) { }
        public static void Unregister(object recipient, System.Action<TMessage> handler, object? tag = null) { }
        public static TMessage With(TData data) { }
    }
    public class MessageMediator : Catel.Messaging.IMessageMediator
    {
        public MessageMediator() { }
        public static Catel.Messaging.IMessageMediator Default { get; }
        public void CleanUp() { }
        public bool IsMessageRegistered(System.Type messageType, object? tag = null) { }
        public bool IsRegistered<TMessage>(object recipient, System.Action<TMessage> handler, object? tag = null) { }
        public bool Register<TMessage>(object recipient, System.Action<TMessage> handler, object? tag = null)
            where TMessage :  notnull { }
        public bool SendMessage<TMessage>(TMessage message, object? tag = null)
            where TMessage :  notnull { }
        public bool Unregister<TMessage>(object recipient, System.Action<TMessage> handler, object? tag = null)
            where TMessage :  notnull { }
        public bool UnregisterRecipient(object recipient, object? tag = null) { }
        public bool UnregisterRecipient(object recipient, object? tag, bool ignoreTag) { }
        public bool UnregisterRecipientAndIgnoreTags(object recipient) { }
    }
    public static class MessageMediatorHelper
    {
        public static void SubscribeRecipient(object instance, Catel.Messaging.IMessageMediator? messageMediator = null) { }
        public static void UnsubscribeRecipient(object instance, Catel.Messaging.IMessageMediator? messageMediator = null) { }
    }
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public sealed class MessageRecipientAttribute : System.Attribute
    {
        public MessageRecipientAttribute() { }
        public object? Tag { get; set; }
    }
    public class SimpleMessage : Catel.Messaging.MessageBase<Catel.Messaging.SimpleMessage, string>
    {
        public SimpleMessage() { }
    }
}
namespace Catel.Reflection
{
    public static class AppDomainExtensions
    {
        public static void LoadAssemblyIntoAppDomain(this System.AppDomain appDomain, System.Reflection.Assembly assembly, bool includeReferencedAssemblies = true) { }
        public static void LoadAssemblyIntoAppDomain(this System.AppDomain appDomain, System.Reflection.AssemblyName assemblyName, bool includeReferencedAssemblies = true) { }
        public static void LoadAssemblyIntoAppDomain(this System.AppDomain appDomain, string assemblyFilename, bool includeReferencedAssemblies = true) { }
        public static void LoadAssemblyIntoAppDomain(this System.AppDomain appDomain, System.Reflection.Assembly assembly, bool includeReferencedAssemblies, System.Collections.Generic.HashSet<string> alreadyLoadedAssemblies) { }
        public static void PreloadAssemblies(this System.AppDomain appDomain, string? directory = null) { }
    }
    public static class AssemblyExtensions
    {
        public static string? Company(this System.Reflection.Assembly assembly) { }
        public static string? Copyright(this System.Reflection.Assembly assembly) { }
        public static string? Description(this System.Reflection.Assembly assembly) { }
        public static System.DateTime GetBuildDateTime(this System.Reflection.Assembly assembly) { }
        public static string GetDirectory(this System.Reflection.Assembly assembly) { }
        public static string? InformationalVersion(this System.Reflection.Assembly assembly) { }
        public static string? Product(this System.Reflection.Assembly assembly) { }
        public static string? Title(this System.Reflection.Assembly assembly) { }
        public static string Version(this System.Reflection.Assembly assembly, int separatorCount = 3) { }
    }
    public static class AssemblyHelper
    {
        public static System.Type[] GetAllTypesSafely(this System.Reflection.Assembly assembly, bool logLoaderExceptions = true) { }
        public static string? GetAssemblyNameWithVersion(string assemblyNameWithoutVersion) { }
        public static System.Reflection.Assembly? GetEntryAssembly() { }
        public static System.DateTime GetLinkerTimestamp(string fileName) { }
        public static System.Collections.Generic.List<System.Reflection.Assembly> GetLoadedAssemblies() { }
        public static System.Collections.Generic.List<System.Reflection.Assembly> GetLoadedAssemblies(this System.AppDomain appDomain) { }
        public static System.Collections.Generic.List<System.Reflection.Assembly> GetLoadedAssemblies(this System.AppDomain appDomain, bool ignoreDynamicAssemblies) { }
        public static System.Reflection.Assembly GetRequiredEntryAssembly() { }
        public static bool IsDynamicAssembly(this System.Reflection.Assembly assembly) { }
    }
    public class AssemblyLoadedEventArgs : System.EventArgs
    {
        public AssemblyLoadedEventArgs(System.Reflection.Assembly assembly, System.Collections.Generic.IEnumerable<System.Type> loadedTypes) { }
        public AssemblyLoadedEventArgs(System.Reflection.Assembly assembly, System.Lazy<System.Collections.Generic.IEnumerable<System.Type>> loadedTypesLazy) { }
        public System.Reflection.Assembly Assembly { get; }
        public System.Collections.Generic.IEnumerable<System.Type> LoadedTypes { get; }
    }
    public static class BindingFlagsHelper
    {
        public const System.Reflection.BindingFlags DefaultBindingFlags = 52;
        public static System.Reflection.BindingFlags GetFinalBindingFlags(bool flattenHierarchy, bool allowStaticMembers, bool? allowNonPublicMembers = default) { }
    }
    public class CachedPropertyInfo
    {
        public CachedPropertyInfo(System.Reflection.PropertyInfo propertyInfo) { }
        public bool HasPublicGetter { get; }
        public bool HasPublicSetter { get; }
        public System.Reflection.PropertyInfo PropertyInfo { get; }
        public bool IsDecoratedWithAttribute(System.Type attributeType) { }
        public bool IsDecoratedWithAttribute<TAttribute>() { }
    }
    public class CannotGetPropertyValueException : System.Exception
    {
        public CannotGetPropertyValueException(string propertyName) { }
        public string PropertyName { get; }
    }
    public class CannotSetPropertyValueException : System.Exception
    {
        public CannotSetPropertyValueException(string propertyName) { }
        public string PropertyName { get; }
    }
    public static class CastExtensions
    {
        public static TValue CastTo<TValue>(this System.DateTime value) { }
        public static TValue CastTo<TValue>(this bool value) { }
        public static TValue CastTo<TValue>(this byte value) { }
        public static TValue CastTo<TValue>(this char value) { }
        public static TValue CastTo<TValue>(this decimal value) { }
        public static TValue CastTo<TValue>(this double value) { }
        public static TValue CastTo<TValue>(this float value) { }
        public static TValue CastTo<TValue>(this int value) { }
        public static TValue CastTo<TValue>(this long value) { }
        public static TValue CastTo<TValue>(this object value) { }
        public static TValue CastTo<TValue>(this sbyte value) { }
        public static TValue CastTo<TValue>(this short value) { }
        public static TValue CastTo<TValue>(this string value) { }
        public static TValue CastTo<TValue>(this uint value) { }
        public static TValue CastTo<TValue>(this ulong value) { }
        public static TValue CastTo<TValue>(this ushort value) { }
        public static bool CastToBoolean<TValue>(this TValue value) { }
        public static byte CastToByte<TValue>(this TValue value) { }
        public static char CastToChar<TValue>(this TValue value) { }
        public static System.DateTime CastToDateTime<TValue>(this TValue value) { }
        public static decimal CastToDecimal<TValue>(this TValue value) { }
        public static double CastToDouble<TValue>(this TValue value) { }
        public static short CastToInt16<TValue>(this TValue value) { }
        public static int CastToInt32<TValue>(this TValue value) { }
        public static long CastToInt64<TValue>(this TValue value) { }
        public static sbyte CastToSByte<TValue>(this TValue value) { }
        public static float CastToSingle<TValue>(this TValue value) { }
        public static string CastToString<TValue>(this TValue value) { }
        public static ushort CastToUInt16<TValue>(this TValue value) { }
        public static uint CastToUInt32<TValue>(this TValue value) { }
        public static ulong CastToUInt64<TValue>(this TValue value) { }
    }
    public static class DelegateExtensions
    {
        public static System.Reflection.MethodInfo GetMethodInfoEx(this System.Delegate del) { }
    }
    public static class DelegateHelper
    {
        public static System.Delegate CreateDelegate(System.Type delegateType, System.Reflection.MethodInfo methodInfo) { }
        public static System.Delegate CreateDelegate(System.Type delegateType, object target, string methodName) { }
        public static System.Delegate CreateDelegate(System.Type delegateType, System.Type targetType, string methodName) { }
        public static System.Delegate CreateDelegate(System.Type delegateType, object? target, System.Reflection.MethodInfo methodInfo) { }
    }
    public class FastMemberInvoker<TEntity> : Catel.Reflection.IFastMemberInvoker
    {
        public FastMemberInvoker() { }
        protected virtual System.Action<TEntity, TMemberType> Compile<TMemberType>(System.Linq.Expressions.Expression<System.Action<TEntity, TMemberType>> expression) { }
        protected virtual System.Func<TEntity, TMemberType> Compile<TMemberType>(System.Linq.Expressions.Expression<System.Func<TEntity, TMemberType>> expression) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out bool item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out byte item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out char item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out System.DateTime item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out decimal item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out double item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out short item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out int item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out long item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out object item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out sbyte item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out float item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out string item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out ushort item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out uint item) { }
        public bool TryGetFieldValue(TEntity entity, string fieldName, out ulong item) { }
        public bool TryGetFieldValue<TValue>(object entity, string fieldName, out TValue value) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out bool item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out byte item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out char item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out System.DateTime item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out decimal item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out double item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out short item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out int item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out long item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out object item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out sbyte item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out float item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out string item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out ushort item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out uint item) { }
        public bool TryGetPropertyValue(TEntity entity, string propertyName, out ulong item) { }
        public bool TryGetPropertyValue<TValue>(object entity, string propertyName, out TValue value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, bool value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, byte value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, char value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, System.DateTime value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, decimal value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, double value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, short value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, int value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, long value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, object value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, sbyte value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, float value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, string value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, ushort value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, uint value) { }
        public bool TrySetFieldValue(TEntity entity, string fieldName, ulong value) { }
        public bool TrySetFieldValue<TValue>(object entity, string fieldName, TValue value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, bool value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, byte value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, char value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, System.DateTime value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, decimal value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, double value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, short value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, int value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, long value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, object value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, sbyte value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, float value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, string value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, ushort value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, uint value) { }
        public bool TrySetPropertyValue(TEntity entity, string propertyName, ulong value) { }
        public bool TrySetPropertyValue<TValue>(object entity, string propertyName, TValue value) { }
    }
    public interface IEntryAssemblyResolver
    {
        System.Reflection.Assembly Resolve();
    }
    public interface IFastMemberInvoker
    {
        bool TryGetFieldValue<TValue>(object entity, string fieldName, out TValue value);
        bool TryGetPropertyValue<TValue>(object entity, string propertyName, out TValue value);
        bool TrySetFieldValue<TValue>(object entity, string fieldName, TValue value);
        bool TrySetPropertyValue<TValue>(object entity, string propertyName, TValue value);
    }
    public static class MemberInfoExtensions
    {
        public static string GetSignature(this System.Reflection.ConstructorInfo constructorInfo) { }
        public static string GetSignature(this System.Reflection.MethodInfo methodInfo) { }
        public static bool IsStatic(this System.Reflection.PropertyInfo propertyInfo) { }
        public static System.Collections.Generic.IEnumerable<System.Reflection.ConstructorInfo> SortByParametersMatchDistance(this System.Collections.Generic.List<System.Reflection.ConstructorInfo> constructors, object?[] parameters) { }
        public static bool TryGetConstructorDistanceByParametersMatch(this System.Reflection.ConstructorInfo constructor, object?[] parameters, out int distance) { }
    }
    public static class ObjectExtensions
    {
        public static System.Attribute[] ToAttributeArray(this object[] objects) { }
    }
    public static class PropertyHelper
    {
        public static TValue GetHiddenPropertyValue<TValue>(object obj, string property, System.Type baseType) { }
        public static System.Reflection.PropertyInfo? GetPropertyInfo(object obj, string property, bool ignoreCase = false) { }
        public static string GetPropertyName(System.Linq.Expressions.Expression propertyExpression, bool allowNested = false) { }
        public static string GetPropertyName<TValue>(System.Linq.Expressions.Expression<System.Func<TValue>> propertyExpression, bool allowNested = false) { }
        public static string GetPropertyName<TModel, TValue>(System.Linq.Expressions.Expression<System.Func<TModel, TValue>> propertyExpression, bool allowNested = false) { }
        public static object GetPropertyValue(object obj, string property, bool ignoreCase = false) { }
        public static TValue GetPropertyValue<TValue>(object obj, string property, bool ignoreCase = false) { }
        public static bool IsPropertyAvailable(object obj, string property, bool ignoreCase = false) { }
        public static bool IsPublicProperty(object obj, string property, bool ignoreCase = false) { }
        public static void SetPropertyValue(object obj, string property, object? value, bool ignoreCase = false) { }
        public static bool TryGetPropertyValue(object obj, string property, out object value) { }
        public static bool TryGetPropertyValue(object obj, string property, bool ignoreCase, out object value) { }
        public static bool TryGetPropertyValue<TValue>(object obj, string property, out TValue value) { }
        public static bool TryGetPropertyValue<TValue>(object obj, string property, bool ignoreCase, out TValue value) { }
        public static bool TrySetPropertyValue(object obj, string property, object? value, bool ignoreCase = false) { }
    }
    public class PropertyNotFoundException : System.Exception
    {
        public PropertyNotFoundException(string propertyName) { }
        public string PropertyName { get; }
    }
    public static class ReflectionExtensions
    {
        public static bool ContainsGenericParametersEx(this System.Type type) { }
        public static System.Reflection.Assembly GetAssemblyEx(this System.Type type) { }
        public static string? GetAssemblyFullNameEx(this System.Type type) { }
        public static System.Attribute? GetAttribute(this System.Reflection.MemberInfo memberInfo, System.Type attributeType) { }
        public static System.Attribute? GetAttribute(this System.Type type, System.Type attributeType) { }
        public static TAttribute? GetAttribute<TAttribute>(this System.Reflection.MemberInfo memberInfo)
            where TAttribute : System.Attribute { }
        public static TAttribute? GetAttribute<TAttribute>(this System.Type type)
            where TAttribute : System.Attribute { }
        public static System.Type? GetBaseTypeEx(this System.Type type) { }
        public static System.Reflection.ConstructorInfo? GetConstructorEx(this System.Type type, System.Type[] types) { }
        public static System.Reflection.ConstructorInfo[] GetConstructorsEx(this System.Type type) { }
        public static System.Attribute? GetCustomAttributeEx(this System.Reflection.Assembly assembly, System.Type attributeType) { }
        public static System.Attribute? GetCustomAttributeEx(this System.Reflection.MethodInfo methodInfo, System.Type attributeType, bool inherit) { }
        public static System.Attribute? GetCustomAttributeEx(this System.Reflection.PropertyInfo propertyInfo, System.Type attributeType, bool inherit) { }
        public static System.Attribute? GetCustomAttributeEx(this System.Type type, System.Type attributeType, bool inherit) { }
        public static TAttribute? GetCustomAttributeEx<TAttribute>(this System.Reflection.PropertyInfo propertyInfo, bool inherit)
            where TAttribute : System.Attribute { }
        public static System.Attribute[] GetCustomAttributesEx(this System.Reflection.Assembly assembly, System.Type attributeType) { }
        public static System.Attribute[] GetCustomAttributesEx(this System.Reflection.MethodInfo methodInfo, bool inherit) { }
        public static System.Attribute[] GetCustomAttributesEx(this System.Reflection.PropertyInfo propertyInfo, bool inherit) { }
        public static System.Attribute[] GetCustomAttributesEx(this System.Type type, bool inherit) { }
        public static System.Attribute[] GetCustomAttributesEx(this System.Reflection.MethodInfo methodInfo, System.Type attributeType, bool inherit) { }
        public static System.Attribute[] GetCustomAttributesEx(this System.Reflection.PropertyInfo propertyInfo, System.Type attributeType, bool inherit) { }
        public static System.Attribute[] GetCustomAttributesEx(this System.Type type, System.Type attributeType, bool inherit) { }
        public static System.Type? GetElementTypeEx(this System.Type type) { }
        public static System.Reflection.EventInfo? GetEventEx(this System.Type type, string name, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.EventInfo? GetEventEx(this System.Type type, string name, bool flattenHierarchy = true, bool allowStaticMembers = false) { }
        public static System.Reflection.EventInfo[] GetEventsEx(this System.Type type, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.EventInfo[] GetEventsEx(this System.Type type, bool flattenHierarchy = true, bool allowStaticMembers = false) { }
        public static System.Type[] GetExportedTypesEx(this System.Reflection.Assembly assembly) { }
        public static System.Reflection.FieldInfo? GetFieldEx(this System.Type type, string name, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.FieldInfo? GetFieldEx(this System.Type type, string name, bool flattenHierarchy = true, bool allowStaticMembers = false) { }
        public static System.Reflection.FieldInfo[] GetFieldsEx(this System.Type type, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.FieldInfo[] GetFieldsEx(this System.Type type, bool flattenHierarchy = true, bool allowStaticMembers = false) { }
        public static System.Reflection.FieldInfo[] GetFieldsEx(this System.Type type, System.Reflection.BindingFlags bindingFlags, bool flattenMembers) { }
        public static System.Type[] GetGenericArgumentsEx(this System.Type type) { }
        public static System.Type GetGenericTypeDefinitionEx(this System.Type type) { }
        public static System.Type? GetInterfaceEx(this System.Type type, string name, bool ignoreCase) { }
        public static System.Type[] GetInterfacesEx(this System.Type type) { }
        public static System.Reflection.MemberInfo[] GetMemberEx(this System.Type type, string name, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.MemberInfo[] GetMemberEx(this System.Type type, string name, bool flattenHierarchy = true, bool allowStaticMembers = false) { }
        public static System.Reflection.MethodInfo? GetMethodEx(this System.Type type, string name, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.MethodInfo? GetMethodEx(this System.Type type, string name, bool flattenHierarchy = true, bool allowStaticMembers = false) { }
        public static System.Reflection.MethodInfo? GetMethodEx(this System.Type type, string name, System.Type[] types, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.MethodInfo? GetMethodEx(this System.Type type, string name, System.Type[] types, bool flattenHierarchy = true, bool allowStaticMembers = false) { }
        public static System.Reflection.MethodInfo[] GetMethodsEx(this System.Type type, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.MethodInfo[] GetMethodsEx(this System.Type type, bool flattenHierarchy = true, bool allowStaticMembers = false) { }
        public static System.Reflection.MethodInfo[] GetMethodsEx(this System.Type type, System.Reflection.BindingFlags bindingFlags, bool flattenMembers) { }
        public static System.Collections.Generic.IEnumerable<System.Type> GetParentTypes(this System.Type type) { }
        public static System.Reflection.PropertyInfo[] GetPropertiesEx(this System.Type type, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.PropertyInfo[] GetPropertiesEx(this System.Type type, bool flattenHierarchy = true, bool allowStaticMembers = false) { }
        public static System.Reflection.PropertyInfo[] GetPropertiesEx(this System.Type type, System.Reflection.BindingFlags bindingFlags, bool flattenMembers) { }
        public static System.Reflection.PropertyInfo? GetPropertyEx(this System.Type type, string name, System.Reflection.BindingFlags bindingFlags, bool allowExplicitInterfaceProperties = true) { }
        public static System.Reflection.PropertyInfo? GetPropertyEx(this System.Type type, string name, bool flattenHierarchy = true, bool allowStaticMembers = false, bool allowExplicitInterfaceProperties = true) { }
        public static string GetSafeFullName(this System.Type type, bool fullyQualifiedAssemblyName = false) { }
        public static int GetTypeDistance(this System.Type fromType, System.Type toType) { }
        public static System.Type[] GetTypesEx(this System.Reflection.Assembly assembly) { }
        public static bool HasBaseTypeEx(this System.Type type, System.Type typeToCheck) { }
        public static bool ImplementsInterfaceEx(this System.Type type, System.Type interfaceType) { }
        public static bool ImplementsInterfaceEx<TInterface>(this System.Type type) { }
        public static bool IsAbstractEx(this System.Type type) { }
        public static bool IsArrayEx(this System.Type type) { }
        public static bool IsAssignableFromEx(this System.Type type, System.Type typeToCheck) { }
        public static bool IsCOMObjectEx(this System.Type type) { }
        public static bool IsCatelType(this System.Type type) { }
        public static bool IsClassEx(this System.Type type) { }
        public static bool IsDecoratedWithAttribute(this System.Reflection.MemberInfo memberInfo, System.Type attributeType) { }
        public static bool IsDecoratedWithAttribute(this System.Type type, System.Type attributeType) { }
        public static bool IsDecoratedWithAttribute<TAttribute>(this System.Reflection.MemberInfo memberInfo) { }
        public static bool IsDecoratedWithAttribute<TAttribute>(this System.Type type) { }
        public static bool IsEnumEx(this System.Type type) { }
        public static bool IsGenericTypeDefinitionEx(this System.Type type) { }
        public static bool IsGenericTypeEx(this System.Type type) { }
        public static bool IsInstanceOfTypeEx(this System.Type type, object objectToCheck) { }
        public static bool IsInstanceOfTypeEx<T>(this System.Type type, T objectToCheck) { }
        public static bool IsInterfaceEx(this System.Type type) { }
        public static bool IsNestedPublicEx(this System.Type type) { }
        public static bool IsPrimitiveEx(this System.Type type) { }
        public static bool IsPublicEx(this System.Type type) { }
        public static bool IsSerializableEx(this System.Type type) { }
        public static bool IsValueTypeEx(this System.Type type) { }
        public static System.Type MakeGenericTypeEx(this System.Type type, System.Type typeArgument) { }
        public static System.Type MakeGenericTypeEx(this System.Type type, params System.Type[] typeArguments) { }
        public static bool TryGetAttribute(this System.Reflection.MemberInfo memberInfo, System.Type attributeType, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out System.Attribute? attribute) { }
        public static bool TryGetAttribute(this System.Type type, System.Type attributeType, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out System.Attribute? attribute) { }
        public static bool TryGetAttribute<TAttribute>(this System.Reflection.MemberInfo memberInfo, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out TAttribute? attribute)
            where TAttribute : System.Attribute { }
        public static bool TryGetAttribute<TAttribute>(this System.Type type, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out TAttribute? attribute)
            where TAttribute : System.Attribute { }
    }
    public static class StaticHelper
    {
        public static System.Type? GetCallingType() { }
    }
    public static class TypeArray
    {
        public static System.Type[] From<T>() { }
        public static System.Type[] From<T1, T2>() { }
        public static System.Type[] From<T1, T2, T3>() { }
        public static System.Type[] From<T1, T2, T3, T4>() { }
        public static System.Type[] From<T1, T2, T3, T4, T5>() { }
    }
    public static class TypeCache
    {
        public static System.Collections.Generic.List<string> InitializedAssemblies { get; }
        public static System.Collections.Generic.List<System.Func<System.Reflection.Assembly, bool>> ShouldIgnoreAssemblyEvaluators { get; }
        public static System.Collections.Generic.List<System.Func<System.Reflection.Assembly, System.Type, bool>> ShouldIgnoreTypeEvaluators { get; }
        public  static  event System.EventHandler<Catel.Reflection.AssemblyLoadedEventArgs>? AssemblyLoaded;
        public static void Clear() { }
        public static System.Type? GetType(string typeNameWithAssembly, bool ignoreCase = false, bool allowInitialization = true) { }
        public static System.Type? GetTypeWithAssembly(string typeName, string assemblyName, bool ignoreCase = false, bool allowInitialization = true) { }
        public static System.Type? GetTypeWithoutAssembly(string typeNameWithoutAssembly, bool ignoreCase = false, bool allowInitialization = true) { }
        public static System.Type[] GetTypes(System.Func<System.Type, bool>? predicate = null, bool allowInitialization = true) { }
        public static System.Type[] GetTypesImplementingInterface(System.Type interfaceType) { }
        public static System.Type[] GetTypesOfAssembly(System.Reflection.Assembly assembly, System.Func<System.Type, bool>? predicate = null) { }
        public static void InitializeTypes(System.Reflection.Assembly? assembly = null, bool forceFullInitialization = false, bool allowMultithreadedInitialization = false) { }
        public static void InitializeTypes(string assemblyName, bool forceFullInitialization, bool allowMultithreadedInitialization = false) { }
    }
    public static class TypeCacheEvaluator
    {
        public static System.Collections.Generic.List<System.Func<System.Reflection.Assembly, bool>> AssemblyEvaluators { get; }
        public static System.Collections.Generic.List<System.Func<System.Reflection.Assembly, System.Type, bool>> TypeEvaluators { get; }
    }
    public static class TypeExtensions
    {
        public static System.Type? GetCollectionElementType(this System.Type type) { }
        public static bool IsBasicType(this System.Type type) { }
        public static bool IsClassType(this System.Type type) { }
        public static bool IsCollection(this System.Type type) { }
        public static bool IsDictionary(this System.Type type) { }
        public static bool IsModelBase(this System.Type type) { }
        public static bool IsNullableType(this System.Type type) { }
    }
    public static class TypeHelper
    {
        public static System.Collections.Generic.IEnumerable<string> MicrosoftPublicKeyTokens { get; }
        public static TOutput Cast<TOutput>(object? value) { }
        public static TOutput Cast<TOutput, TInput>(TInput? value) { }
        public static TOutput Cast<TOutput, TInput>(TInput? value, TOutput whenNullValue) { }
        public static string ConvertTypeToVersionIndependentType(string type, bool stripAssemblies = false) { }
        public static string FormatInnerTypes(System.Collections.Generic.IEnumerable<string> innerTypes, bool stripAssemblies = false) { }
        public static string FormatType(string assembly, string type) { }
        public static string? GetAssemblyName(string fullTypeName) { }
        public static string GetAssemblyNameWithoutOverhead(string fullyQualifiedAssemblyName) { }
        public static string[] GetInnerTypes(string type) { }
        public static string GetTypeName(string fullTypeName) { }
        public static string GetTypeNameWithAssembly(string fullTypeName) { }
        public static string GetTypeNameWithoutNamespace(string fullTypeName) { }
        public static string GetTypeNamespace(string fullTypeName) { }
        public static TTargetType? GetTypedInstance<TTargetType>(object instance)
            where TTargetType :  class { }
        public static bool IsSubclassOfRawGeneric(System.Type generic, System.Type toCheck) { }
        public static bool TryCast<TOutput, TInput>(TInput? value, out TOutput output) { }
    }
    public static class TypeInfoExtensions
    {
        public static System.Reflection.ConstructorInfo? GetConstructor(this System.Reflection.TypeInfo typeInfo, System.Type[] types, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.ConstructorInfo[] GetConstructors(this System.Reflection.TypeInfo typeInfo, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.EventInfo? GetEvent(this System.Reflection.TypeInfo typeInfo, string name, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.EventInfo[] GetEvents(this System.Reflection.TypeInfo typeInfo, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.FieldInfo? GetField(this System.Reflection.TypeInfo typeInfo, string name, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.FieldInfo[] GetFields(this System.Reflection.TypeInfo typeInfo, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.MemberInfo[] GetMember(this System.Reflection.TypeInfo typeInfo, string name, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.MemberInfo[] GetMembers(this System.Reflection.TypeInfo typeInfo, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.MethodInfo? GetMethod(this System.Reflection.TypeInfo typeInfo, string name, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.MethodInfo? GetMethod(this System.Reflection.TypeInfo typeInfo, string name, System.Type[] types, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.MethodInfo[] GetMethods(this System.Reflection.TypeInfo typeInfo, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.PropertyInfo[] GetProperties(this System.Reflection.TypeInfo typeInfo, System.Reflection.BindingFlags bindingFlags) { }
        public static System.Reflection.PropertyInfo? GetProperty(this System.Reflection.TypeInfo typeInfo, string name, System.Reflection.BindingFlags bindingFlags) { }
    }
}
namespace Catel.Runtime
{
    public class ReferenceEqualityComparer<TObject> : System.Collections.Generic.IEqualityComparer<TObject>
        where TObject :  class
    {
        public ReferenceEqualityComparer() { }
        public bool Equals(TObject? x, TObject? y) { }
        public int GetHashCode(TObject? obj) { }
    }
    public class ReferenceInfo
    {
        public ReferenceInfo(object instance, int? id, bool isFirstUsage) { }
        public int? Id { get; }
        public object Instance { get; }
        public bool IsFirstUsage { get; }
    }
    public class ReferenceManager
    {
        public ReferenceManager() { }
        public int Count { get; }
        public Catel.Runtime.ReferenceInfo? GetInfo(object instance, bool autoAssignId = true) { }
        public Catel.Runtime.ReferenceInfo GetInfoAt(int index) { }
        public Catel.Runtime.ReferenceInfo? GetInfoById(int id) { }
        public void RegisterManually(int id, object? instance) { }
    }
}
namespace Catel.Runtime.Serialization
{
    public class CacheInvalidatedEventArgs : System.EventArgs
    {
        public CacheInvalidatedEventArgs(System.Type type) { }
        public System.Type Type { get; }
    }
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple=false)]
    public class ExcludeFromSerializationAttribute : System.Attribute
    {
        public ExcludeFromSerializationAttribute() { }
    }
    public interface IFieldSerializable
    {
        bool GetFieldValue<T>(string fieldName, ref T value);
        bool SetFieldValue<T>(string fieldName, T value);
    }
    public interface IObjectAdapter
    {
        Catel.Runtime.Serialization.MemberValue? GetMemberValue(object model, string memberName, Catel.Runtime.Serialization.SerializationModelInfo modelInfo);
        void SetMemberValue(object model, Catel.Runtime.Serialization.MemberValue member, Catel.Runtime.Serialization.SerializationModelInfo modelInfo);
    }
    public interface IPropertySerializable
    {
        bool GetPropertyValue<T>(string propertyName, ref T value);
        bool SetPropertyValue<T>(string propertyName, T value);
    }
    public interface ISerializable
    {
        void FinishDeserialization();
        void FinishSerialization();
        void StartDeserialization();
        void StartSerialization();
    }
    public interface ISerializationConfiguration
    {
        System.Globalization.CultureInfo Culture { get; set; }
    }
    public interface ISerializationContext : System.IDisposable
    {
        Catel.Runtime.Serialization.ISerializationConfiguration? Configuration { get; }
        Catel.Runtime.Serialization.SerializationContextMode ContextMode { get; }
        int Depth { get; }
        object Model { get; set; }
        System.Type ModelType { get; }
        string ModelTypeName { get; }
        Catel.Runtime.ReferenceManager ReferenceManager { get; }
        System.Collections.Generic.Stack<System.Type> TypeStack { get; }
    }
    public interface ISerializationContextContainer
    {
        void SetSerializationContext<T>(Catel.Runtime.Serialization.ISerializationContext<T> serializationContext)
            where T :  class, Catel.Runtime.Serialization.ISerializationContextInfo;
    }
    public static class ISerializationContextExtensions
    {
        public static System.Type? FindParentType(this Catel.Runtime.Serialization.ISerializationContext serializationContext, System.Func<System.Type, bool> predicate, int maxLevels = -1) { }
    }
    public interface ISerializationContextInfo
    {
        bool ShouldAutoGenerateGraphIds(Catel.Runtime.Serialization.ISerializationContext context);
    }
    public interface ISerializationContextInfoFactory
    {
        Catel.Runtime.Serialization.ISerializationContextInfo GetSerializationContextInfo(Catel.Runtime.Serialization.ISerializer serializer, object model, object data, Catel.Runtime.Serialization.ISerializationConfiguration configuration);
    }
    public interface ISerializationContext<TSerializationContext> : Catel.Runtime.Serialization.ISerializationContext, System.IDisposable
        where TSerializationContext :  class, Catel.Runtime.Serialization.ISerializationContextInfo
    {
        TSerializationContext Context { get; }
        Catel.Runtime.Serialization.ISerializationContext<TSerializationContext>? Parent { get; }
    }
    public interface ISerializationManager
    {
        event System.EventHandler<Catel.Runtime.Serialization.CacheInvalidatedEventArgs> CacheInvalidated;
        void AddSerializerModifier(System.Type type, System.Type serializerModifierType);
        void Clear(System.Type type);
        System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetCatelProperties(System.Type type, bool includeModelBaseProperties = false);
        System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetCatelPropertiesToSerialize(System.Type type);
        System.Collections.Generic.HashSet<string> GetCatelPropertyNames(System.Type type, bool includeModelBaseProperties = false);
        System.Collections.Generic.HashSet<string> GetFieldNames(System.Type type);
        System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetFields(System.Type type);
        System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetFieldsToSerialize(System.Type type);
        System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetRegularProperties(System.Type type);
        System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetRegularPropertiesToSerialize(System.Type type);
        System.Collections.Generic.HashSet<string> GetRegularPropertyNames(System.Type type);
        Catel.Runtime.Serialization.ISerializerModifier[] GetSerializerModifiers(System.Type type);
        void RemoveSerializerModifier(System.Type type, System.Type serializerModifierType);
        void Warmup(System.Type type);
    }
    public static class ISerializationManagerExtensions
    {
        public static void AddSerializerModifier<TType, TSerializerModifier>(this Catel.Runtime.Serialization.ISerializationManager serializationManager)
            where TSerializerModifier : Catel.Runtime.Serialization.ISerializerModifier { }
        public static Catel.Runtime.Serialization.ISerializerModifier[] GetSerializerModifiers<TType>(this Catel.Runtime.Serialization.ISerializationManager serializationManager) { }
        public static void RemoveSerializerModifier<TType, TSerializerModifier>(this Catel.Runtime.Serialization.ISerializationManager serializationManager)
            where TSerializerModifier : Catel.Runtime.Serialization.ISerializerModifier { }
    }
    public interface ISerializer
    {
        event System.EventHandler<Catel.Runtime.Serialization.SerializationEventArgs>? Deserialized;
        event System.EventHandler<Catel.Runtime.Serialization.MemberSerializationEventArgs>? DeserializedMember;
        event System.EventHandler<Catel.Runtime.Serialization.SerializationEventArgs>? Deserializing;
        event System.EventHandler<Catel.Runtime.Serialization.MemberSerializationEventArgs>? DeserializingMember;
        event System.EventHandler<Catel.Runtime.Serialization.SerializationEventArgs>? Serialized;
        event System.EventHandler<Catel.Runtime.Serialization.MemberSerializationEventArgs>? SerializedMember;
        event System.EventHandler<Catel.Runtime.Serialization.SerializationEventArgs>? Serializing;
        event System.EventHandler<Catel.Runtime.Serialization.MemberSerializationEventArgs>? SerializingMember;
        object? Deserialize(object model, Catel.Runtime.Serialization.ISerializationContextInfo serializationContext, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
        object? Deserialize(object model, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
        object? Deserialize(System.Type modelType, Catel.Runtime.Serialization.ISerializationContextInfo serializationContext, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
        object? Deserialize(System.Type modelType, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
        System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(object model, Catel.Runtime.Serialization.ISerializationContextInfo serializationContext, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
        System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(object model, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
        System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(System.Type modelType, Catel.Runtime.Serialization.ISerializationContextInfo serializationContext, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
        System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(System.Type modelType, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
        void Serialize(object model, Catel.Runtime.Serialization.ISerializationContextInfo serializationContext, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
        void Serialize(object model, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
        void SerializeMembers(object model, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration, params string[] membersToIgnore);
        void Warmup(System.Collections.Generic.IEnumerable<System.Type>? types = null, int typesPerThread = 1000);
    }
    public static class ISerializerExtensions
    {
        public static TModel? Deserialize<TModel>(this Catel.Runtime.Serialization.ISerializer serializer, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
    }
    public interface ISerializerModifier
    {
        void DeserializeMember(Catel.Runtime.Serialization.ISerializationContext context, Catel.Runtime.Serialization.MemberValue memberValue);
        void OnDeserialized(Catel.Runtime.Serialization.ISerializationContext context, object model);
        void OnDeserializing(Catel.Runtime.Serialization.ISerializationContext context, object model);
        void OnSerialized(Catel.Runtime.Serialization.ISerializationContext context, object model);
        void OnSerializing(Catel.Runtime.Serialization.ISerializationContext context, object model);
        void SerializeMember(Catel.Runtime.Serialization.ISerializationContext context, Catel.Runtime.Serialization.MemberValue memberValue);
        bool ShouldIgnoreMember(Catel.Runtime.Serialization.ISerializationContext context, object model, Catel.Runtime.Serialization.MemberValue memberValue);
        bool? ShouldSerializeAsCollection();
        bool? ShouldSerializeAsDictionary();
        bool? ShouldSerializeEnumMemberUsingToString(Catel.Runtime.Serialization.MemberValue memberValue);
        bool? ShouldSerializeMemberUsingParse(Catel.Runtime.Serialization.MemberValue memberValue);
    }
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple=false)]
    public class IncludeInSerializationAttribute : System.Attribute
    {
        public IncludeInSerializationAttribute() { }
        public IncludeInSerializationAttribute(string name) { }
        public string Name { get; set; }
    }
    public class KeyValuePairSerializerModifier : Catel.Runtime.Serialization.SerializerModifierBase
    {
        public KeyValuePairSerializerModifier() { }
        public override void DeserializeMember(Catel.Runtime.Serialization.ISerializationContext context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        public override void SerializeMember(Catel.Runtime.Serialization.ISerializationContext context, Catel.Runtime.Serialization.MemberValue memberValue) { }
    }
    public class MemberMetadata
    {
        public MemberMetadata(System.Type containingType, System.Type memberType, Catel.Runtime.Serialization.SerializationMemberGroup memberGroup, string memberName) { }
        public System.Type ContainingType { get; }
        public Catel.Runtime.Serialization.SerializationMemberGroup MemberGroup { get; }
        public string MemberName { get; }
        public string MemberNameForSerialization { get; set; }
        public System.Type MemberType { get; }
        public object? Tag { get; set; }
    }
    public class MemberSerializationEventArgs : Catel.Runtime.Serialization.SerializationEventArgs
    {
        public MemberSerializationEventArgs(Catel.Runtime.Serialization.ISerializationContext serializationContext, Catel.Runtime.Serialization.MemberValue memberValue) { }
        public Catel.Runtime.Serialization.MemberValue MemberValue { get; }
    }
    [System.Diagnostics.DebuggerDisplay("{Name} => {Value} ({ActualMemberType})")]
    public class MemberValue
    {
        public MemberValue(Catel.Runtime.Serialization.SerializationMemberGroup memberGroup, System.Type modelType, System.Type memberType, string name, string nameForSerialization, object? value) { }
        public System.Type? ActualMemberType { get; }
        public Catel.Runtime.Serialization.SerializationMemberGroup MemberGroup { get; }
        public System.Type MemberType { get; }
        public string MemberTypeName { get; }
        public System.Type ModelType { get; }
        public string ModelTypeName { get; }
        public string Name { get; }
        public string NameForSerialization { get; }
        public object? Value { get; set; }
        public System.Type GetBestMemberType() { }
    }
    public class ObjectAdapter : Catel.Runtime.Serialization.IObjectAdapter
    {
        public ObjectAdapter(Catel.Data.IObjectAdapter objectAdapter) { }
        public virtual Catel.Runtime.Serialization.MemberValue? GetMemberValue(object model, string memberName, Catel.Runtime.Serialization.SerializationModelInfo modelInfo) { }
        public virtual void SetMemberValue(object model, Catel.Runtime.Serialization.MemberValue member, Catel.Runtime.Serialization.SerializationModelInfo modelInfo) { }
    }
    [System.Serializable]
    public class SerializableKeyValuePair : System.Runtime.Serialization.ISerializable
    {
        public SerializableKeyValuePair() { }
        public SerializableKeyValuePair(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public object? Key { get; set; }
        [Catel.Runtime.Serialization.ExcludeFromSerialization]
        [System.Xml.Serialization.XmlIgnore]
        public System.Type? KeyType { get; set; }
        public object? Value { get; set; }
        [Catel.Runtime.Serialization.ExcludeFromSerialization]
        [System.Xml.Serialization.XmlIgnore]
        public System.Type? ValueType { get; set; }
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public class SerializationConfiguration : Catel.Runtime.Serialization.ISerializationConfiguration
    {
        public SerializationConfiguration() { }
        public System.Globalization.CultureInfo Culture { get; set; }
    }
    public static class SerializationContextHelper
    {
        public static string GetSerializationScopeName() { }
    }
    public abstract class SerializationContextInfoBase<TSerializationContextInfo> : Catel.Runtime.Serialization.ISerializationContextContainer, Catel.Runtime.Serialization.ISerializationContextInfo
        where TSerializationContextInfo :  class, Catel.Runtime.Serialization.ISerializationContextInfo
    {
        protected SerializationContextInfoBase() { }
        public Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo>? Context { get; }
        protected virtual void OnContextUpdated(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context) { }
        public virtual bool ShouldAutoGenerateGraphIds(Catel.Runtime.Serialization.ISerializationContext context) { }
    }
    public enum SerializationContextMode
    {
        Serialization = 0,
        Deserialization = 1,
    }
    public class SerializationContextScope<TSerializationContextInfo>
        where TSerializationContextInfo :  class, Catel.Runtime.Serialization.ISerializationContextInfo
    {
        public SerializationContextScope() { }
        public System.Collections.Generic.Stack<Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo>> Contexts { get; }
        public Catel.Runtime.ReferenceManager ReferenceManager { get; }
        public System.Collections.Generic.Stack<System.Type> TypeStack { get; }
    }
    public class SerializationContext<TSerializationContextInfo> : Catel.Disposable, Catel.Runtime.Serialization.ISerializationContext, Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo>, System.IDisposable
        where TSerializationContextInfo :  class, Catel.Runtime.Serialization.ISerializationContextInfo
    {
        public SerializationContext(object model, System.Type modelType, TSerializationContextInfo context, Catel.Runtime.Serialization.SerializationContextMode contextMode, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public Catel.Runtime.Serialization.ISerializationConfiguration? Configuration { get; }
        public TSerializationContextInfo Context { get; }
        public Catel.Runtime.Serialization.SerializationContextMode ContextMode { get; }
        public System.Collections.Generic.Stack<Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo>> Contexts { get; }
        public int Depth { get; }
        public object Model { get; set; }
        public System.Type ModelType { get; }
        public string ModelTypeName { get; }
        public Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo>? Parent { get; }
        public Catel.Runtime.ReferenceManager ReferenceManager { get; }
        public System.Runtime.Serialization.SerializationInfo? SerializationInfo { get; set; }
        public System.Collections.Generic.Stack<System.Type> TypeStack { get; }
        protected override void DisposeManaged() { }
    }
    public class SerializationEventArgs : System.EventArgs
    {
        public SerializationEventArgs(Catel.Runtime.Serialization.ISerializationContext serializationContext) { }
        public Catel.Runtime.Serialization.ISerializationContext SerializationContext { get; }
    }
    public static class SerializationFactory
    {
        public static Catel.Runtime.Serialization.Xml.IXmlSerializer GetXmlSerializer() { }
    }
    public class SerializationManager : Catel.Runtime.Serialization.ISerializationManager
    {
        public SerializationManager() { }
        public event System.EventHandler<Catel.Runtime.Serialization.CacheInvalidatedEventArgs>? CacheInvalidated;
        public void AddSerializerModifier(System.Type type, System.Type serializerModifierType) { }
        public void Clear(System.Type type) { }
        protected virtual System.Collections.Generic.List<System.Type> FindSerializerModifiers(System.Type type) { }
        public System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetCatelProperties(System.Type type, bool includeModelBaseProperties = false) { }
        public virtual System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetCatelPropertiesToSerialize(System.Type type) { }
        public System.Collections.Generic.HashSet<string> GetCatelPropertyNames(System.Type type, bool includeModelBaseProperties = false) { }
        public System.Collections.Generic.HashSet<string> GetFieldNames(System.Type type) { }
        public System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetFields(System.Type type) { }
        public virtual System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetFieldsToSerialize(System.Type type) { }
        public System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetRegularProperties(System.Type type) { }
        public virtual System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> GetRegularPropertiesToSerialize(System.Type type) { }
        public System.Collections.Generic.HashSet<string> GetRegularPropertyNames(System.Type type) { }
        public virtual Catel.Runtime.Serialization.ISerializerModifier[] GetSerializerModifiers(System.Type type) { }
        public void RemoveSerializerModifier(System.Type type, System.Type serializerModifierType) { }
        public void Warmup(System.Type type) { }
    }
    public enum SerializationMemberGroup
    {
        CatelProperty = 0,
        RegularProperty = 1,
        Field = 2,
        SimpleRootObject = 3,
        Collection = 4,
        Dictionary = 5,
    }
    public class SerializationModelInfo
    {
        public SerializationModelInfo(System.Type modelType, System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> catelProperties, System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> fields, System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> regularProperties) { }
        public System.Collections.Generic.List<Catel.Data.IPropertyData> CatelProperties { get; }
        public System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> CatelPropertiesByName { get; }
        public System.Collections.Generic.HashSet<string> CatelPropertyNames { get; }
        public System.Collections.Generic.HashSet<string> FieldNames { get; }
        public System.Collections.Generic.List<System.Reflection.FieldInfo> Fields { get; }
        public System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> FieldsByName { get; }
        public System.Type ModelType { get; }
        public System.Collections.Generic.List<System.Reflection.PropertyInfo> Properties { get; }
        public System.Collections.Generic.Dictionary<string, Catel.Runtime.Serialization.MemberMetadata> PropertiesByName { get; }
        public System.Collections.Generic.HashSet<string> PropertyNames { get; }
    }
    public class SerializationObject
    {
        public bool IsSuccessful { get; }
        public Catel.Runtime.Serialization.SerializationMemberGroup MemberGroup { get; }
        public string MemberName { get; }
        public object? MemberValue { get; }
        public System.Type ModelType { get; }
        public static Catel.Runtime.Serialization.SerializationObject FailedToDeserialize(System.Type modelType, Catel.Runtime.Serialization.SerializationMemberGroup memberGroup, string memberName) { }
        public static Catel.Runtime.Serialization.SerializationObject SucceededToDeserialize(System.Type modelType, Catel.Runtime.Serialization.SerializationMemberGroup memberGroup, string memberName, object? memberValue) { }
    }
    public class SerializationScope
    {
        public SerializationScope(Catel.Runtime.Serialization.ISerializer serializer, Catel.Runtime.Serialization.ISerializationConfiguration? configuration) { }
        public Catel.Runtime.Serialization.ISerializationConfiguration? Configuration { get; set; }
        public Catel.Runtime.Serialization.ISerializer Serializer { get; }
    }
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class SerializeAsCollectionAttribute : System.Attribute
    {
        public SerializeAsCollectionAttribute() { }
    }
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple=false)]
    public class SerializeEnumAsStringAttribute : System.Attribute
    {
        public SerializeEnumAsStringAttribute() { }
    }
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class SerializeUsingParseAndToStringAttribute : System.Attribute
    {
        public SerializeUsingParseAndToStringAttribute() { }
    }
    public abstract class SerializerBase<TSerializationContextInfo> : Catel.Runtime.Serialization.ISerializer
        where TSerializationContextInfo :  class, Catel.Runtime.Serialization.ISerializationContextInfo
    {
        protected const string CollectionName = "Items";
        protected const string DictionaryName = "Pairs";
        protected const string RootObjectName = "Value";
        protected SerializerBase(Catel.Runtime.Serialization.ISerializationManager serializationManager, Catel.IoC.ITypeFactory typeFactory, Catel.Runtime.Serialization.IObjectAdapter objectAdapter) { }
        protected Catel.Runtime.Serialization.IObjectAdapter ObjectAdapter { get; }
        protected Catel.Runtime.Serialization.ISerializationManager SerializationManager { get; }
        protected Catel.IoC.ITypeFactory TypeFactory { get; }
        public event System.EventHandler<Catel.Runtime.Serialization.SerializationEventArgs>? Deserialized;
        public event System.EventHandler<Catel.Runtime.Serialization.MemberSerializationEventArgs>? DeserializedMember;
        public event System.EventHandler<Catel.Runtime.Serialization.SerializationEventArgs>? Deserializing;
        public event System.EventHandler<Catel.Runtime.Serialization.MemberSerializationEventArgs>? DeserializingMember;
        public event System.EventHandler<Catel.Runtime.Serialization.SerializationEventArgs>? Serialized;
        public event System.EventHandler<Catel.Runtime.Serialization.MemberSerializationEventArgs>? SerializedMember;
        public event System.EventHandler<Catel.Runtime.Serialization.SerializationEventArgs>? Serializing;
        public event System.EventHandler<Catel.Runtime.Serialization.MemberSerializationEventArgs>? SerializingMember;
        protected virtual void AfterDeserialization(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context) { }
        protected virtual void AfterDeserializeMember(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected virtual void AfterSerialization(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context) { }
        protected virtual void AfterSerializeMember(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected abstract void AppendContextToStream(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, System.IO.Stream stream);
        protected virtual void BeforeDeserialization(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context) { }
        protected virtual void BeforeDeserializeMember(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected virtual void BeforeSerialization(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context) { }
        protected virtual void BeforeSerializeMember(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected System.Collections.Generic.List<Catel.Runtime.Serialization.SerializableKeyValuePair> ConvertDictionaryToCollection(object? memberValue) { }
        protected virtual object CreateModelInstance(System.Type type) { }
        protected virtual object? Deserialize(object model, Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context) { }
        public object? Deserialize(object model, Catel.Runtime.Serialization.ISerializationContextInfo serializationContext, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public virtual object? Deserialize(object model, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public virtual object? Deserialize(object model, TSerializationContextInfo serializationContext, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public object? Deserialize(System.Type modelType, Catel.Runtime.Serialization.ISerializationContextInfo serializationContext, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public virtual object? Deserialize(System.Type modelType, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public virtual object? Deserialize(System.Type modelType, TSerializationContextInfo serializationContext, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        protected abstract Catel.Runtime.Serialization.SerializationObject DeserializeMember(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue);
        protected virtual System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context) { }
        public System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(object model, Catel.Runtime.Serialization.ISerializationContextInfo serializationContextInfo, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public virtual System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(object model, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public virtual System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(object model, TSerializationContextInfo serializationContext, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(System.Type modelType, Catel.Runtime.Serialization.ISerializationContextInfo serializationContextInfo, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public virtual System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(System.Type modelType, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public virtual System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(System.Type modelType, TSerializationContextInfo serializationContext, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        protected virtual object? DeserializeUsingObjectParse(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected virtual Catel.Runtime.Serialization.MemberValue? EndMemberDeserialization(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue member, Catel.Runtime.Serialization.SerializationObject serializationObject, System.Collections.Generic.IEnumerable<Catel.Runtime.Serialization.ISerializerModifier> serializerModifiers) { }
        protected void EndMemberSerialization(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue member) { }
        protected Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> GetContext(System.Type modelType, System.IO.Stream stream, Catel.Runtime.Serialization.SerializationContextMode contextMode, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        protected Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> GetContext(System.Type modelType, TSerializationContextInfo context, Catel.Runtime.Serialization.SerializationContextMode contextMode, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        protected virtual Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> GetContext(object model, System.Type modelType, TSerializationContextInfo context, Catel.Runtime.Serialization.SerializationContextMode contextMode, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        protected virtual Catel.Runtime.Serialization.ISerializationConfiguration GetCurrentSerializationConfiguration(Catel.Runtime.Serialization.ISerializationConfiguration? configuration) { }
        protected virtual Catel.Scoping.ScopeManager<Catel.Runtime.Serialization.SerializationScope> GetCurrentSerializationScopeManager(Catel.Runtime.Serialization.ISerializationConfiguration? configuration) { }
        protected Catel.Runtime.Serialization.SerializationMemberGroup GetMemberGroup(System.Type modelType, string memberName) { }
        protected System.Type? GetMemberType(System.Type modelType, string memberName) { }
        protected virtual System.Reflection.MethodInfo? GetObjectParseMethod(System.Type memberType) { }
        protected virtual System.Reflection.MethodInfo? GetObjectToStringMethod(System.Type memberType) { }
        public virtual System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> GetSerializableMembers(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, object model, params string[] membersToIgnore) { }
        protected abstract Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> GetSerializationContextInfo(object model, System.Type modelType, System.IO.Stream stream, Catel.Runtime.Serialization.SerializationContextMode contextMode, Catel.Runtime.Serialization.ISerializationConfiguration? configuration);
        protected virtual bool IsRootCollection(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected virtual bool IsRootDictionary(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected virtual bool IsRootObject(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue, System.Func<Catel.Runtime.Serialization.MemberValue, bool> predicate) { }
        protected virtual void PopulateModel(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> members) { }
        protected virtual void Serialize(object model, Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context) { }
        public void Serialize(object model, Catel.Runtime.Serialization.ISerializationContextInfo context, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public virtual void Serialize(object model, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        public virtual void Serialize(object model, TSerializationContextInfo context, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        protected abstract void SerializeMember(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue);
        protected virtual void SerializeMembers(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> membersToSerialize) { }
        public virtual void SerializeMembers(object model, System.IO.Stream stream, Catel.Runtime.Serialization.ISerializationConfiguration? configuration, params string[] membersToIgnore) { }
        protected virtual string? SerializeUsingObjectToString(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected virtual bool ShouldExternalSerializerHandleMember(Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected virtual bool ShouldExternalSerializerHandleMember(System.Type memberType) { }
        protected virtual bool ShouldIgnoreMember(object model, string propertyName) { }
        protected bool ShouldSerializeAsCollection(Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected virtual bool ShouldSerializeAsCollection(System.Type memberType) { }
        protected bool ShouldSerializeAsDictionary(Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected virtual bool ShouldSerializeAsDictionary(System.Type memberType) { }
        protected virtual bool ShouldSerializeEnumAsString(Catel.Runtime.Serialization.MemberValue memberValue, bool checkActualMemberType) { }
        protected virtual bool ShouldSerializeModelAsCollection(System.Type memberType) { }
        protected virtual bool ShouldSerializeUsingParseAndToString(Catel.Runtime.Serialization.MemberValue memberValue, bool checkActualMemberType) { }
        protected virtual void StartMemberDeserialization(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue member) { }
        protected bool StartMemberSerialization(Catel.Runtime.Serialization.ISerializationContext<TSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue member, Catel.Runtime.Serialization.ISerializerModifier[] serializerModifiers) { }
        protected abstract void Warmup(System.Type type);
        public void Warmup(System.Collections.Generic.IEnumerable<System.Type>? types = null, int typesPerThread = 1000) { }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public class SerializerModifierAttribute : System.Attribute
    {
        public SerializerModifierAttribute(System.Type serializerModifierType) { }
        public System.Type SerializerModifierType { get; }
    }
    public abstract class SerializerModifierBase : Catel.Runtime.Serialization.ISerializerModifier
    {
        protected SerializerModifierBase() { }
        public virtual void DeserializeMember(Catel.Runtime.Serialization.ISerializationContext context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        public virtual void OnDeserialized(Catel.Runtime.Serialization.ISerializationContext context, object model) { }
        public virtual void OnDeserializing(Catel.Runtime.Serialization.ISerializationContext context, object model) { }
        public virtual void OnSerialized(Catel.Runtime.Serialization.ISerializationContext context, object model) { }
        public virtual void OnSerializing(Catel.Runtime.Serialization.ISerializationContext context, object model) { }
        public virtual void SerializeMember(Catel.Runtime.Serialization.ISerializationContext context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        public virtual bool ShouldIgnoreMember(Catel.Runtime.Serialization.ISerializationContext context, object model, Catel.Runtime.Serialization.MemberValue memberValue) { }
        public virtual bool? ShouldSerializeAsCollection() { }
        public virtual bool? ShouldSerializeAsDictionary() { }
        public virtual bool? ShouldSerializeEnumMemberUsingToString(Catel.Runtime.Serialization.MemberValue memberValue) { }
        public virtual bool? ShouldSerializeMemberUsingParse(Catel.Runtime.Serialization.MemberValue memberValue) { }
    }
    public abstract class SerializerModifierBase<TModel> : Catel.Runtime.Serialization.SerializerModifierBase
    {
        protected SerializerModifierBase() { }
        public virtual void OnDeserialized(Catel.Runtime.Serialization.ISerializationContext context, TModel model) { }
        public virtual void OnDeserializing(Catel.Runtime.Serialization.ISerializationContext context, TModel model) { }
        public virtual void OnSerialized(Catel.Runtime.Serialization.ISerializationContext context, TModel model) { }
        public virtual void OnSerializing(Catel.Runtime.Serialization.ISerializationContext context, TModel model) { }
    }
    public static class XmlReaderExtensions
    {
        public static bool MoveToNextContentElement(this System.Xml.XmlReader xmlReader, string startMember) { }
    }
}
namespace Catel.Runtime.Serialization.Xml
{
    public class DataContractSerializerFactory : Catel.Runtime.Serialization.Xml.IDataContractSerializerFactory
    {
        public DataContractSerializerFactory() { }
        public System.Runtime.Serialization.DataContractResolver? DataContractResolver { get; set; }
        protected virtual bool AddTypeToKnownTypesIfSerializable(System.Type typeToAdd, Catel.Runtime.Serialization.Xml.DataContractSerializerFactory.XmlSerializerTypeInfo serializerTypeInfo) { }
        protected virtual bool AllowNonPublicReflection(System.Type type) { }
        public virtual System.Runtime.Serialization.DataContractSerializer GetDataContractSerializer(System.Type serializingType, System.Type typeToSerialize, string xmlName, string? rootNamespace = null, System.Collections.Generic.List<System.Type>? additionalKnownTypes = null) { }
        public virtual System.Collections.Generic.List<System.Type> GetKnownTypes(System.Type serializingType, System.Type typeToSerialize, System.Collections.Generic.List<System.Type>? additionalKnownTypes = null) { }
        protected virtual void GetKnownTypes(System.Type type, Catel.Runtime.Serialization.Xml.DataContractSerializerFactory.XmlSerializerTypeInfo serializerTypeInfo, bool resolveAbstractClassesAndInterfaces = true) { }
        protected virtual System.Type[] GetKnownTypesViaAttributes(System.Type type) { }
        protected virtual bool IsTypeSerializable(System.Type type, Catel.Runtime.Serialization.Xml.DataContractSerializerFactory.XmlSerializerTypeInfo serializerTypeInfo) { }
        protected virtual bool ShouldTypeBeIgnored(System.Type type, Catel.Runtime.Serialization.Xml.DataContractSerializerFactory.XmlSerializerTypeInfo serializerTypeInfo) { }
        protected class XmlSerializerTypeInfo
        {
            public XmlSerializerTypeInfo(System.Type serializingType, System.Type typeToSerialize, System.Collections.Generic.IEnumerable<System.Type>? additionalKnownTypes = null) { }
            public System.Collections.Generic.IEnumerable<System.Type> KnownTypes { get; }
            public System.Type SerializingType { get; }
            public System.Collections.Generic.IEnumerable<System.Type> SpecialCollectionTypes { get; }
            public System.Collections.Generic.IEnumerable<System.Type> SpecialGenericCollectionTypes { get; }
            public System.Type TypeToSerialize { get; }
            public System.Collections.Generic.IEnumerable<System.Type> TypesAlreadyHandled { get; }
            public void AddCollectionAsHandled(System.Type type) { }
            public bool AddKnownType(System.Type type) { }
            public void AddTypeAsHandled(System.Type type) { }
            public bool ContainsKnownType(System.Type type) { }
            public bool IsCollectionAlreadyHandled(System.Type type) { }
            public bool IsSpecialCollectionType(System.Type type) { }
            public bool IsTypeAlreadyHandled(System.Type type) { }
            public bool IsTypeSerializable(System.Type type) { }
        }
    }
    public interface ICustomXmlSerializable
    {
        void Deserialize(System.Xml.XmlReader xmlReader);
        void Serialize(System.Xml.XmlWriter xmlWriter);
    }
    public interface IDataContractSerializerFactory
    {
        System.Runtime.Serialization.DataContractResolver? DataContractResolver { get; set; }
        System.Runtime.Serialization.DataContractSerializer GetDataContractSerializer(System.Type serializingType, System.Type typeToSerialize, string xmlName, string? rootNamespace = null, System.Collections.Generic.List<System.Type>? additionalKnownTypes = null);
        System.Collections.Generic.List<System.Type> GetKnownTypes(System.Type serializingType, System.Type typeToSerialize, System.Collections.Generic.List<System.Type>? additionalKnownTypes = null);
    }
    public interface IXmlNamespaceManager
    {
        Catel.Runtime.Serialization.Xml.XmlNamespace? GetNamespace(System.Type type, string preferredPrefix);
    }
    public interface IXmlSerializer : Catel.Runtime.Serialization.ISerializer { }
    public static class XmlHelper
    {
        public static object? ConvertToObject(System.Xml.Linq.XElement element, System.Type objectType, System.Func<object> createDefaultValue) { }
        public static System.Xml.Linq.XElement? ConvertToXml(string elementName, System.Type objectType, object objectValue) { }
    }
    public class XmlNamespace
    {
        public XmlNamespace(string prefix, string uri) { }
        public string Prefix { get; }
        public string Uri { get; }
        public override string ToString() { }
    }
    public class XmlNamespaceManager : Catel.Runtime.Serialization.Xml.IXmlNamespaceManager
    {
        public XmlNamespaceManager() { }
        public Catel.Runtime.Serialization.Xml.XmlNamespace? GetNamespace(System.Type type, string preferredPrefix) { }
    }
    public class XmlSerializationConfiguration : Catel.Runtime.Serialization.SerializationConfiguration
    {
        public XmlSerializationConfiguration() { }
        public System.Xml.XmlReaderSettings? ReaderSettings { get; set; }
        public System.Xml.XmlWriterSettings? WriterSettings { get; set; }
    }
    public class XmlSerializationContextInfo : Catel.Runtime.Serialization.SerializationContextInfoBase<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo>
    {
        public XmlSerializationContextInfo(System.Xml.XmlReader xmlReader, object model) { }
        public XmlSerializationContextInfo(System.Xml.XmlWriter xmlWriter, object model) { }
        public bool AllowCustomXmlSerialization { get; set; }
        public bool IsRootObject { get; }
        public System.Collections.Generic.HashSet<System.Type> KnownTypes { get; }
        public object? Model { get; }
        public System.Xml.XmlReader? XmlReader { get; }
        public System.Xml.XmlWriter? XmlWriter { get; }
        protected override void OnContextUpdated(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context) { }
    }
    public class XmlSerializationContextInfoFactory : Catel.Runtime.Serialization.ISerializationContextInfoFactory
    {
        public XmlSerializationContextInfoFactory() { }
        public Catel.Runtime.Serialization.ISerializationContextInfo GetSerializationContextInfo(Catel.Runtime.Serialization.ISerializer serializer, object model, object data, Catel.Runtime.Serialization.ISerializationConfiguration configuration) { }
    }
    public class XmlSerializer : Catel.Runtime.Serialization.SerializerBase<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo>, Catel.Runtime.Serialization.ISerializer, Catel.Runtime.Serialization.Xml.IXmlSerializer
    {
        public XmlSerializer(Catel.Runtime.Serialization.ISerializationManager serializationManager, Catel.Runtime.Serialization.Xml.IDataContractSerializerFactory dataContractSerializerFactory, Catel.Runtime.Serialization.Xml.IXmlNamespaceManager xmlNamespaceManager, Catel.IoC.ITypeFactory typeFactory, Catel.Runtime.Serialization.IObjectAdapter objectAdapter) { }
        protected override void AfterSerialization(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context) { }
        protected override void AppendContextToStream(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context, System.IO.Stream stream) { }
        protected override void BeforeDeserialization(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context) { }
        protected override void BeforeSerialization(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context) { }
        protected override object? Deserialize(object model, Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context) { }
        protected override Catel.Runtime.Serialization.SerializationObject DeserializeMember(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected override System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> DeserializeMembers(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context) { }
        protected virtual string GetNamespacePrefix() { }
        protected virtual string GetNamespaceUrl() { }
        protected override Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> GetSerializationContextInfo(object model, System.Type modelType, System.IO.Stream stream, Catel.Runtime.Serialization.SerializationContextMode contextMode, Catel.Runtime.Serialization.ISerializationConfiguration? configuration) { }
        protected string GetXmlElementName(System.Type modelType, object model, string? memberName) { }
        protected virtual object? ReadXmlObject(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context, System.Xml.XmlReader xmlReader, System.Runtime.Serialization.DataContractSerializer serializer, string namespacePrefix, string xmlName, System.Type modelType) { }
        protected override void Serialize(object model, Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context) { }
        protected override void SerializeMember(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected override void SerializeMembers(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Xml.XmlSerializationContextInfo> context, System.Collections.Generic.List<Catel.Runtime.Serialization.MemberValue> membersToSerialize) { }
        protected override bool ShouldIgnoreMember(object model, string propertyName) { }
        protected override void Warmup(System.Type type) { }
    }
    public enum XmlSerializerOptimalizationMode
    {
        PrettyXml = 0,
        PrettyXmlAgressive = 1,
        Performance = 2,
    }
}
namespace Catel.Scoping
{
    public class ScopeClosedEventArgs : System.EventArgs
    {
        public ScopeClosedEventArgs(object? scopeObject, string scopeName) { }
        public string ScopeName { get; }
        public object? ScopeObject { get; }
    }
    public class ScopeManager<T> : System.IDisposable
        where T :  class
    {
        protected ScopeManager(string scopeName, System.Func<T>? createScopeFunction) { }
        public int RefCount { get; }
        public T ScopeObject { get; }
        public event System.EventHandler<Catel.Scoping.ScopeClosedEventArgs>? ScopeClosed;
        public virtual void Dispose() { }
        public static Catel.Scoping.ScopeManager<T> GetScopeManager(string scopeName = "", System.Func<T>? createScopeFunction = null) { }
        public static bool ScopeExists(string scopeName = "") { }
    }
}
namespace Catel.Services
{
    public class AppDataService : Catel.Services.IAppDataService
    {
        public AppDataService() { }
        public virtual string GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget applicationDataTarget) { }
    }
    public class GuidObjectIdGenerator<TObjectType> : Catel.Services.ObjectIdGenerator<TObjectType, System.Guid>
        where TObjectType :  class
    {
        public GuidObjectIdGenerator() { }
        protected override System.Guid GenerateUniqueIdentifier() { }
    }
    public interface IAppDataService
    {
        string GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget applicationDataTarget);
    }
    public interface IDispatcherService
    {
        void BeginInvoke(System.Action action, bool onlyBeginInvokeWhenNoAccess = true);
        void Invoke(System.Action action, bool onlyInvokeWhenNoAccess = true);
        System.Threading.Tasks.Task InvokeAsync(System.Action action);
        System.Threading.Tasks.Task InvokeAsync(System.Delegate method, params object[] args);
        System.Threading.Tasks.Task<T> InvokeAsync<T>(System.Func<T> func);
        System.Threading.Tasks.Task InvokeTaskAsync(System.Func<System.Threading.Tasks.Task> actionAsync);
        System.Threading.Tasks.Task InvokeTaskAsync(System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task> actionAsync, System.Threading.CancellationToken cancellationToken);
        System.Threading.Tasks.Task<T> InvokeTaskAsync<T>(System.Func<System.Threading.Tasks.Task<T>> funcAsync);
        System.Threading.Tasks.Task<T> InvokeTaskAsync<T>(System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<T>> funcAsync, System.Threading.CancellationToken cancellationToken);
    }
    public static class IDispatcherServiceExtensions
    {
        public static void BeginInvoke(this Catel.Services.IDispatcherService dispatcherService, System.Action action) { }
        public static void BeginInvoke(this Catel.Services.IDispatcherService dispatcherService, System.Delegate method, params object[] args) { }
        public static void BeginInvokeIfRequired(this Catel.Services.IDispatcherService dispatcherService, System.Action action) { }
        public static void BeginInvokeIfRequired(this Catel.Services.IDispatcherService dispatcherService, System.Delegate method, params object[] args) { }
        public static void Invoke(this Catel.Services.IDispatcherService dispatcherService, System.Delegate method, params object[] args) { }
        public static void InvokeIfRequired(this Catel.Services.IDispatcherService dispatcherService, System.Action action) { }
        public static void InvokeIfRequired(this Catel.Services.IDispatcherService dispatcherService, System.Delegate method, params object[] args) { }
    }
    public interface ILanguageService
    {
        bool CacheResults { get; set; }
        System.Globalization.CultureInfo FallbackCulture { get; set; }
        System.Globalization.CultureInfo PreferredCulture { get; set; }
        event System.EventHandler<System.EventArgs>? LanguageUpdated;
        void ClearLanguageResources();
        string? GetString(string resourceName);
        string? GetString(string resourceName, System.Globalization.CultureInfo cultureInfo);
        string? GetString(Catel.Services.ILanguageSource languageSource, string resourceName, System.Globalization.CultureInfo cultureInfo);
        void PreloadLanguageSources();
        void RegisterLanguageSource(Catel.Services.ILanguageSource languageSource);
    }
    public static class ILanguageServiceExtensions
    {
        public static string GetRequiredString(this Catel.Services.ILanguageService languageService, string resourceName) { }
        public static string GetRequiredString(this Catel.Services.ILanguageService languageService, string resourceName, System.Globalization.CultureInfo cultureInfo) { }
        public static string GetRequiredString(this Catel.Services.ILanguageService languageService, Catel.Services.ILanguageSource languageSource, string resourceName, System.Globalization.CultureInfo cultureInfo) { }
    }
    public interface ILanguageSource
    {
        string GetSource();
    }
    public interface IObjectConverterService
    {
        System.Globalization.CultureInfo DefaultCulture { get; set; }
        object? ConvertFromObjectToObject(object? value, System.Type targetType);
        string ConvertFromObjectToString(object? value);
        string ConvertFromObjectToString(object? value, System.Globalization.CultureInfo culture);
        object? ConvertFromStringToObject(string value, System.Type targetType);
        object? ConvertFromStringToObject(string value, System.Type targetType, System.Globalization.CultureInfo culture);
    }
    public static class IObjectConverterServiceExtensions
    {
        public static T? ConvertFromObjectToObject<T>(this Catel.Services.IObjectConverterService service, object value) { }
        public static T? ConvertFromStringToObject<T>(this Catel.Services.IObjectConverterService service, string value) { }
    }
    public interface IObjectIdGenerator<TUniqueIdentifier>
    {
        TUniqueIdentifier GetUniqueIdentifier(bool reuse = false);
        void ReleaseIdentifier(TUniqueIdentifier identifier);
    }
    public interface IObjectIdGenerator<in TObjectType, TUniqueIdentifier> : Catel.Services.IObjectIdGenerator<TUniqueIdentifier>
        where in TObjectType :  class
    {
        TUniqueIdentifier GetUniqueIdentifierForInstance(TObjectType instance, bool reuse = false);
    }
    public interface IRollingInMemoryLogService
    {
        Catel.Logging.RollingInMemoryLogListener LogListener { get; }
        int MaximumNumberOfErrorLogEntries { get; set; }
        int MaximumNumberOfLogEntries { get; set; }
        int MaximumNumberOfWarningLogEntries { get; set; }
        event System.EventHandler<Catel.Logging.LogMessageEventArgs>? LogMessage;
        System.Collections.Generic.IEnumerable<Catel.Logging.LogEntry> GetErrorLogEntries();
        System.Collections.Generic.IEnumerable<Catel.Logging.LogEntry> GetLogEntries();
        System.Collections.Generic.IEnumerable<Catel.Logging.LogEntry> GetWarningLogEntries();
    }
    public interface IService
    {
        string Name { get; }
    }
    public sealed class IntegerObjectIdGenerator<TObjectType> : Catel.Services.NumericBasedObjectIdGenerator<TObjectType, int>
        where TObjectType :  class
    {
        public IntegerObjectIdGenerator() { }
        protected override int GenerateUniqueIdentifier() { }
    }
    public class LanguageResourceKey : System.IEquatable<Catel.Services.LanguageResourceKey>
    {
        public LanguageResourceKey(string resourceName, System.Globalization.CultureInfo cultureInfo) { }
        public System.Globalization.CultureInfo CultureInfo { get; }
        public string ResourceName { get; }
        public bool Equals(Catel.Services.LanguageResourceKey? other) { }
        public override bool Equals(object? obj) { }
        public override int GetHashCode() { }
        public override string ToString() { }
    }
    [System.Diagnostics.DebuggerDisplay("{GetSource()}")]
    public class LanguageResourceSource : Catel.Services.ILanguageSource
    {
        public LanguageResourceSource(string assemblyName, string namespaceName, string resourceFileName) { }
        public string AssemblyName { get; }
        public string NamespaceName { get; }
        public string ResourceFileName { get; }
        public string GetSource() { }
    }
    public class LanguageService : Catel.Services.LanguageServiceBase, Catel.Services.ILanguageService
    {
        public LanguageService() { }
        public bool CacheResults { get; set; }
        public System.Globalization.CultureInfo FallbackCulture { get; set; }
        public System.Globalization.CultureInfo PreferredCulture { get; set; }
        public event System.EventHandler<System.EventArgs>? LanguageUpdated;
        public void ClearLanguageResources() { }
        public string? GetString(string resourceName) { }
        public string? GetString(string resourceName, System.Globalization.CultureInfo cultureInfo) { }
        public override string? GetString(Catel.Services.ILanguageSource languageSource, string resourceName, System.Globalization.CultureInfo cultureInfo) { }
        protected override void PreloadLanguageSource(Catel.Services.ILanguageSource languageSource) { }
        public virtual void PreloadLanguageSources() { }
        public void RegisterLanguageSource(Catel.Services.ILanguageSource languageSource) { }
    }
    public abstract class LanguageServiceBase
    {
        protected LanguageServiceBase() { }
        public abstract string? GetString(Catel.Services.ILanguageSource languageSource, string resourceName, System.Globalization.CultureInfo cultureInfo);
        protected abstract void PreloadLanguageSource(Catel.Services.ILanguageSource languageSource);
    }
    public sealed class LongObjectIdGenerator<TObjectType> : Catel.Services.NumericBasedObjectIdGenerator<TObjectType, long>
        where TObjectType :  class
    {
        public LongObjectIdGenerator() { }
        protected override long GenerateUniqueIdentifier() { }
    }
    public abstract class NumericBasedObjectIdGenerator<TObjectType, TUniqueIdentifier> : Catel.Services.ObjectIdGenerator<TObjectType, TUniqueIdentifier>
        where TObjectType :  class
        where TUniqueIdentifier :  notnull
    {
        protected NumericBasedObjectIdGenerator() { }
        protected static TUniqueIdentifier Value { get; set; }
    }
    public class ObjectConverterService : Catel.Services.IObjectConverterService
    {
        public ObjectConverterService() { }
        public System.Globalization.CultureInfo DefaultCulture { get; set; }
        public virtual object? ConvertFromObjectToObject(object? value, System.Type targetType) { }
        public virtual string ConvertFromObjectToString(object? value) { }
        public string ConvertFromObjectToString(object? value, System.Globalization.CultureInfo culture) { }
        public virtual object? ConvertFromStringToObject(string value, System.Type targetType) { }
        public object? ConvertFromStringToObject(string value, System.Type targetType, System.Globalization.CultureInfo culture) { }
    }
    public abstract class ObjectIdGenerator<TObjectType, TUniqueIdentifier> : Catel.Services.IObjectIdGenerator<TUniqueIdentifier>, Catel.Services.IObjectIdGenerator<TObjectType, TUniqueIdentifier>
        where TObjectType :  class
    {
        protected readonly object _lock;
        protected ObjectIdGenerator() { }
        protected abstract TUniqueIdentifier GenerateUniqueIdentifier();
        public TUniqueIdentifier GetUniqueIdentifier(bool reuse = false) { }
        public TUniqueIdentifier GetUniqueIdentifierForInstance(TObjectType instance, bool reuse = false) { }
        public void ReleaseIdentifier(TUniqueIdentifier identifier) { }
    }
    public class RollingInMemoryLogService : Catel.Services.ServiceBase, Catel.Services.IRollingInMemoryLogService
    {
        public RollingInMemoryLogService() { }
        public RollingInMemoryLogService(Catel.Logging.RollingInMemoryLogListener? logListener) { }
        public Catel.Logging.RollingInMemoryLogListener LogListener { get; }
        public int MaximumNumberOfErrorLogEntries { get; set; }
        public int MaximumNumberOfLogEntries { get; set; }
        public int MaximumNumberOfWarningLogEntries { get; set; }
        public event System.EventHandler<Catel.Logging.LogMessageEventArgs>? LogMessage;
        public System.Collections.Generic.IEnumerable<Catel.Logging.LogEntry> GetErrorLogEntries() { }
        public System.Collections.Generic.IEnumerable<Catel.Logging.LogEntry> GetLogEntries() { }
        public System.Collections.Generic.IEnumerable<Catel.Logging.LogEntry> GetWarningLogEntries() { }
    }
    public abstract class ServiceBase : Catel.Services.IService
    {
        protected ServiceBase() { }
        public virtual string Name { get; }
    }
    public class ShimDispatcherService : Catel.Services.IDispatcherService
    {
        public ShimDispatcherService() { }
        public void BeginInvoke(System.Action action, bool onlyBeginInvokeWhenNoAccess = true) { }
        public void Invoke(System.Action action, bool onlyInvokeWhenNoAccess = true) { }
        public System.Threading.Tasks.Task InvokeAsync(System.Action action) { }
        public System.Threading.Tasks.Task InvokeAsync(System.Delegate method, params object[] args) { }
        public System.Threading.Tasks.Task<T> InvokeAsync<T>(System.Func<T> func) { }
        public System.Threading.Tasks.Task InvokeTaskAsync(System.Func<System.Threading.Tasks.Task> actionAsync) { }
        public System.Threading.Tasks.Task InvokeTaskAsync(System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task> actionAsync, System.Threading.CancellationToken cancellationToken) { }
        public System.Threading.Tasks.Task<T> InvokeTaskAsync<T>(System.Func<System.Threading.Tasks.Task<T>> funcAsync) { }
        public System.Threading.Tasks.Task<T> InvokeTaskAsync<T>(System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<T>> funcAsync, System.Threading.CancellationToken cancellationToken) { }
    }
    public sealed class ULongObjectIdGenerator<TObjectType> : Catel.Services.NumericBasedObjectIdGenerator<TObjectType, ulong>
        where TObjectType :  class
    {
        public ULongObjectIdGenerator() { }
        protected override ulong GenerateUniqueIdentifier() { }
    }
}
namespace Catel.Text
{
    public static class StringBuilderExtensions
    {
        public static void AppendLine(this System.Text.StringBuilder sb, string format, object arg0) { }
        public static void AppendLine(this System.Text.StringBuilder sb, string format, params object[] args) { }
        public static void AppendLine(this System.Text.StringBuilder sb, string format, object arg0, object arg1) { }
        public static void AppendLine(this System.Text.StringBuilder sb, string format, object arg0, object arg1, object arg2) { }
    }
}
namespace Catel.Threading
{
    [System.Diagnostics.DebuggerDisplay("Id = {Id}, Taken = {_taken}")]
    [System.Diagnostics.DebuggerTypeProxy(typeof(Catel.Threading.AsyncLock.DebugView))]
    public sealed class AsyncLock
    {
        public AsyncLock() { }
        public AsyncLock(Catel.Threading.IAsyncWaitQueue<System.IDisposable> queue) { }
        public int Id { get; }
        public bool IsTaken { get; }
        public bool IsTakenByCurrentTask { get; }
        public System.IDisposable Lock() { }
        public System.IDisposable Lock(System.Threading.CancellationToken cancellationToken) { }
        public Catel.Threading.AwaitableDisposable<System.IDisposable> LockAsync() { }
        public Catel.Threading.AwaitableDisposable<System.IDisposable> LockAsync(System.Threading.CancellationToken cancellationToken) { }
    }
    public static class AsyncWaitQueueExtensions
    {
        public static System.Threading.Tasks.Task<T> EnqueueAsync<T>(this Catel.Threading.IAsyncWaitQueue<T> @this, object syncObject, System.Action continueWith, System.Threading.CancellationToken token) { }
    }
    public struct AwaitableDisposable<T>
        where T : System.IDisposable
    {
        public AwaitableDisposable(System.Threading.Tasks.Task<T> task) { }
        public System.Threading.Tasks.Task<T> AsTask() { }
        public System.Runtime.CompilerServices.ConfiguredTaskAwaitable<T> ConfigureAwait(bool continueOnCapturedContext) { }
        public System.Runtime.CompilerServices.TaskAwaiter<T> GetAwaiter() { }
        public static System.Threading.Tasks.Task<T> op_Implicit(Catel.Threading.AwaitableDisposable<T> source) { }
    }
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [System.Diagnostics.DebuggerTypeProxy(typeof(Catel.Threading.DefaultAsyncWaitQueue<T>.DebugView))]
    public sealed class DefaultAsyncWaitQueue<T> : Catel.Threading.IAsyncWaitQueue<T>
    {
        public DefaultAsyncWaitQueue() { }
    }
    public interface IAsyncWaitQueue<T>
    {
        bool IsEmpty { get; }
        System.IDisposable CancelAll();
        System.IDisposable Dequeue(T result = default);
        System.IDisposable DequeueAll(T result = default);
        System.Threading.Tasks.Task<T> EnqueueAsync();
        System.IDisposable TryCancel(System.Threading.Tasks.Task task);
    }
    public static class ReaderWriterLockSlimExtensions
    {
        public static void PerformRead(this System.Threading.ReaderWriterLockSlim lockSlim, System.Action criticalOperation) { }
        public static T PerformRead<T>(this System.Threading.ReaderWriterLockSlim lockSlim, System.Func<T> criticalOperation) { }
        public static void PerformUpgradableRead(this System.Threading.ReaderWriterLockSlim lockSlim, System.Action criticalOperation) { }
        public static T PerformUpgradableRead<T>(this System.Threading.ReaderWriterLockSlim lockSlim, System.Func<T> criticalOperation) { }
        public static void PerformWrite(this System.Threading.ReaderWriterLockSlim lockSlim, System.Action criticalOperation) { }
    }
    public class SynchronizationContext
    {
        public SynchronizationContext() { }
        public bool IsLockAcquired { get; }
        public void Acquire() { }
        public void Execute(System.Action code) { }
        public T Execute<T>(System.Func<T> code) { }
        public void Release() { }
    }
    public static class SynchronizationContextExtensions
    {
        public static Catel.IDisposableToken<Catel.Threading.SynchronizationContext> AcquireScope(this Catel.Threading.SynchronizationContext synchronizationContext) { }
    }
    public static class TaskExtensions
    {
        public static System.Threading.Tasks.Task AwaitWithTimeoutAsync(this System.Threading.Tasks.Task task, int timeout) { }
        public static void WaitAndUnwrapException(this System.Threading.Tasks.Task task) { }
        public static void WaitAndUnwrapException(this System.Threading.Tasks.Task task, System.Threading.CancellationToken cancellationToken) { }
        public static TResult WaitAndUnwrapException<TResult>(this System.Threading.Tasks.Task<TResult> task) { }
        public static TResult WaitAndUnwrapException<TResult>(this System.Threading.Tasks.Task<TResult> task, System.Threading.CancellationToken cancellationToken) { }
    }
}
namespace System.ComponentModel
{
    public class BeginEditEventArgs : System.ComponentModel.EditEventArgs { }
    public class CancelEditCompletedEventArgs : System.EventArgs { }
    public class CancelEditEventArgs : System.ComponentModel.EditEventArgs { }
    public class EditEventArgs : System.EventArgs { }
    public class EndEditEventArgs : System.ComponentModel.EditEventArgs { }
    public interface IAdvancedEditableObject : System.ComponentModel.IEditableObject { }
    public interface IDataWarningInfo { }
    public interface INotifyDataWarningInfo { }
    public static class PropertyChangedEventArgsExtensions { }
}