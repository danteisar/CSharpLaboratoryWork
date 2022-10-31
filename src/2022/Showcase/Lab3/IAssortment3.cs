using Product.Lab3;

namespace Showcase.Lab3;

public interface IAssortment3<T> where T : class, IThing3
{
    int Id { get; set; }
    int Size { get; }
    T this[int index] { get; set; }
    void Push(T thing);
    void Push(T thing, int index);
    T Pop();
    T Pop(int index);
    void Swap(int index1, int index2);
    T Replace(T thing, int index);
    int Find();
    int Find(int id);
    int Find(string name);
    void OrderById();
    void OrderByName();
}