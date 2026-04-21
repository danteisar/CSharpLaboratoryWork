using Lab2.Abstractions;
using Lab3.Abstractions;
using Lab3.Extensions;

namespace Lab3.Storage;

public class Assortment<T> : IAssortment<T> where T : class, IProduct
{
    #region initialization

    protected Assortment(int size)
    {
        Size = size;
        _things = new T[size];
        Id = _amount++;
    }

    public static implicit operator Assortment<T>(int size) => new(size);

    #endregion

    #region Props & _fields

    private static long _amount = 1;

    protected long _id;
    protected readonly T[] _things;

    public virtual long Id
    {
        get => _id;
        set
        {
            if (_id == value) return;
            _id = value;
            SetCode();
        }
    }

    public int Size { get; }

    #endregion

    #region index

    public virtual T this[int index]
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

            value.SetCode(Id, index);
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

    public virtual int Find()
    {
        return Array.IndexOf(_things, _things.FirstOrDefault(x => x != null));
    }

    public virtual int Find(long id)
    {
        return Array.IndexOf(_things, _things.FirstOrDefault(x => x?.Id == id));
    }

    public virtual int Find(string name)
    {
        return Array.IndexOf(_things, _things.FirstOrDefault(x => x?.Name == name));
    }

    public virtual void OrderById()
    {
        var things = _things
            .Where(p => p != null)
            .OrderBy(i => i?.Id)
            .Select(_ => Pop());

        foreach (var thing in things)
        {
            Push(thing);
        }
    }

    public virtual void OrderByName()
    {
        var things = _things
           .Where(p => p != null)
           .OrderBy(i => i?.Name)
           .Select(_ => Pop());

        foreach (var thing in things)
        {
            Push(thing);
        }
    }

    #endregion

    private void SetCode()
    {
        for (var index = 0; index < _things.Length; index++)
        {
            if (_things[index] is null) continue;

            _things[index].SetCode(Id, index);
        }
    }

    public override string ToString() => _things.Aggregate($"\tАссортимент #{Id}:\n", (current, thing) => current + (thing == null ? "\t- пусто -\n" : $"{thing}\n"));
}
