using Lab2.Products;
using Lab4.Abstractions;
using Lab4.Events;

namespace Lab4.Products;

public sealed class NewBook(long id, string name, string author, int year, decimal price)
    : Book(id, name, author, year, price), INewProduct
{
    public event EventHandler<ProductEventArgs> DataChanged;

    public override long Id
    {
        get => base.Id;
        set
        {
            if (base.Id == value) return;
            base.Id = value;
            DataChanged?.Invoke(this, new ProductEventArgs(value, base.Price));
        }
    }

    public override decimal Price
    {
        get => base.Price;
        set
        {
            if (decimal.Equals(value, base.Price)) return;
            base.Price = value;
            DataChanged?.Invoke(this, new ProductEventArgs(base.Id, value));
        }
    }
}