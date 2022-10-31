using Barcode.Lab3;

namespace Product.Lab4;

public interface IThing4
{
    int Id { get; set; }
    string Name { get; set; }   
    IBarcode3 Barcode { get; set; }

    event EventHandler<Thing4IdEventArgs> ThingIdChanged;
}