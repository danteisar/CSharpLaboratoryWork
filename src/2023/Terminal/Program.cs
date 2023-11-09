while (true)
{
    Console.Clear();
    Console.WriteLine("Укажите номер лабораторной работы: 1, 2, 3 или 4");

    var t = Console.ReadKey();
    Console.WriteLine();

    switch (t.KeyChar)
    {
        case '1': TestLab1(); break;
        //case '2': TestLab2(); break;
        //case '3': TestLab3(); break;
        //case '4': TestLab4(); break;
    }
    Console.WriteLine("Нажмите чтобы продолжить...");
    Console.ReadKey();
}

static void TestLab1()
{
    Barcode1.Type = BarcodeType.Full;
    Console.WriteLine("Введите текст: ");
    Barcode1 barcode = Console.ReadLine();
    Console.WriteLine("ШТРИХ КОД: ");
    Console.WriteLine(barcode);
}