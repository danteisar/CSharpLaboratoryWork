using Currency;

namespace LaboratoryWork3
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

        /// <summary>
        /// Название продукции
        /// </summary>
        string Name { get; set; }

        #region Default implementation

        private static Course _course;

        /// <summary>
        /// Текущий курс валют
        /// </summary>
        static Course Course
        {
            get => _course;
            set
            {
                var old = _course;
                _course = value;
                CourseChanged?.Invoke(old, _course);
            }
        }

        /// <summary>
        /// Изменение курса
        /// </summary>
        static CourseChanged CourseChanged;

        #endregion
    }
}
