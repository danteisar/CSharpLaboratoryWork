namespace Store.Exercises.Exercise1;

internal class Exercise12: ExerciseBase, IExercise
{
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(3);
    
    public string[] Code => ["public static IEnumerable<int> GetInts()",
                             "{",
                             "    yield return 1;",
                             "    Console.WriteLine(2);",
                             "    yield return 3;",                            
                             "}",
                             " ",
                             "Console.WriteLine(GetInts().Last());"
                             ];

    public string[] Variants => ["2", "3", "2 3", "3 2"];
    public override string Exercise()
    {
        return "2 3";
    }
}