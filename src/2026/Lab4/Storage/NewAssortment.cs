using Lab2.Abstractions;
using Lab3.Abstractions;
using Lab3.Extensions;
using Lab3.Storage;
using Lab4.Abstractions;
using Lab4.Events;

namespace Lab4.Storage;

public sealed class NewAssortment<T> : Assortment<T>, IDisposable
    where T : class, INewProduct
{
    #region initialization

    private NewAssortment(int size) : base(size)
    {

    }

    public static implicit operator NewAssortment<T>(int size) => new(size);

    #endregion

    #region Props & _fields

    private Action<IAssortment<T>> _onIdChanged;

    public override long Id
    {
        get => _id;
        set
        {
            if (_id == value) return;
            _id = value;
            _onIdChanged?.Invoke(this);
        }
    }

    #endregion

    #region index

    public override T this[int index]
    {
        get
        {
            var thing = base[index];

            if (thing is { })
            {
                _onIdChanged -= thing.SetCode;
                thing.DataChanged -= OnProductDataChanged;
            }
           
            return thing;
        }
        set
        {
            base[index] = value;

            if (_things[index] == value)
            {
                _onIdChanged += value.SetCode;
                value.DataChanged += OnProductDataChanged;
            }
        }   
    }

    private void OnProductDataChanged(object sender, ProductEventArgs e)
    {
        if (sender is T product)
        {
            product.SetCode(this);
        }
    }

    #endregion

    #region Search & find + delegates

    public override int Find() => FindBy(x => x != null);

    public override int Find(long id) => FindBy(x => x?.Id == id);

    public override int Find(string name) => FindBy(x => x?.Name == name);

    private int FindBy(Func<T, bool> selector)
    {
        return Array.IndexOf(_things, _things.FirstOrDefault(selector));
    }

    public override void OrderById() => OrderBy(x => x?.Id);

    public override void OrderByName() => OrderBy(x => x?.Name);

    private void OrderBy<TKey>(Func<T, TKey> select)
    {
        var things = _things
            .Where(p => p != null)
            .OrderBy(i => select(i))
            .Select(_ => Pop());

        foreach (var thing in things)
        {
            Push(thing);
        }
    }

    public void Dispose()
    {
        foreach (var thing in _things.Where(x=>x is { }))
        {
            _onIdChanged -= thing.SetCode;
            thing.DataChanged -= OnProductDataChanged;
        }
    }

    #endregion
}
