namespace Store.Exercises.Exercise5;

internal class Exercise55 : Exercise51, IExercise
{
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(10);
    
    public override string[] Code => ["public static void Do(IComparable[] objects, int left, int right)",
                                      "{",
                                      "    int i = left, j = right;",
                                      "    IComparable pivot = objects[(left + right) / 2];",
                                      "    while (i <= j)",
                                      "    {",
                                      "        while (objects[i].CompareTo(pivot) > 0) i++;",
                                      "        while (objects[j].CompareTo(pivot) < 0) j--;",
                                      "        if (i <= j) { (objects[i], objects[j]) = (objects[j], objects[i]); i++; j--; }",
                                      "        if (left < j) Do(objects, left, j);",
                                      "        if (i < right) Do(objects, i, right);",
                                      "    }",                           
                                      "}"];
                              
    public override string[] TestCode => ["var array = [5, 3, 2, 1, 4];",
                                          "Do(array, 0, array.Length - 1)",                            
                                          "Console.WriteLine(string.Join(\" \", array.Select(x => x.ToString()).ToArray());"];

    public static void DoReverse(IComparable[] objects, int left, int right)
    {
        int i = left, j = right;
        IComparable pivot = objects[left];
        while (i < j) 
        {
            while (objects[i].CompareTo(pivot) > 0) i++;
            while (objects[j].CompareTo(pivot) < 0) j--;
            if (i <= j) 
            {
                (objects[i], objects[j]) = (objects[j], objects[i]); 
                i++; 
                j--; 
            }           
            if (left < j) DoReverse(objects, left, j);
            if (i < right) DoReverse(objects, i, right);
        } 
    }

    public override string Exercise()
    {
        Ints[] i = [5, 3, 2, 1, 4];
        DoReverse(i, 0, i.Length - 1);
        return Example(i);
    }
}