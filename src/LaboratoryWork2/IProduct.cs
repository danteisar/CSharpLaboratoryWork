using System;
using Currency;

namespace LaboratoryWork2
{
    public interface IProduct
    {
        Rub Rub { get; set; }
        Usd Usd { get; set; }
        string Name { get; set; }

        static Cource Cource;
    }
}
