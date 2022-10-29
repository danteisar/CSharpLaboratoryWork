using Currency;

namespace LaboratoryWork3
{
    public interface IWorkshop<T> where T : class, IProduct
    {
        T this[int index] { get; set; }

        /// <summary>
        /// Положить товар в первую пустую ячейку   
        /// </summary>
        /// <param name="product"></param>  
        /// <returns></returns>
        T Put(T product);

        /// <summary>
        /// Положить товар в ячейку, либо вернуть если место занято или пытались положить мимо
        /// </summary>
        /// <param name="product"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        T Put(T product, int index);

        /// <summary>
        /// Достать товар
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T Get(int index);

        /// <summary>
        /// Достать товар
        /// </summary>
        /// <returns></returns>
        T Get();

        /// <summary>
        /// Заметить товар в ячейке своим
        /// </summary>
        /// <param name="product"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        T Replace(T product, int index);

        /// <summary>
        /// Заменить товар в ячейках
        /// </summary>
        /// <param name="firstIndex"></param>
        /// <param name="secondIndex"></param>
        void Replace(int firstIndex, int secondIndex);

        /// <summary>
        /// Найти товары по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        T[] SearchByName(string name);

        /// <summary>
        /// Отсортировать товар по цене
        /// </summary>  
        /// <returns></returns>
        void SortedByCost();

        /// <summary>
        /// Текущий курс
        /// </summary>
        /// <returns></returns>
        Course Course { get; set; }
    }
}
