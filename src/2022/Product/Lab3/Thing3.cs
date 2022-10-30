using Barcode.Lab3;

namespace Product.Lab3;

public abstract class Thing3 : IThing3
{
    protected Thing3(int id, string name)
    {
        Name = name;
        Id = id;
    }

    private int _id;

    public int Id
    {
        get => _id;
        set
        {
            if (_id == value) return;
            _id = value;
            Barcode3 = (Barcode3)_id.ToString();
        }
    }
    protected abstract string Type { get; }
    public string Name { get; set; }
    protected abstract string Information { get; }
    public IBarcode3 Barcode3 { get; set; }

    public override string ToString()
    {
        return $"\n\t{Type}:\t{Name}\n\t{Information}\n\n{Barcode3}";
    }
}