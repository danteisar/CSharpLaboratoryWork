using Product.Lab4;
using Showcase.Lab2;

namespace Showcase.Lab4;

public class Assortment4<T> : IAssortment4<T> where T : class, IThing4
{
    #region initialization

    private Assortment4(int size)
    {
        Size = size;
        _things = new T[size];
        Id = Store.Amount++;
    }

    private readonly T[] _things;

    public static implicit operator Assortment4<T>(int size) => new(size);

    #endregion

    #region Props & _fields

    private Action<IAssortment4<T>> _idChanged;

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
            if (index > _things.Length - 1 || index < 0) return null;
            var thing = _things[index];
            _things[index] = null;
           
            _idChanged -= thing.SetBarcode;
            thing.ThingIdChanged -= ThingOnIdChanged;
            
            return thing;
        }
        set 
        {
            if (index > _things.Length - 1 || index < 0) return;
            if (_things[index] != null) return;
            _things[index] = value;
            
            _idChanged += value.SetBarcode;
            value.ThingIdChanged += ThingOnIdChanged;
            
            value.SetBarcode(Id, index);
        }
    }

    private void ThingOnIdChanged(object sender, Thing4IdEventArgs e)
    {
        if (sender is not T thing) return;
        var index = Find(thing.Id);
        Replace(thing, index);
    }

    public void Push(T thing) => Push(thing, Array.IndexOf(_things, null));
    public void Push(T thing, int index) => this[index] = thing;

    public T Pop() => this[Find()];
    public T Pop(int index) => this[index];

    public void Swap(int index1, int index2) => (this[index1], this[index2]) = (this[index2], this[index1]);
    public T Replace(T thing, int index)
    {
        var tmp = this[index];
        this[index] = thing;
        return tmp;
    }

    #endregion

    #region Search & find

    public int Find() => Find(x => x != null);
    public int Find(int id) => Find(x => x?.Id == id);
    public int Find(string name) => Find(x => x?.Name == name);
    private int Find(Func<T, bool> condition) => Array.IndexOf(_things, _things.FirstOrDefault(condition));

    public void OrderById() => OrderBy(x => x.Id);
    public void OrderByName() => OrderBy(x => x.Name);

    private void OrderBy<TD>(Func<T, TD> keySelector)
    {
        var things = _things
            .Where(p => p != null)
            .Select(_ => Pop());

        foreach (var thing in things.OrderBy(keySelector))
        {
            Push(thing);
        }
    }

    #endregion

    public override string ToString() => _things.Aggregate($"\tАссортимент #{Id}:\n", (current, thing) => current + (thing == null ? "\t- пусто -\n" : $"{thing}\n"));
}
