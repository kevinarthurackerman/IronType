namespace IronType.EntityFramework.Relational;

internal sealed class LazyInitializedRelationalTypeMapping : RelationalTypeMapping
{
    private readonly Type _clrType;
    private readonly Lazy<RelationalTypeMapping> _innerRelationalTypeMapper;

    public override Type ClrType => _clrType;

    public override ValueComparer Comparer => _innerRelationalTypeMapper.Value.Comparer;

    public override ValueComparer KeyComparer => _innerRelationalTypeMapper.Value.KeyComparer;

    public override ValueConverter? Converter => _innerRelationalTypeMapper.Value.Converter;

    public override Func<IProperty, IEntityType, ValueGenerator>? ValueGeneratorFactory =>
        _innerRelationalTypeMapper.Value.ValueGeneratorFactory;

    public override StoreTypePostfix StoreTypePostfix =>
        _innerRelationalTypeMapper.Value.StoreTypePostfix;

    public override string StoreType => _innerRelationalTypeMapper.Value.StoreType;

    public override string StoreTypeNameBase => _innerRelationalTypeMapper.Value.StoreTypeNameBase;

    public override DbType? DbType => _innerRelationalTypeMapper.Value.DbType;

    public override bool IsUnicode => _innerRelationalTypeMapper.Value.IsUnicode;

    public override int? Size => _innerRelationalTypeMapper.Value.Size;

    public override int? Precision => _innerRelationalTypeMapper.Value.Precision;

    public override int? Scale => _innerRelationalTypeMapper.Value.Scale;

    public override bool IsFixedLength => _innerRelationalTypeMapper.Value.IsFixedLength;

    public LazyInitializedRelationalTypeMapping(Type clrType, Func<RelationalTypeMapping> initializeRelationalTypeMapping)
        : base(string.Empty, typeof(object))
    {
        _clrType = clrType;
        _innerRelationalTypeMapper = new(initializeRelationalTypeMapping);
    }

    public override string GenerateSqlLiteral(object? value) =>
        _innerRelationalTypeMapper.Value.GenerateSqlLiteral(value);

    public override string GenerateProviderValueSqlLiteral(object? value) =>
        _innerRelationalTypeMapper.Value.GenerateProviderValueSqlLiteral(value);

    public override MethodInfo GetDataReaderMethod() =>
        _innerRelationalTypeMapper.Value.GetDataReaderMethod();

    public override Expression GenerateCodeLiteral(object value) =>
        _innerRelationalTypeMapper.Value.GenerateCodeLiteral(value);

    public override RelationalTypeMapping Clone(string storeType, int? size) =>
        _innerRelationalTypeMapper.Value.Clone(storeType, size);

    public override RelationalTypeMapping Clone(int? precision, int? scale) =>
        _innerRelationalTypeMapper.Value.Clone(precision, scale);

    public override RelationalTypeMapping Clone(in RelationalTypeMappingInfo mappingInfo) =>
        _innerRelationalTypeMapper.Value.Clone(in mappingInfo);

    public override CoreTypeMapping Clone(ValueConverter? converter) =>
        _innerRelationalTypeMapper.Value.Clone(converter);

    public override Expression CustomizeDataReaderExpression(Expression expression) =>
        _innerRelationalTypeMapper.Value.CustomizeDataReaderExpression(expression);

    public override bool Equals(object? obj) => _innerRelationalTypeMapper.Value.Equals(obj);

    public override int GetHashCode() => _innerRelationalTypeMapper.Value.GetHashCode();

    public override string? ToString() => _innerRelationalTypeMapper.Value.ToString();

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) =>
        throw new NotImplementedException();
}