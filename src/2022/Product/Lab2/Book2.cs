namespace Product.Lab2;

public class Book2 : Thing2
{
    public string Author { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }

    public Book2(int id, string name, string author, int year
    , decimal price) : base(id, name)
    {
        Author = author;
        Year = year;
        Price = price;
    }

    protected override string Type => "Книга";
    protected override string Information => $"Автор:\t{Author}\n\tГод:\t{Year}\n\tЦена:\t{Price}";
}