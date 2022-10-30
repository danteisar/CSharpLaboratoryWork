using Barcode.Lab3;

namespace Product.Lab3;

public sealed class Comic3 : Book3, IThing3
{
    public Comic3(int id, string name, string author, int year, decimal price) : base(id, name, author, year, price)
    {
        _barcode3 = id.ToString();
    }

    protected override string Type => "Комикс";

    private Barcode3 _barcode3;

    // Иммунитет по измению штрих кода
    IBarcode3 IThing3.Barcode3
    {
        get => _barcode3;
        set => _barcode3 = Id.ToString();
    }
}