using Currency;

namespace LaboratoryWork1
{
   
    /// <summary>
    /// Товар
    /// </summary>
    public abstract class Product
    {
        public static Cource CurrentCource { get; set; } = (Rub)100;

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
                _usd = value * (Usd)CurrentCource;
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
                _rub = value * (Rub)CurrentCource;
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
