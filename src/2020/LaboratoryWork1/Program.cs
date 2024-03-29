﻿using System;
using Currency;

namespace LaboratoryWork1
{
    public class Program
    {
        private static void State(int i)
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Этап {i}");
        }

        private static void Main()
        {
            // -----------------------------------------------------------------------------------------------------------
            Rub usdCost = 100;
            Product.CurrentCourse = usdCost;
            Product book1 = new Book("Война и мир", (Rub)1534, "Лев Толстой", 308, 1);
            Product book2 = new Book("Война и мир", (Rub)1534, "Лев Толстой", 322, 2);
            Product book3 = new Book("Книга 3", (Usd)2, "Автор 2", 33, 2);
            Product book4 = new Book("Война и мир", (Rub)1635, "Лев Толстой", 400, 3);
            Workshop workshop = 5;
            workshop[0] = book1;
            workshop[1] = book2;
            Console.WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- I
           
            State(1);
            Console.WriteLine($"\nТекущий курс: {workshop.Course()}");
            Console.WriteLine(Environment.NewLine);
            // ----------------------------------------------------------------------------------------------------------- II
            
            State(2);
            var product = workshop.Get(0);
            product.Usd = 100;
            Console.WriteLine($"\nВзят товар:\n{product}");
            // ----------------------------------------------------------------------------------------------------------- III
            
            State(3);
            Usd s = 1m / 40;
            Console.WriteLine(workshop.Course());
            Console.WriteLine("Смена курса валют");
            workshop.Course(s);
            Console.WriteLine(workshop.Course());
            workshop.Put(book3, 3);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- IV
            
            State(4);
            product = workshop.Put(product, 2);
            if (product != null)
                Console.WriteLine("\nМесто занято!");
            Console.WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- V
            
            State(5);
            workshop.Replace(1, 2);
            Console.WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- VI
            
            State(6);
            product = workshop.Get(2);
            Console.WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- VII
            
            State(7);
            product = workshop.Replace(product, 0);
            if (product != null) Console.WriteLine($"\nВозвращен товар:\n{product}");
            Console.WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- VIII
            
            State(8);
            foreach (var p in workshop.SearchByName("Война и мир"))
            {
               Console.WriteLine(p);
            }
            // ----------------------------------------------------------------------------------------------------------- IX
            
            State(9);
            workshop.Put(book4, 4);
            Console.WriteLine(workshop);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Отсортированный по цене:");
            workshop.SortedByCost();
            Console.WriteLine(workshop);
            // ----------------------------------------------------------------------------------------------------------- X
            Console.ReadKey();
        }
    }
}
