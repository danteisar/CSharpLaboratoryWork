using Barcode.Lab1;
using Product.Lab2;
using Product.Lab3;
using Product.Lab4;
using Showcase.Lab2;
using Showcase.Lab3;
using Showcase.Lab4;

#region Lab 1 + 2

static void TestLab1()
{
    Console.WriteLine("Введите текст: ");
    Barcode1 barcode = Console.ReadLine();
    Console.WriteLine("ШТРИХ КОД: ");
    Console.WriteLine(barcode);
}


static void TestLab2()
{
    var lab1Data = new List<Thing>
    {
        new Book2(3000, "ВОЙНА И МИРЪ III", "Л.Н. Толстой", 1867, 300000),
        new Book2(1000, "ВОЙНА И МИРЪ I", "Л.Н. Толстой", 1863, 1000000),
        new Book2(2000, "ВОЙНА И МИРЪ II", "Л.Н. Толстой", 1865, 200000),
        new Book2(4000, "ВОЙНА И МИРЪ IV", "Л.Н. Толстой", 1869, 400000)
    };

    Assortment2 a1 = 4;

    Test(a1, lab1Data); 
}

static void Test(Assortment2 a, List<Thing> list)
{
    Console.WriteLine("".PadLeft(80, '='));
    foreach (var product in list)
    {
        a.Push(product);
    }

    a.OrderByName();
    a.Id++;

    Console.WriteLine(a);
}

#endregion

#region lab 3

static void TestLab3()
{
    Console.Clear();

    var lab3Data = new List<IThing3>
    {
        new Book3(3000, "ВОЙНА И МИРЪ III", "Л.Н. Толстой", 1867, 300000),
        new Book3(1000, "ВОЙНА И МИРЪ I", "Л.Н. Толстой", 1863, 1000000),
        new Book3(2000, "ВОЙНА И МИРЪ II", "Л.Н. Толстой", 1865, 200000),
        new Book3(4000, "ВОЙНА И МИРЪ IV", "Л.Н. Толстой", 1869, 400000)
    };
    var lab3Data2 = new List<Comic3>
    {
        new (5555, "Хранители", "С. Маккоауд", 2008, 2071),
        new (6666, "Понимание комикса", "А. Шпигельман", 1990, 860),
        new (7777, "Ходячие мертвецы", "Р. Кирман", 2003, 2257)
    };

    Assortment3<IThing3> a1 = 4;
    Assortment3<Comic3> a2 = 3;

    Test3(a1, lab3Data);
    Test3(a1, lab3Data2.Select(x => x as IThing3).ToList());
    Test3(a2, lab3Data2);
}

static void Test3<T>(IAssortment3<T> a, List<T> list) where T : class, IThing3
{
    Console.WriteLine("".PadLeft(80, '='));
    foreach (var product in list)
    {
        a.Push(product);
    }

    a.OrderByName();
    a.Id++;

    Console.WriteLine(a);
}

#endregion

#region lab 4

static void TestLab4()
{
    var lab4Data = new List<IThing4>
    {
        new Book4(3000, "ВОЙНА И МИРЪ III", "Л.Н. Толстой", 1867, 300000),
        new Book4(1000, "ВОЙНА И МИРЪ I", "Л.Н. Толстой", 1863, 1000000),
        new Book4(2000, "ВОЙНА И МИРЪ II", "Л.Н. Толстой", 1865, 200000),
        new Book4(4000, "ВОЙНА И МИРЪ IV", "Л.Н. Толстой", 1869, 400000)
    };
    var lab4Data2 = new List<Comic4>
    {
        new (5555, "Хранители", "С. Маккоауд", 2008, 2071),
        new (6666, "Понимание комикса", "А. Шпигельман", 1990, 860),
        new (7777, "Ходячие мертвецы", "Р. Кирман", 2003, 2257)
    };

    Assortment4<IThing4> a1 = 4;
    Assortment4<Comic4> a2 = 3;

    Test4(a1, lab4Data);
    Test4(a1, lab4Data2.Select(x => x as IThing4).ToList());
    Test4(a2, lab4Data2);
}

static void Test4<T>(IAssortment4<T> a, List<T> list) where T : class, IThing4
{
    Console.WriteLine("".PadLeft(80, '='));
    foreach (var product in list)
    {
        a.Push(product);
    }

    a.OrderByName();
    a.Id++;

    Console.WriteLine(a);
}

#endregion

while (true)
{
    Console.Clear();
    Console.WriteLine("Укажите номер лабораторной работы: 1, 2, 3 или 4");

    var t = Console.ReadKey();
    Console.WriteLine();

    switch (t.KeyChar)
    {
        case '1': TestLab1(); break;
        case '2': TestLab2(); break;
        case '3': TestLab3(); break;
        case '4': TestLab4(); break;
    }
    Console.WriteLine("Нажмите чтобы продолжить...");
    Console.ReadKey();
}