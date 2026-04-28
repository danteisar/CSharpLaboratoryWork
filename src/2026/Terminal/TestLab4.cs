using DataMatrixGenerator;
using Lab1.Codes;
using Lab4.Abstractions;
using Lab4.Products;
using Lab4.Storage;

namespace Terminal;

internal static class TestLab4
{
    public static void Test()
    {
        Console.Clear();
        Console.WriteLine("ЛАБОРАТОРНАЯ РАБОТА №4");

        ProductCode.OutputMode = OutputMode.Text;

        NewBook book1 = new(100, "ВОЙНА И МИРЪ I", "Л.Н. Толстой", 1863, 100);
        NewBook book2 = new(200, "ВОЙНА И МИРЪ III", "Л.Н. Толстой", 1867, 200);
        using NewPackage book3 = new(book2);

        using NewAssortment<INewProduct> assortment1 = 2;

        assortment1[0] = book1;
        assortment1.Push(book3);
        assortment1.ShowAndWait("Этап 1");

        book1.Id = 300;
        book1.Price = 300;
        book2.Id = 400;
        book2.Price = 400;
        assortment1.ShowAndWait("Этап 2");

        assortment1.Id = 100;
        assortment1.ShowAndWait("Этап 3");
    }
}
