namespace Store.Exercises;

internal class Exercise2 : ExerciseBase, IExercise
{
    public int Number => 2;

    public string[] Code => ["public static IEnumerable<int> GetInts()",
                             "{",
                             "    yield return 1;",
                             "    yield return 2;",
                             "    Console.WriteLine(3);",
                             "}",
                             " ",
                             "Console.WriteLine(GetInts().Last());"
                             ];

    public string[] Variants => ["1", "2", "2 3", "3", "1 2 3", "3 2", "3 2 1"];

    public static IEnumerable<int> GetInts()
    {
        yield return 1;
        yield return 2;
        Console.WriteLine(3);
    }

    public override string Exercise() => "3 2";
}