using LaboratoryWorks.BarCode;

namespace LaboratoryWork1._1
{
    public abstract class Product
    {
        private BarCode _barCode;

        public string Name { get; set; }

        public static bool ByBar { get; set; }

        public int Code
        {
            get => _barCode;
            set => _barCode.Code = value;
        }

        public string Bar
        {
            get => _barCode;
            set => _barCode.Bar = value;
        }

        public Product(int code, string name)
        {
            Name = name;
            _barCode = code;
        }

        public override string ToString() => $"Товар: {Name}, Код: {(ByBar ? Bar : Code.ToString())}";
    }
}
