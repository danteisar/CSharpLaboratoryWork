namespace Lab3.Abstractions;

/// <summary>
/// Ассортимент товара
/// </summary>
/// <typeparam name="T">Ограниченный ссылочным типом, реализующий интерфейс товара</typeparam>
public interface IAssortment<T>
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    long Id { get; set; }
    /// <summary>
    /// Размер
    /// </summary>
    int Size { get; }
    /// <summary>
    /// Индексатор
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    T this[int index] { get; set; }
    /// <summary>
    /// Добавление
    /// </summary>
    /// <param name="thing"></param>
    void Push(T thing);
    /// <summary>
    /// Добавление в ячейку
    /// </summary>
    /// <param name="thing"></param>
    /// <param name="index"></param>
    void Push(T thing, int index);
    /// <summary>
    /// Доставание
    /// </summary>
    /// <returns></returns>
    T Pop();
    /// <summary>
    /// Доставание по индексу
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    T Pop(int index);
    /// <summary>
    /// Перестановка
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    void Swap(int index1, int index2);
    /// <summary>
    /// Замена
    /// </summary>
    /// <param name="thing"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    T Replace(T thing, int index);
    /// <summary>
    /// Поиск свобной ячейки
    /// </summary>
    /// <returns></returns>
    int Find();
    /// <summary>
    /// Поиск по идентифкатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    int Find(long id);
    /// <summary>
    /// Поиск по имени
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int Find(string name);
    /// <summary>
    /// Сортировка по идентифкатору
    /// </summary>
    void OrderById();
    /// <summary>
    /// Сортировка по имени
    /// </summary>
    void OrderByName();
}