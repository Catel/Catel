[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v6.0", FrameworkDisplayName="")]
namespace Catel.Core
{
    public static class ModuleInitializer
    {
        public static void Initialize() { }
    }
}
namespace Catel
{
    public static class JsonExtensions
    {
        public static Newtonsoft.Json.JsonReader CreateReader(this Newtonsoft.Json.Linq.JToken token, Catel.Runtime.Serialization.ISerializationConfiguration? configuration) { }
        public static string ToJson(this Catel.Data.ModelBase model, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
    }
    public class SerializationJsonModule : Catel.IoC.IServiceLocatorInitializer
    {
        public SerializationJsonModule() { }
        public void Initialize(Catel.IoC.IServiceLocator serviceLocator) { }
    }
}
namespace Catel.Runtime.Serialization.Json
{
    public class CatelJsonContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        public CatelJsonContractResolver() { }
        protected override Newtonsoft.Json.JsonConverter? ResolveContractConverter(System.Type objectType) { }
    }
    public class CatelJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public CatelJsonConverter(Catel.Runtime.Serialization.Json.IJsonSerializer jsonSerializer, Catel.Runtime.Serialization.ISerializationConfiguration? configuration) { }
        public override bool CanConvert(System.Type objectType) { }
        public override object? ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object? existingValue, Newtonsoft.Json.JsonSerializer serializer) { }
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer) { }
    }
    public interface ICustomJsonSerializable
    {
        void Deserialize(Newtonsoft.Json.JsonReader jsonReader);
        void Serialize(Newtonsoft.Json.JsonWriter jsonWriter);
    }
    public interface IJsonSerializer : Catel.Runtime.Serialization.ISerializer
    {
        bool PreserveReferences { get; set; }
        bool WriteTypeInfo { get; set; }
        object? Deserialize(System.Type modelType, Newtonsoft.Json.JsonReader jsonReader, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
        void Serialize(object model, Newtonsoft.Json.JsonWriter jsonWriter, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null);
    }
    public class JsonSerializationConfiguration : Catel.Runtime.Serialization.SerializationConfiguration
    {
        public JsonSerializationConfiguration() { }
        public Newtonsoft.Json.DateParseHandling DateParseHandling { get; set; }
        public System.DateTimeKind DateTimeKind { get; set; }
        public Newtonsoft.Json.DateTimeZoneHandling DateTimeZoneHandling { get; set; }
        public Newtonsoft.Json.Formatting Formatting { get; set; }
        public bool UseBson { get; set; }
    }
    public class JsonSerializationContextInfo : Catel.Runtime.Serialization.SerializationContextInfoBase<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo>
    {
        public JsonSerializationContextInfo(Newtonsoft.Json.JsonSerializer jsonSerializer, Newtonsoft.Json.JsonReader? jsonReader, Newtonsoft.Json.JsonWriter? jsonWriter) { }
        public Newtonsoft.Json.Linq.JArray? JsonArray { get; set; }
        public System.Collections.Generic.Dictionary<string, Newtonsoft.Json.Linq.JProperty>? JsonProperties { get; set; }
        public Newtonsoft.Json.JsonReader? JsonReader { get; }
        public Newtonsoft.Json.JsonSerializer JsonSerializer { get; }
        public Newtonsoft.Json.JsonWriter? JsonWriter { get; }
    }
    public class JsonSerializationContextInfoFactory : Catel.Runtime.Serialization.ISerializationContextInfoFactory
    {
        public JsonSerializationContextInfoFactory() { }
        public Catel.Runtime.Serialization.ISerializationContextInfo GetSerializationContextInfo(Catel.Runtime.Serialization.ISerializer serializer, object model, object data, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
    }
    public class JsonSerializer : Catel.Runtime.Serialization.SerializerBase<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo>, Catel.Runtime.Serialization.ISerializer, Catel.Runtime.Serialization.Json.IJsonSerializer
    {
        public const string GraphId = "$graphid";
        public const string GraphRefId = "$graphrefid";
        public const string TypeName = "$typename";
        public JsonSerializer(Catel.Runtime.Serialization.ISerializationManager serializationManager, Catel.IoC.ITypeFactory typeFactory, Catel.Runtime.Serialization.IObjectAdapter objectAdapter) { }
        public bool PreserveReferences { get; set; }
        public bool WriteTypeInfo { get; set; }
        protected override void AfterSerialization(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo> context) { }
        protected override void AppendContextToStream(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo> context, System.IO.Stream stream) { }
        protected override void BeforeDeserialization(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo> context) { }
        protected override void BeforeSerialization(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo> context) { }
        protected override object? Deserialize(object model, Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo> context) { }
        public object? Deserialize(System.Type modelType, Newtonsoft.Json.JsonReader jsonReader, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        protected override Catel.Runtime.Serialization.SerializationObject DeserializeMember(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected override Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo> GetSerializationContextInfo(object model, System.Type modelType, System.IO.Stream stream, Catel.Runtime.Serialization.SerializationContextMode contextMode, Catel.Runtime.Serialization.ISerializationConfiguration? configuration) { }
        protected virtual Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo> GetSerializationContextInfo(object model, System.Type modelType, Newtonsoft.Json.JsonReader? jsonReader, Newtonsoft.Json.JsonWriter? jsonWriter, Catel.Runtime.Serialization.SerializationContextMode contextMode, System.Collections.Generic.Dictionary<string, Newtonsoft.Json.Linq.JProperty>? jsonProperties, Newtonsoft.Json.Linq.JArray? jsonArray, Catel.Runtime.Serialization.ISerializationConfiguration? configuration) { }
        protected override void Serialize(object model, Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo> context) { }
        public void Serialize(object model, Newtonsoft.Json.JsonWriter jsonWriter, Catel.Runtime.Serialization.ISerializationConfiguration? configuration = null) { }
        protected override void SerializeMember(Catel.Runtime.Serialization.ISerializationContext<Catel.Runtime.Serialization.Json.JsonSerializationContextInfo> context, Catel.Runtime.Serialization.MemberValue memberValue) { }
        protected override void Warmup(System.Type type) { }
    }
}