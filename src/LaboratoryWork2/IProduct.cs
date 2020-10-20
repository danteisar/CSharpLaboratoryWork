using Currency;

namespace LaboratoryWork2
{
    /// <summary>
    /// Абстракция продукции
    /// </summary>
    public interface IProduct
    {
        /// <summary>
        /// Цена в валюте №1
        /// </summary>
        Rub Rub { get; set; }
        /// <summary>
        /// Цена в валюте №2
        /// </summary>
        Usd Usd { get; set; }
        string Name { get; set; }
        /// <summary>
        /// Текущий курс валют
        /// </summary>
        static Course Course;
    }
}
