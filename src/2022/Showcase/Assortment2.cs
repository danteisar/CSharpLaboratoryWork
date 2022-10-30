using Product;

namespace Showcase;

public class Assortment2<T> : IAssortment<T> where T : class, IProduct
{
    #region initialization

    private Assortment2(int size)
    {
        Size = size;
        _products = new T[size];
        Id = _nextId++;
    }

    private readonly T[] _products;

    public static implicit operator Assortment2<T>(int size) => new(size);

    #endregion

    #region Props & _fields

    private static int _nextId = 1;
    private int _id;

    public int Id
    {
        get => _id;
        set
        {
            if (_id == value) return;
            _id = value;
            SetBarcode();
        }
    }

    public int Size { get; }

    #endregion


    #region index

    public T this[int index]
    {
        get
        {
            if (index > _products.Length - 1 || index < 0) return null;
            var product = _products[index];
            _products[index] = null;
            return product;
        }
        set
        {
            if (index > _products.Length - 1 || index < 0) return;
            if (_products[index] != null) return;
            _products[index] = value;

            SetBarcode(value, index);
        }
    }

    public void Push(T product)
    {
        Push(product, Array.IndexOf(_products, null));
    }

    public void Push(T product, int index)
    {
        this[index] = product;
    }

    public T Pop()
    {
        return this[Find()];
    }

    public T Pop(int index)
    {
        return this[index];
    }

    public void Swap(int index1, int index2)
    {
        (this[index1], this[index2]) = (this[index2], this[index1]);
    }

    public T Replace(T product, int index)
    {
        var tmp = this[index];
        this[index] = product;
        return tmp;
    }

    #endregion

    #region Search & find

    public int Find()
    {
        return Array.IndexOf(_products, _products.FirstOrDefault(x => x != null));
    }

    public int Find(int id)
    {
        return Array.IndexOf(_products, _products.FirstOrDefault(x => x?.Id == id));
    }

    public int Find(string name)
    {
        return Array.IndexOf(_products, _products.FirstOrDefault(x => x?.Name == name));
    }

    public void OrderById()
    {
        var showcase = _products
            .Where(p => p != null)
            .Select(_ => Pop());

        OrderBy(showcase);
    }

    public void OrderByName()
    {
        var showcase = _products
            .Where(p => p != null)
            .Select(_ => Pop());

        OrderBy(showcase);
    }

    private void OrderBy(IEnumerable<T> products)
    {
        foreach (var product in products.OrderBy(i => i?.Name))
        {
            Push(product);
        }
    }

    #endregion

    private void SetBarcode()
    {
        for (var index = 0; index < _products.Length; index++)
        {
            SetBarcode(_products[index], index);
        }
    }

    private void SetBarcode(T product, int index)
    {
        if (product == null) return;
        product.Barcode.Text = $"{Id} {index + 1} {product.Id}";
    }

    public override string ToString() => _products.Aggregate($"\tАссортимент #{Id}:\n", (current, product) => current + (product == null ? "- пусто -\n" : $"{product}\n"));
}
