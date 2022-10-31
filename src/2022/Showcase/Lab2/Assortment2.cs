using Product.Lab2;

namespace Showcase.Lab2;

public class Assortment2
{
    #region initialization

    private Assortment2(int size)
    {
        Size = size;
        _things = new Thing2[size];
        Id = Store.Amount++;
    }

    private readonly Thing2[] _things;

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

    public Thing2 this[int index]
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

    public void Push(Thing2 thing2)
    {
        Push(thing2, Array.IndexOf(_things, null));
    }

    public void Push(Thing2 thing2, int index)
    {
        this[index] = thing2;
    }

    public Thing2 Pop()
    {
        return this[Find()];
    }

    public Thing2 Pop(int index)
    {
        return this[index];
    }

    public void Swap(int index1, int index2)
    {
        (this[index1], this[index2]) = (this[index2], this[index1]);
    }

    public Thing2 Replace(Thing2 thing2, int index)
    {
        var tmp = this[index];
        this[index] = thing2;
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

    private void OrderBy(IEnumerable<Thing2> things)
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

    private void SetBarcode(Thing2 thing2, int index)
    {
        if (thing2 == null) return;
        thing2.Barcode.Text = $"{Id} {index + 1} {thing2.Id}";
    }

    public override string ToString() => _things.Aggregate($"\tАссортимент #{Id}:\n", (current, thing) => current + (thing == null ? "\t- пусто -\n" : $"{thing}\n"));
}