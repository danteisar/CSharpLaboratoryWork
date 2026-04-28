using DataMatrixGenerator;
using Lab1.Codes;
using Lab2.Abstractions;
using Lab2.Products;

namespace Terminal;

internal static class TestLab2
{
    public static void Test()
    {
        Console.Clear();
        Console.WriteLine("ЛАБОРАТОРНАЯ РАБОТА №2");

        ProductCode.OutputMode = OutputMode.Full;

        IProduct book1 = new Book(1, "ВОЙНА И МИРЪ I", "Л.Н. Толстой", 1863, 100);
        IProduct book2 = new Package(new Book(2, "ВОЙНА И МИРЪ III", "Л.Н. Толстой", 1867, 200));

        IProduct[] products = [book1, book2];

        foreach (var prdct in products)
        {
            Console.WriteLine(prdct);
            prdct.Id++;
            prdct.Price += 1;
            Console.WriteLine(prdct);
        }

        Console.ReadKey();
    }
}
