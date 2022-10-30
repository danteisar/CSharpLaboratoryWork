namespace Product;

public interface IProduct
{
    int Id { get; set; }
    string Name { get; set; }
    Barcode.Barcode Barcode { get; set; }
}