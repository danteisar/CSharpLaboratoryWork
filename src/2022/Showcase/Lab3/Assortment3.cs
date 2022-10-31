using Barcode.Lab1;
using Product.Lab3;
using Showcase.Lab2;

namespace Showcase.Lab3;

public class Assortment3<T> : IAssortment3<T> where T : class, IThing3
{
    #region initialization

    private Assortment3(int size)
    {
        Size = size;
        _things = new T[size];
        Id = Store.Amount++;
    }

    private readonly T[] _things;

    public static implicit operator Assortment3<T>(int size) => new(size);

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

    public T this[int index]
    {
        get
        {
            if (index > _things.Length - 1 || index < 0) return null;
            var thing = _things[index];
            _things[index] = null;
            return thing;
        }
        set
        {
            if (index > _things.Length - 1 || index < 0) return;
            if (_things[index] != null) return;
            _things[index] = value;

            SetBarcode(value, index);
        }
    }

    public void Push(T thing)
    {
        Push(thing, Array.IndexOf(_things, null));
    }

    public void Push(T thing, int index)
    {
        this[index] = thing;
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

    public T Replace(T thing, int index)
    {
        var tmp = this[index];
        this[index] = thing;
        return tmp;
    }

    #endregion

    #region Search & find

    public int Find()
    {
        return Array.IndexOf(_things, _things.FirstOrDefault(x => x != null));
    }

    public int Find(int id)
    {
        return Array.IndexOf(_things, _things.FirstOrDefault(x => x?.Id == id));
    }

    public int Find(string name)
    {
        return Array.IndexOf(_things, _things.FirstOrDefault(x => x?.Name == name));
    }

    public void OrderById()
    {
        var showcase = _things
            .Where(p => p != null)
            .Select(_ => Pop());

        OrderBy(showcase);
    }

    public void OrderByName()
    {
        var showcase = _things
            .Where(p => p != null)
            .Select(_ => Pop());

        OrderBy(showcase);
    }

    private void OrderBy(IEnumerable<T> things)
    {
        foreach (var thing in things.OrderBy(i => i?.Name))
        {
            Push(thing);
        }
    }

    #endregion

    private void SetBarcode()
    {
        for (var index = 0; index < _things.Length; index++)
        {
            SetBarcode(_things[index], index);
        }
    }

    private void SetBarcode(T thing, int index)
    {
        if (thing == null) return;
        thing.Barcode = (Barcode1)$"{Id} {index + 1} {thing.Id}";
    }

    public override string ToString() => _things.Aggregate($"\tАссортимент #{Id}:\n", (current, thing) => current + (thing == null ? "\t- пусто -\n" : $"{thing}\n"));
}
