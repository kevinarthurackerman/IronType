namespace IronType.Json.DependencyInjection;

internal class IronTypeJsonOptionsConfigurator : IConfigureOptions<JsonOptions>
{
    private readonly Action<JsonSerializerOptions> _configureOptions;

    public IronTypeJsonOptionsConfigurator(Action<JsonSerializerOptions> configureOptions)
    {
        _configureOptions = configureOptions;
    }

    public void Configure(JsonOptions options)
    {
        _configureOptions(options.JsonSerializerOptions);
    }
}
