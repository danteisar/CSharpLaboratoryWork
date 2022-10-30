using Product.Lab4;

namespace Showcase.Lab4;

public interface IAssortment4<T> where T : class, IThing4
{
    int Id { get; set; }
    int Size { get; }
    T this[int index] { get; set; }
    void Push(T product);
    void Push(T product, int index);
    T Pop();
    T Pop(int index);
    void Swap(int index1, int index2);
    T Replace(T product, int index);
    int Find();
    int Find(int id);
    int Find(string name);
    void OrderById();
    void OrderByName();
}