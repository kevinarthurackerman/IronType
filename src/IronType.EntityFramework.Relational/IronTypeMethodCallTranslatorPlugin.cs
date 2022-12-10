namespace IronType.EntityFramework.Relational;

public class IronTypeMethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
{
    public IEnumerable<IMethodCallTranslator> Translators { get; }

    public IronTypeMethodCallTranslatorPlugin(
        IServiceProvider serviceProvider,
        IronTypeConfiguration ironTypeConfiguration)
    {
        Translators = ironTypeConfiguration.MethodMappings
            .Select(x => new PassthroughMethodCallTranslator(
                x.ConvertToFrameworkMethod,
                () => serviceProvider.GetRequiredService<ISqlExpressionFactory>(),
                () => serviceProvider.GetRequiredService<IMethodCallTranslatorProvider>(),
                () => serviceProvider.GetRequiredService<IModel>()))
            .ToArray();
    }
}
