using Currency;

namespace LaboratoryWork1
{
    public class Book : Product
    {
        #region ctor

        private void Init(string author, uint pages, ushort tome = 0)
        {
            Author = author;
            Pages = pages;
            Tome = tome;
        }

        public Book(string name, Rub roubles, string author, uint pages, ushort tome = 0) : base(name, roubles)
        {
            Init(author, pages, tome);
        }

        public Book(string name, Usd euro, string author, uint pages, ushort tome = 0) : base(name, euro)
        {
            Init(author, pages, tome);
        }

        #endregion

        #region Props

        public ushort Tome { get; set; }
        public string Author { get; set; }
        public uint Pages { get; set; }

        #endregion

        public static bool operator ==(Book book1, Product book2) => book1?.Rub.Cost + book1?.Usd.Cost == book2?.Rub.Cost + book2?.Usd.Cost;
        public static bool operator !=(Book book1, Product book2) => book1?.Rub.Cost + book1?.Usd.Cost != book2?.Rub.Cost + book2?.Usd.Cost;
        public static bool operator >(Book book1, Book book2) => book1?.Rub.Cost + book1?.Usd.Cost > book2?.Rub.Cost + book2?.Usd.Cost;
        public static bool operator <(Book book1, Book book2) => book1?.Rub.Cost + book1?.Usd.Cost < book2?.Rub.Cost + book2?.Usd.Cost;

        public override string ToString() => $"{Name}, Author:{Author}{(Tome == 0 ? string.Empty : $", Том: {Tome}")}, Страниц: {Pages}, Цена: {Rub} ({Usd})";

    }
}
