namespace Product;

public abstract class Product : IProduct
{
    protected Product(int id, string name)
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
            if (Barcode == null) 
                Barcode = new Barcode.Barcode(_id.ToString());
            else
                Barcode.Text = _id.ToString();
        }
    }   
    protected abstract string Type { get; }
    public string Name { get; set; }
    protected abstract string Information { get; }
    public Barcode.Barcode Barcode { get; set; }


    public override string ToString()
    {
        return $"\n\t{Type}:\t{Name}\n\t{Information}\n\n{Barcode}";
    }
}