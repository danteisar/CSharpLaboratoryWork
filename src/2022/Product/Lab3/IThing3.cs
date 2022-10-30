using Barcode.Lab3;

namespace Product.Lab3;

public interface IThing3
{
    int Id { get; set; }
    string Name { get; set; }   
    IBarcode3 Barcode3 { get; set; }
}