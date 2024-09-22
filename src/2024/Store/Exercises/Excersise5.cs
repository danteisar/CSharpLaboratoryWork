﻿namespace Store.Exercises;

internal class Exercise5 : ExerciseBase, IExercise
{
    public int Number => 5;

    public string[] Code => ["public static void Do(IComparable[] objects, int left, int right)",
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
                             "Do(array, 0, i.Length - 1)"];

    public string[] Variants => ["5 3 2 1 4", "1 2 3 5 4", "1 2 3 4 5", "5 4 3 2 1"];

    public class Ints(int i): IComparable
    {
        public int I { get; } = i;
        public static implicit operator Ints(int i) => new(i);
        public override string ToString() => I.ToString();

        public int CompareTo(object obj)
        {
            if (obj is Ints i)
            {
                return I.CompareTo(i.I);
            }
            throw new NotSupportedException();
        }
    }

    public static void Do(IComparable[] objects, int left, int right)
    {
        int i = left, j = right;
        IComparable pivot = objects[(left + right) / 2];
        while (i < j) 
        {
            while (objects[i].CompareTo(pivot) < 0) i++;
            while (objects[j].CompareTo(pivot) > 0) j--;
            if (i <= j) (objects[i], objects[j]) = (objects[j], objects[i]);
            i++; j--;
            if (left < j) Do(objects, left, j);
            if (i < right) Do(objects, i, right);
        } 
    }

    public override string Exercise()
    {
        Ints[] i = [5, 3, 2, 1, 4];
        Do(i, 0, i.Length - 1);
        var tmp = string.Empty;
        foreach (var item in i)
        {
            tmp += item + " ";
        }
        return tmp.TrimEnd();
    }
}