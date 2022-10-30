using Product;

namespace Showcase;

public class Assortment3<T> : IAssortment<T> where T : class, IProduct
{
    #region initialization

    private Assortment3(int size)
    {
        Size = size;
        _products = new T[size];
        Id = _nextId++;
    }

    private readonly T[] _products;

    public static implicit operator Assortment3<T>(int size) => new(size);

    #endregion

    #region Props & _fields

    private Action<IAssortment<T>> _idChanged;

    private static int _nextId = 1;

    private int _id;

    public int Id
    {
        get => _id;
        set
        {
            if (_id == value) return;
            _id = value;
            _idChanged?.Invoke(this);
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
            _idChanged -= product.SetBarcode;
            return product;
        }
        set
        {
            if (index > _products.Length - 1 || index < 0) return;
            if (_products[index] != null) return;
            _products[index] = value;
            _idChanged += value.SetBarcode;
            value.SetBarcode(Id, index);
        }
    }

    public void Push(T product) => Push(product, Array.IndexOf(_products, null));
    public void Push(T product, int index) => this[index] = product;

    public T Pop() => this[Find()];
    public T Pop(int index) => this[index];

    public void Swap(int index1, int index2) => (this[index1], this[index2]) = (this[index2], this[index1]);
    public T Replace(T product, int index)
    {
        var tmp = this[index];
        this[index] = product;
        return tmp;
    }

    #endregion

    #region Search & find

    public int Find() => Find(x => x!= null);
    public int Find(int id) => Find(x => x?.Id == id);
    public int Find(string name) => Find(x=>x?.Name == name);
    private int Find(Func<T, bool> condition) => Array.IndexOf(_products, _products.FirstOrDefault(condition));

    public void OrderById() => OrderBy(x=>x.Id);
    public void OrderByName() => OrderBy(x => x.Name);

    private void OrderBy<TD>(Func<T, TD> keySelector)
    {
        var products = _products
            .Where(p => p != null)
            .Select(_ => Pop());

        foreach (var product in products.OrderBy(keySelector))
        {
            Push(product);
        }
    }

    #endregion
    
    public override string ToString() => _products.Aggregate($"\tАссортимент #{Id}:\n", (current, product) => current + (product == null ? "- пусто -\n" : $"{product}\n"));
}
