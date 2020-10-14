using System.Collections.Generic;
using System.Linq;
using Currency;

namespace LaboratoryWork1
{
    public class Workshop
    {
        #region ctor

        private Workshop(int slots)
        {
            _storage = new Product[slots];
        }
        public static implicit operator Workshop(ushort count) => new Workshop(count);

        public static implicit operator Workshop(List<Product> products)
        {
            var workshop = new Workshop(products.Count);

            for (var i = 0; i < products.Count; i++)
            {
                workshop[i] = products[i];
            }

            return workshop;
        }

        #endregion

        #region _fields

        private readonly Product[] _storage;

        #endregion

        #region Index

        public Product this[int index]
        {
            get
            {
                if (index > _storage.Length - 1 || index < 0) return null;
                var product = _storage[index];
                _storage[index] = null;
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
        public Product Put(Product product)
        {
            for (var i = 0; i < _storage.Length; i++)
            {
                if (_storage[i] != null) continue;
                this[i] = product;
                return null;
            }
            return product;
        }

        /// <summary>
        /// Положить товар в ячейку, либо вернуть если место занято или пытались положить мимо
        /// </summary>
        /// <param name="product"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Product Put(Product product, int index)
        {
            var productInStorage = this[index];

            if (productInStorage != null)
            {
                this[index] = productInStorage;
                return product;
            }

            this[index] = product;
            return null;
        }

        /// <summary>
        /// Достать товар
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Product Get(int index) => this[index];

        /// <summary>
        /// Достать товар
        /// </summary>
        /// <returns></returns>
        public Product Get()
        {
            for (var i = 0; i < _storage.Length; i++)
            {
                var s = Get(i);
                if (s != null) return s;
            }
            return null;
        }
        #endregion

        #region Обмен

        /// <summary>
        /// Заметить товар в ячейке своим
        /// </summary>
        /// <param name="product"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Product Replace(Product product, int index)
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
        public Product[] SearchByName(string name) => _storage.Where(s => s?.Name == name).ToArray();

        /// <summary>
        /// Отсортировать товар по цене
        /// </summary>  
        /// <returns></returns>
        public void SortedByCost()  
        {
            var showcase = _storage.Where(p => p != null).Select(_ => Get()).ToList();

            foreach (var product in from i in showcase orderby i?.Rub.Cost + i?.Usd.Cost descending select i)
            {
                Put(product);
            }
        }

        #endregion

        #region Курс валют

        /// <summary>
        /// Изменить курс
        /// </summary>
        /// <param name="newCource"></param>
        public void Cource(Usd newCource)
        {
            Product.CurrentCource = newCource;
            foreach (var product in _storage.Where(s => s != null))
            {
                product.Rub = product.Rub;
            }
        }

        /// <summary>
        /// Изменить курс
        /// </summary>
        /// <param name="newCource"></param>
        public void Cource(Rub newCource)
        {
            Product.CurrentCource = newCource;
            foreach (var product in _storage.Where(s => s != null))
            {
                product.Rub = product.Rub;
            }
        }

        /// <summary>
        /// Текущий курс
        /// </summary>
        /// <returns></returns>
        public Cource Cource() => Product.CurrentCource;
        #endregion

        /// <summary>
        /// Перегрузка метода ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _storage.Aggregate("Товар:\n", (current, product) => current + (product == null ? "- пусто -\n" : $"{product.Name} по цене {product.Rub} руб. ({product.Usd} евро))\n"));

        #endregion
    }
}
