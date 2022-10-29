using System.Collections.Generic;
using System.Linq;
using Currency;

namespace LaboratoryWork2
{
   public class Workshop<T> : IWorkshop<T> where T: class, IProduct
    {
        #region ctor

        private Workshop(int slots)
        {
            _storage = new T[slots];
        }
        public static implicit operator Workshop<T>(ushort count) => new Workshop<T>(count);

        public static implicit operator Workshop<T>(List<T> products)
        {
            var workshop = new Workshop<T>(products.Count);

            for (var i = 0; i < products.Count; i++)
            {
                workshop[i] = products[i];
            }

            return workshop;
        }

        #endregion

        #region _fields

        private readonly T[] _storage;

        #endregion

        #region Index

        public T this[int index]
        {
            get
            {
                if (index > _storage.Length - 1 || index < 0) return default(T);
                var product = _storage[index];
                _storage[index] = default;
                return product;
            }
            set
            {
                if (index > _storage.Length - 1 || index < 0) return;
                if (_storage[index] != null) return;
                _storage[index] = value;
            }
        }

        #endregion

        #region Methods
        
        #region Получение и возврат

        /// <summary>
        /// Положить товар в первую пустую ячейку
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public T Put(T product)
        {
            for (var i = 0; i < _storage.Length; i++)
            {
                if (_storage[i] != null) continue;
                this[i] = product;
                return default;
            }
            return product;
        }

        /// <summary>
        /// Положить товар в ячейку, либо вернуть если место занято или пытались положить мимо
        /// </summary>
        /// <param name="product"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public T Put(T product, int index)
        {
            var productInStorage = this[index];

            if (productInStorage != null)
            {
                this[index] = productInStorage;
                return product;
            }

            this[index] = product;
            return default;
        }

        /// <summary>
        /// Достать товар
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T Get(int index) => this[index];

        /// <summary>
        /// Достать товар
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            for (var i = 0; i < _storage.Length; i++)
            {
                var s = Get(i);
                if (s != null) return s;
            }
            return default;
        }

        #endregion

        #region Обмен

        /// <summary>
        /// Заметить товар в ячейке своим
        /// </summary>
        /// <param name="product"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public T Replace(T product, int index)
        {
            var inStorageProduct = Get(index);
            Put(product, index);
            return inStorageProduct;
        }

        /// <summary>
        /// Заменить товар в ячейках
        /// </summary>
        /// <param name="firstIndex"></param>
        /// <param name="secondIndex"></param>
        public void Replace(int firstIndex, int secondIndex)
        {
            var inStorageProduct1 = Get(firstIndex);
            var inStorageProduct2 = Get(secondIndex);
            Put(inStorageProduct2, firstIndex);
            Put(inStorageProduct1, secondIndex);
        }

        #endregion

        #region Сортировка и поиск

        /// <summary>
        /// Найти товары по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T[] SearchByName(string name) => _storage.Where(s => s?.Name == name).ToArray();

        /// <summary>
        /// Отсортировать товар по цене
        /// </summary>  
        /// <returns></returns>
        public void SortedByCost()
        {
            var showcase = _storage.Where(p => p != null).Select(_ => Get()).ToList();

            foreach (var product in from i in showcase orderby i select i)
            {
                Put(product);
            }
        }

        #endregion

        #region Курс валют

        /// <summary>
        /// Изменить курс
        /// </summary>
        /// <param name="newCourse"></param>
        public void Course(Usd newCourse)
        {
            IProduct.Course = newCourse;
            foreach (var product in _storage.Where(s => s != null))
            {
                product.Rub = product.Rub;
            }
        }

        /// <summary>
        /// Изменить курс
        /// </summary>
        /// <param name="newCourse"></param>
        public void Course(Rub newCourse)
        {
            IProduct.Course = newCourse;
            foreach (var product in _storage.Where(s => s != null))
            {
                product.Rub = product.Rub;
            }
        }

        /// <summary>
        /// Текущий курс
        /// </summary>
        /// <returns></returns>
        public Course Course() => IProduct.Course;

        #endregion

        /// <summary>
        /// Перегрузка метода ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _storage.Aggregate("Товар:\n", (current, product) => current + (product == null ? "- пусто -\n" : $"{product}\n"));

        #endregion
    }
}
