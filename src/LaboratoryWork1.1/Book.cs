namespace LaboratoryWork1._1
{
    public class Book : Product
    {
        public ushort Tome { get; set; }
        public string Author { get; set; }
        public uint Pages { get; set; }

        public Book(string name, int code, ushort tome, string author, uint pages) : base(code, name)
        {
            Tome = tome;
            Author = author;
            Pages = pages;
        }

        public override string ToString() => $"Книга: {base.ToString()}, Автор:{Author}{(Tome == 0 ? string.Empty : $", Том: {Tome}")}, Страниц: {Pages}";
    }
}
