
ShowBarcode("");
ShowBarcode("123456x10x2020");
ShowBarcode("123456x10x2020", true);
ShowBarcode("Hello world!");

static void ShowBarcode(string text, bool optimize = false)
{
    if (optimize) 
        Console.WriteLine("ОПТИМИЗИРОВАННЫЙ ШТРИХ-КОД");
    var t = new Barcode.Barcode(text, optimize);
    Console.WriteLine(t);
    Console.WriteLine();
}
