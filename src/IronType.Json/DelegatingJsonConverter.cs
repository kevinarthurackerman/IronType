namespace IronType.Json;

internal class DelegatingJsonConverter<TApp, TFramework> : JsonConverter<TApp>
{
    private readonly Func<TApp, TFramework> _convertToFrameworkValue;
    private readonly Func<TFramework, TApp> _convertToAppValue;

    public DelegatingJsonConverter(Func<TApp, TFramework> convertToFrameworkValue, Func<TFramework, TApp> convertToAppValue)
    {
        _convertToFrameworkValue = convertToFrameworkValue;
        _convertToAppValue = convertToAppValue;
    }

    public override TApp? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = (JsonConverter<TFramework>)options.GetConverter(typeof(TFramework));

        var frameworkValue = converter.Read(ref reader, typeToConvert, options);
        
        if (frameworkValue == null) return default;

        return _convertToAppValue(frameworkValue);
    }

    public override void Write(Utf8JsonWriter writer, TApp value, JsonSerializerOptions options)
    {
        var converter = (JsonConverter<TFramework>)options.GetConverter(typeof(TFramework));

        var frameworkValue = _convertToFrameworkValue(value);

        converter.Write(writer, frameworkValue, options);
    }
}
