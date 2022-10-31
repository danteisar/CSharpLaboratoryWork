using Barcode.Lab3;

namespace Product.Lab4;

public sealed class Comic4 : Book4, IThing4
{
    public Comic4(int id, string name, string author, int year, decimal price) : base(id, name, author, year, price)
    {
       
    }

    protected override string Type => "Комикс";

    // Иммунитет по измению штрих кода

    IBarcode3 IThing4.Barcode
    {
        get => base.Barcode;
        set
        {
            // ignore
        }
    }
}