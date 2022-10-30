using Product;
using Showcase;

var data1 = new List<IProduct>
{
    new Book(1000, "ВОЙНА И МИРЪ I", "Л.Н. Толстой", 1863, 1000000),
    new Book(2000, "ВОЙНА И МИРЪ II", "Л.Н. Толстой", 1865, 200000),
    new Book(3000, "ВОЙНА И МИРЪ III", "Л.Н. Толстой", 1867, 300000),
    new Book(4000, "ВОЙНА И МИРЪ IV", "Л.Н. Толстой", 1869, 400000)
};

var data2 = new List<Comic>
{
    new (5555, "Хранители", "С. Маккоауд", 2008, 2071),
    new (6666, "Понимание комикса", "А. Шпигельман", 1990, 860),
    new (7777, "Ходячие мертвецы", "Р. Кирман", 2003, 2257)
};

static void Test<T>(IAssortment<T> a, List<T> list) where T : class, IProduct
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

Assortment2<IProduct> a1 = 4;
Assortment2<IProduct> a2 = 4;
Assortment3<IProduct> a3 = 4;
Assortment3<Comic> a4 = 3;

//Test(a1, data1);
//Test(a2, data1);
//Test(a3, data1);
Test(a4, data2);
