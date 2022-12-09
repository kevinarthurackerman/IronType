namespace IronType.Examples.BlazorWasm.Shared;

public class OrderViewModel
{
    public OrderId Id { get; set; }
    public LocalDate OrderedOn { get; set; }
    public string CustomerName { get; set; } = null!;
}