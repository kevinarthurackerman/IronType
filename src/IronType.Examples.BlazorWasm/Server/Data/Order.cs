namespace IronType.Examples.BlazorWasm.Server.Data;

public class Order
{
    public OrderId Id { get; set; }
    public LocalDate OrderedOn { get; set; }
}