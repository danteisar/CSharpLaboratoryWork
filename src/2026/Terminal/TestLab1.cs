using DataMatrixGenerator;
using Lab1.Codes;

namespace Terminal;

internal static class TestLab1
{
    public static void Test()
    {
        Console.Clear();
        Console.WriteLine("ЛАБОРАТОРНАЯ РАБОТА №1");

        ProductCode.OutputMode = OutputMode.Full;

        ProductCode code = new();

        while (true)
        {
            Console.WriteLine("Введите текст:");
            var text = Console.ReadLine();
            if (string.IsNullOrEmpty(text)) break;
            code.Text = text;
            Console.WriteLine(code);
        }

        Console.ReadKey();
    }
}
