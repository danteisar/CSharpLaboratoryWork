using System;
using System.Collections.Generic;
using Currency;
using static System.Console;

namespace LaboratoryWork3
{
    public class Program
    {
        private static void State(int i)
        {
            WriteLine(Environment.NewLine);
            WriteLine($"Этап {i}");
        }

        private static void Main()
        {
            // -----------------------------------------------------------------------------------------------------------
            Rub usdCost = 100;
            IProduct.Course = usdCost;

            IProduct book1 = new Book("Война и мир", (Rub)1534, "Лев Толстой", 308, 1);
            IBook book2 = new Book("Война и мир", (Rub)1534, "Лев Толстой", 322, 2);
            Product book3 = new Book("Книга 3", (Usd)2, "Автор 2", 33, 2);
            var book4 = new Book("Война и мир", (Rub)1635, "Лев Толстой", 400, 3);
            var comix = new Comix("Комикс", 200, "DC", 200);

            var list = new List<IProduct> { book1, book2, null, null, null };
            Workshop<IProduct> workshop1 = list;
            Workshop<IBook> workshop2 = 4;

            IWorkshop<IProduct> workshop = workshop1;
            IWorkshop<IBook> workshop3 = workshop2;

            workshop2.Put(comix);
            WriteLine(workshop);
            WriteLine(workshop2);
            // ----------------------------------------------------------------------------------------------------------- I

            State(1);
            WriteLine($"\nТекущий курс: {workshop1.Course}");
            WriteLine(Environment.NewLine);
            // ----------------------------------------------------------------------------------------------------------- II

            State(2);
            var product = workshop1.Get();
            WriteLine($"\nВзят товар:\n{product}");
            product.Usd = 100;
            WriteLine($"\nОбновлена цена тована:\n{product}");
            // ----------------------------------------------------------------------------------------------------------- III

            State(3);
            Usd s = 1m / 40;
            WriteLine(workshop.Course);
            WriteLine("Смена курса валют");
            workshop.Course =s;
            WriteLine(workshop.Course);
            workshop.Put(book3, 3);
            WriteLine(Environment.NewLine);
            WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- IV

            State(4);
            product = workshop.Put(product, 2);
            if (product != null)
                WriteLine("\nМесто занято!");
            WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- V

            State(5);
            workshop.Replace(1, 2);
            WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- VI

            State(6);
            product = workshop.Get(2);
            WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- VII

            State(7);
            product = workshop.Replace(product, 0);
            if (product != null) WriteLine($"\nВозвращен товар:\n{product}");
            WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- VIII

            State(8);
            foreach (var p in workshop.SearchByName("Война и мир"))
            {
                WriteLine(p);
            }
            // ----------------------------------------------------------------------------------------------------------- IX

            State(9);
            workshop3.Put(book4, 4);
            WriteLine(workshop);
            WriteLine(Environment.NewLine);
            WriteLine("Отсортированный по цене:");
            workshop.SortedByCost();
            WriteLine(workshop);


            // ----------------------------------------------------------------------------------------------------------- X
            ReadKey();
        }
    }
}
