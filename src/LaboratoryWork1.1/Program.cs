using System;

namespace LaboratoryWork1._1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Product.ByBar = true;

            Workshop workshop = 5;

            Product book1 = new Book("Война и мир", 1000000, 1, "Лев Толстой", 308);
            Product book2 = new Book("Война и мир", 2, 2, "Лев Толстой", 322);
            Product book3 = new Book("Книга 3    ", 3, 2, "Автор 2", 33);
            Product book4 = new Book("Война и мир", 45, 3, "Лев Толстой", 400);

            workshop[0] = book1;
            workshop[1] = book2;
            workshop[2] = book3;
            workshop[3] = book4;

            Console.WriteLine(workshop);

            Console.ReadKey();
        }
    }
}
