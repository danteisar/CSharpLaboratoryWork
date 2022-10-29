using LaboratoryWorks.BarCode;
using System.Collections.Generic;
using System.Linq;
using static LaboratoryWorks.BarCode.BarCodeGenerator;

namespace LaboratoryWork1._1
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

                value.Bar = (Generate(value.Code) + Generate(index) + Generate(Code)).Replace($"{Spliter}{Spliter}", $"{Spliter}");
            }
        }

        #endregion
            
        public int Code { get; set; }

        /// <summary>
        ///     Перегрузка метода ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _storage.Aggregate("Товар:\n", (current, product) => current + (product == null ? "- пусто -\n" : $"{product}\n"));
    }
}
