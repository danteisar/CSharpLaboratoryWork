using Currency;

namespace LaboratoryWork3
{
    public interface IProduct
    {
        Rub Rub { get; set; }
        Usd Usd { get; set; }
        string Name { get; set; }

        static Cource Cource;
    }
}
