
//ShowBarcode("");
//ShowBarcode("123456x10x2020");
//ShowBarcode("123456x10x2020", true);
//ShowBarcode("For c# labs!!!");

using Barcode;
using Product;
using Showcase;

static void ShowBarcode(string text, bool optimize = false)
{
    if (optimize) 
        Console.WriteLine("ОПТИМИЗИРОВАННЫЙ ШТРИХ-КОД");
    var t = new Barcode.Barcode(text, optimize);
    Console.WriteLine(t);
    Console.WriteLine();
}

IProduct p1 = new Book(100, "ВОЙНА И МИРЪ I", "Л.Н. Толстой", 1863, 1000000);
IProduct p2 = new Book(200, "ВОЙНА И МИРЪ II", "Л.Н. Толстой", 1865, 200000);
IProduct p3 = new Book(300, "ВОЙНА И МИРЪ III", "Л.Н. Толстой", 1867, 300000);
IProduct p4 = new Book(300, "ВОЙНА И МИРЪ IV", "Л.Н. Толстой", 1869, 400000);

Assortment2<IProduct> a1 = 4;

a1.Push(p4);
a1.Push(p2);
a1.Push(p3);
a1.Push(p1);

a1.OrderById();

IAssortment<IProduct> a2 = (Assortment2<IProduct>)3;

a2.Push(p2);
a2.Push(p1);
a2.Push(p4);
a2.Push(p3);

a2.OrderByName();

Console.WriteLine(a1);
Console.WriteLine(a2);
