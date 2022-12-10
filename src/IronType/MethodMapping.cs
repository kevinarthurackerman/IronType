namespace IronType;

public sealed class MethodMapping : IMethodMapping
{
    private readonly Func<MethodMappingInfo, ConvertToFrameworkMethodContext, MethodMappingInfo?> _convertToFrameworkMethod;

    public MethodMapping(Func<MethodMappingInfo, ConvertToFrameworkMethodContext, MethodMappingInfo?> convertToFrameworkMethod)
    {
        _convertToFrameworkMethod = convertToFrameworkMethod;
    }

    public MethodMappingInfo? ConvertToFrameworkMethod(MethodMappingInfo appMethodMappingInfo, ConvertToFrameworkMethodContext context)
        => _convertToFrameworkMethod(appMethodMappingInfo, context);
}

public interface IMethodMapping
{
    public MethodMappingInfo? ConvertToFrameworkMethod(MethodMappingInfo appMethodMappingInfo, ConvertToFrameworkMethodContext context);
}

public record MethodMappingInfo(Expression? Instance, MethodInfo Method, IReadOnlyList<Expression> Arguments);

public class ConvertToFrameworkMethodContext
{
    private readonly Func<Expression, Type, Expression> _convert;
    private readonly Func<Expression, Expression> _convertToFramework;
    private readonly Func<object?, Type, Expression> _createConstant;

    public ConvertToFrameworkMethodContext(
        Func<Expression, Type, Expression> convertValue,
        Func<Expression, Expression> convertToFramework,
        Func<object?, Type, Expression> createConstant)
    {
        _convert = convertValue;
        _convertToFramework = convertToFramework;
        _createConstant = createConstant;
    }

    public Expression Convert(Expression value, Type toType)
        => _convert(value, toType);

    public Expression ConvertToFramework(Expression value)
        => _convertToFramework(value);

    public Expression Create(object? value, Type type)
        => _createConstant(value, type);
}