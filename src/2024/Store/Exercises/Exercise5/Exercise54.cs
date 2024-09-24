using System;

namespace Store.Exercises.Exercise5;

internal class Exercise54: Exercise51, IExercise
{
    public override string[] Code => ["public static void Do(IComparable[] objects, int left, int right)",
                             "{",
                             "    int i = left, j = right;",
                             "    IComparable pivot = objects[(left + right) / 2];",
                             "    while (i < j) ",
                             "    {",
                             "        while (objects[i].CompareTo(pivot) < 0) i++;",
                             "        while (objects[j].CompareTo(pivot) > 0) j--;",
                             "        if (i <= j) (objects[i], objects[j]) = (objects[j], objects[i]);",
                             "        i++; j--;",
                             "        if (left < j) Do(objects, left, j);",
                             "        if (i < right) Do(objects, i, right);",
                             "    }",                           
                             "}",
                             " ",
                             "var array = [5, 3, 2, 1, 4];",
                             "Do(array, i.Length - 1, 1)",                            
                             "Console.WriteLine(string.Join(\" \", array.Select(x => x.ToString()).ToArray());"];

    public override string Exercise()
    {
        Ints[] i = [5, 3, 2, 1, 4];
        Do(i, i.Length - 1, 1);
        return Example(i);
    }
}
