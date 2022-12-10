namespace IronType.EntityFramework.Relational;

internal class PassthroughMethodCallTranslator : IMethodCallTranslator
{
    private readonly Func<MethodMappingInfo, ConvertToFrameworkMethodContext, MethodMappingInfo?> _convertToFrameworkMethod;
    private readonly Lazy<ISqlExpressionFactory> _sqlExpressionFactory;
    private readonly Lazy<IMethodCallTranslatorProvider> _methodCallTranslatorProvider;
    private readonly Lazy<IModel> _model;

    public PassthroughMethodCallTranslator(
        Func<MethodMappingInfo, ConvertToFrameworkMethodContext, MethodMappingInfo?> convertToFrameworkMethod,
        Func<ISqlExpressionFactory> initializeSqlExpressionFactory,
        Func<IMethodCallTranslatorProvider> initializeMethodCallTranslatorProvider,
        Func<IModel> initializeModel)
    {
        _convertToFrameworkMethod = convertToFrameworkMethod;
        _sqlExpressionFactory = new(initializeSqlExpressionFactory);
        _methodCallTranslatorProvider = new(initializeMethodCallTranslatorProvider);
        _model = new(initializeModel);
    }

    public SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        var context = new ConvertToFrameworkMethodContext(Convert, ConvertToFramework, Constant);
        var frameworkMethodMappingInfo = _convertToFrameworkMethod(new MethodMappingInfo(instance, method, arguments), context);

        if (frameworkMethodMappingInfo == null) return null;

        var translateExpression = _methodCallTranslatorProvider.Value.Translate(
            _model.Value,
            (SqlExpression?)frameworkMethodMappingInfo.Instance,
            frameworkMethodMappingInfo.Method,
            frameworkMethodMappingInfo.Arguments.Cast<SqlExpression>().ToArray(),
            logger);

        if (translateExpression == null) return null;

        return (SqlExpression)Convert(translateExpression, method.ReturnType);
    }

    private Expression Convert(Expression value, Type type)
        => _sqlExpressionFactory.Value.Convert((SqlExpression)value, type);

    private Expression ConvertToFramework(Expression value)
        => _sqlExpressionFactory.Value.ApplyTypeMapping((SqlExpression)value, null);

    private Expression Constant(object? value, Type type)
        => _sqlExpressionFactory.Value.Constant(value, type);
}
