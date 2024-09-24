namespace Store.Exercises.Exercise1;

internal class Exercise13: ExerciseBase, IExercise
{   
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(3);
    
    public string[] Code => ["private static string _tmp = string.Empty;",
                             "public static IEnumerable<int> GetInts()",
                             "{",
                             "    yield return \"1\";",
                             "    yield return \"2\";",
                             "    _tmp += \"3\";",
                             "}",
                             " ",                             
                             "_tmp += GetInts().Last();",
                             "Console.WriteLine(_tmp);"
                             ];

    public string[] Variants => ["2", "3", "2 3", "3 2"];

    private static string _tmp = string.Empty;
    public static IEnumerable<string> GetInts()
    {
        yield return "1";
        yield return "2";
        _tmp += "3";
    }

    public override string Exercise()
    {
        _tmp = string.Empty;
        _tmp += GetInts().Last();
        return _tmp;
    }
}
