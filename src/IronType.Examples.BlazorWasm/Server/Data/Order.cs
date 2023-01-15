namespace IronType.Examples.BlazorWasm.Server.Data;

public class Order
{
    public OrderId Id { get; set; }
    public LocalDate OrderedOn { get; set; }
    public string CustomerName { get; set; } = null!;
    public Location Location { get; set; }
    public Length Length { get; set; }
    public Length Width { get; set; }
    public Length Height { get; set; }
    public Mass Weight { get; set; }
}