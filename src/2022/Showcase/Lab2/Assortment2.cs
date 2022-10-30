using Product.Lab2;

namespace Showcase.Lab2;

public class Assortment2
{
    #region initialization

    private Assortment2(int size)
    {
        Size = size;
        _products = new Thing[size];
        Id = Store.Amount++;
    }

    private readonly Thing[] _products;

    public static implicit operator Assortment2(int size) => new(size);

    #endregion

    #region Props & _fields

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

    public Thing this[int index]
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

    public void Push(Thing thing)
    {
        Push(thing, Array.IndexOf(_products, null));
    }

    public void Push(Thing thing, int index)
    {
        this[index] = thing;
    }

    public Thing Pop()
    {
        return this[Find()];
    }

    public Thing Pop(int index)
    {
        return this[index];
    }

    public void Swap(int index1, int index2)
    {
        (this[index1], this[index2]) = (this[index2], this[index1]);
    }

    public Thing Replace(Thing thing, int index)
    {
        var tmp = this[index];
        this[index] = thing;
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

    private void OrderBy(IEnumerable<Thing> products)
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

    private void SetBarcode(Thing thing, int index)
    {
        if (thing == null) return;
        thing.Barcode.Text = $"{Id} {index + 1} {thing.Id}";
    }

    public override string ToString() => _products.Aggregate($"\tАссортимент #{Id}:\n", (current, product) => current + (product == null ? "\t- пусто -\n" : $"{product}\n"));
}