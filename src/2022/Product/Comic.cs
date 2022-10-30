namespace Product
{
    public sealed class Comic : Book
    {
        public Comic(int id, string name, string author, int year, decimal price) : base(id, name, author, year, price)
        {
        }

        protected override string Type => "Комикс";
    }
}