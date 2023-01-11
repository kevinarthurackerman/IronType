namespace IronType.NewtonsoftJson;

internal class DelegatingJsonConverter<TApp, TFramework> : JsonConverter<TApp>
{
    private readonly Func<TApp, TFramework> _convertToFrameworkValue;
    private readonly Func<TFramework, TApp> _convertToAppValue;

    public DelegatingJsonConverter(Func<TApp, TFramework> convertToFrameworkValue, Func<TFramework, TApp> convertToAppValue)
    {
        _convertToFrameworkValue = convertToFrameworkValue;
        _convertToAppValue = convertToAppValue;
    }

    public override void WriteJson(JsonWriter writer, TApp? value, JsonSerializer serializer)
    {
        var frameworkValue = value == null
            ? default
            : _convertToFrameworkValue(value);

        serializer.Serialize(writer, frameworkValue, typeof(TFramework));
    }

    public override TApp? ReadJson(JsonReader reader, Type objectType, TApp? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var frameworkValue = serializer.Deserialize<TFramework>(reader);
        
        if (frameworkValue == null) return default;

        return _convertToAppValue(frameworkValue);
    }
}
