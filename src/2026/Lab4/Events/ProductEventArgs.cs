namespace Lab4.Events;

public sealed class ProductEventArgs(long newId, decimal newPrice) : EventArgs
{
    public long NewId { get; set; } = newId;
    public decimal NewPrice { get; set; } = newPrice;
}   
