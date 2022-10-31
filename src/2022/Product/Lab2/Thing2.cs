using Barcode.Lab1;

namespace Product.Lab2;

public abstract class Thing2
{
    protected Thing2(int id, string name)
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
            Barcode = _id.ToString();
        }
    }   
    protected abstract string Type { get; }
    public string Name { get; set; }
    protected abstract string Information { get; }
    public Barcode1 Barcode { get; set; }
    
    public override string ToString()
    {
        return $"\n\t{Type}:\t{Name}\n\t{Information}\n\n{Barcode}";
    }
}