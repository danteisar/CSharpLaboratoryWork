using Lab1.Codes;
using Lab2.Abstractions;
using Lab2.Products;
using Lab3.Abstractions;
using Lab3.Storage;

namespace Terminal;

internal static class TestLab3
{
    public static void Test()
    {
        Console.Clear();
        Console.WriteLine("ЛАБОРАТОРНАЯ РАБОТА №3");

        ProductCode.OutputMode = OutputMode.Text;

        IProduct book1 = new Book(100, "ВОЙНА И МИРЪ I", "Л.Н. Толстой", 1863, 100);
        var book2 = new Package(new Book(200, "ВОЙНА И МИРЪ III", "Л.Н. Толстой", 1867, 200));

        IAssortment<IProduct> assortment1 = (Assortment<IProduct>)2;
        IAssortment<Package> assortment2 = (Assortment<Package>)1;

        assortment1[0] = book1;
        assortment1.Push(book2);
        assortment1.OrderById();
        assortment1.ShowAndWait("Этап 1");

        assortment1.Id = 100;
        assortment1.ShowAndWait("Этап 2");

        var package = assortment1[assortment1.Find(book2.Id)];
        assortment2[0] = (Package)package;
        assortment1.ShowAndWait("Этап 3");
        assortment2.ShowAndWait("Этап 4");
    }
}
