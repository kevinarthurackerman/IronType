namespace IronType.Examples.BlazorWasm.Shared;

public class OrderViewModel
{
    public OrderId Id { get; set; }
    public LocalDate OrderedOn { get; set; }
    public string CustomerName { get; set; } = null!;
    public Length Length { get; set; }
    public Length Width { get; set; }
    public Length Height { get; set; }
    public Mass Weight { get; set; }
}