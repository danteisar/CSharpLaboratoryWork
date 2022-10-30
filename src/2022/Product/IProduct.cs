namespace Product;

public interface IProduct
{
    int Id { get; set; }
    string Type { get; }
    string Name { get; set; }
    string Information { get; }
    Barcode.Barcode Barcode { get; set; }
}