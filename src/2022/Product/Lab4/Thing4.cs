using Barcode.Lab3;

namespace Product.Lab4;

public abstract class Thing4 : IThing4
{
    protected Thing4(int id, string name)
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
            var oldId = _id;
            _id = value;
            Barcode3 = (Barcode3)_id.ToString();
            OnIdChange(oldId, value);
        }
    }
    protected abstract string Type { get; }
    public string Name { get; set; }
    protected abstract string Information { get; }
    public IBarcode3 Barcode3 { get; set; }

    public event EventHandler<Thing4IdEventArgs> ThingIdChanged;

    private void OnIdChange(int oldId, int newId)
    {
        ThingIdChanged?.Invoke(this, new(oldId, newId));
    }

    public override string ToString()
    {
        return $"\n\t{Type}:\t{Name}\n\t{Information}\n\n{Barcode3}";
    }
}