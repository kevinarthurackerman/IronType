namespace IronType.Examples.BlazorWasm.Shared;

public readonly record struct OrderId(Guid Value);

public class OrderIdTypeMapping : SimpleTypeMapping<OrderId, Guid> { }