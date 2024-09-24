namespace Store.Exercises.Exercise1;

internal class Exercise12: ExerciseBase, IExercise
{
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(3);
    
    public string[] Code => ["public static IEnumerable<int> GetInts()",
                             "{",
                             "    yield return 1;",
                             "    Console.WriteLine(2);",
                             "    yield return 3;",
                             "    Console.WriteLine(4);",
                             "}",
                             " ",
                             "Console.WriteLine(GetInts().Last());"
                             ];

    public string[] Variants => ["3", "2 4 3", "2 3", "4 3 2", "4 3"];
 
    private static string _tmp = string.Empty;
    public static IEnumerable<string> GetInts()
    {
        yield return "1";
        _tmp += "2";
        yield return "3";
        _tmp += "4";
    }

    public override string Exercise()
    {
        _tmp = string.Empty;
        _tmp += GetInts().Last();
        return "2 4 3";
    }
}