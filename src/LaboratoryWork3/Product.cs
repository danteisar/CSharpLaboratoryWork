using Currency;

namespace LaboratoryWork3
{
   
    /// <summary>
    /// Товар
    /// </summary>
    public abstract class Product : IProduct
    {
        #region ctor

        protected Product(string name)
        {
            Name = name;
        }
        protected Product(string name, Rub price) : this(name)
        {
            Rub = price;
        }

        protected Product(string name, Usd price) : this(name)
        {

            Usd = price;
        }

        #endregion

        #region _fields 

        private Rub _rub;
        private Usd _usd;

        #endregion

        #region Props

        /// <summary>
        /// Цена в рублях
        /// </summary>
        public Rub Rub
        {
            get => _rub;
            set
            {
                _rub = value;
                _usd = value * (Usd)IProduct.Cource;
            }
        }

        /// <summary>
        /// Цена в Евро
        /// </summary>
        public Usd Usd
        {
            get => _usd;
            set
            {
                _rub = value * (Rub)IProduct.Cource;
                _usd = value;
            }
        }

        /// <summary>
        /// Наименование товара
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}
