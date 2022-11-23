namespace IronType.Examples.BlazorWasm.Server.Data;

public class Order
{
    public OrderId Id { get; set; }
}

public readonly record struct OrderId(Guid Value);

public class OrderIdTypeData : SimpleTypeData<OrderId, Guid> { }