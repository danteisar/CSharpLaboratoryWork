using Barcode.Lab3;

namespace Product.Lab4;

public sealed class Comic4 : Book4, IThing4
{
    public Comic4(int id, string name, string author, int year, decimal price) : base(id, name, author, year, price)
    {
        _barcode3 = id.ToString();
    }

    protected override string Type => "Комикс";

    private Barcode3 _barcode3;

    // Иммунитет по измению штрих кода
    IBarcode3 IThing4.Barcode3
    {
        get => _barcode3;
        set => _barcode3 = Id.ToString();
    }
}