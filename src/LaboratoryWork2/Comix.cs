using Currency;

namespace LaboratoryWork2
{
    public sealed class Comix : Book
    {
        public Comix(string name, Usd price, string author, uint pages) : base(name, price, author, pages)
        {

        }

        /// <summary>
        /// Для возможности сравнения
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int CompareTo(object? obj)
        {
            if (!(obj is Product product))
            {
                if (ReferenceEquals(this, obj)) return 0;
                return -1;
            }
            if (Usd.Cost == product.Usd.Cost) return 0;
            if (Usd.Cost > product.Usd.Cost) return 1;
            return -1;
        }
    }
}
