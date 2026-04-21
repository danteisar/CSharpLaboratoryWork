using Lab3.Abstractions;

namespace Terminal;

public static class Extensions
{
    public static void ShowAndWait<T>(this IAssortment<T> assortment, string title)
    {
        Console.WriteLine(title);
        Console.WriteLine(assortment);
        Console.ReadKey();
        Console.Clear();
    }
}
