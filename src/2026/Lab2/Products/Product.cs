using Lab1.Abstractions;
using Lab2.Abstractions;

namespace Lab2.Products;

public abstract class Product : IProduct
{
    private long _id;
    private decimal price;

    public virtual long Id
    {
        get => _id;
        set
        {
            if (_id == value) return;
            _id = value;
            UpdateCode();
        }
    }    
    public virtual decimal Price
    {
        get => price;
        set
        {
            if (decimal.Equals(price, value)) return;
            price = value;
            UpdateCode();
        }
    }

    public string Name { get; }
    public IProductCode Code { get; }
    public abstract string Type { get; }    
    public abstract string Information { get; }

    protected Product(long id, decimal price, string name, IProductCode code)
    {
        _id = id;
        Name = name;
        Code = code;
        Price = price;
    }    

    private void UpdateCode()
    {
        Code.Text = $"{Id}:{Price}";
    }

    public override string ToString()
    {
        return $"\n\t{Type}:\t{Name}\n\t{Information}\n{Code}";
    }
}
