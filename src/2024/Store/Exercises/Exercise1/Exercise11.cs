namespace Store.Exercises.Exercise1;

internal class Exercise11 : ExerciseBase, IExercise
{   
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(3);
    
    public string[] Code => ["public static IEnumerable<int> GetInts()",
                             "{",
                             "    yield return 1;",
                             "    yield return 2;",
                             "    Console.WriteLine(3);",
                             "}",
                             " ",
                             "Console.WriteLine(GetInts().Last());"
                             ];

    public string[] Variants => ["2", "3", "2 3", "3 2"];

    public override string Exercise()
    {
        return "3 2";
    }
}