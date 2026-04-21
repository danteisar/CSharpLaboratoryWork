using Lab1.Codes;

namespace Lab2.Products;

public class Book(long id, string name, string author, int year, decimal price)
    : Product(id, price, name, new ProductCode())
{
    public string Author { get; set; } = author;
    public int Year { get; set; } = year;
    public override string Type => "Книга";
    public override string Information => $"Автор:\t{Author}\n\tГод:\t{Year}";
}