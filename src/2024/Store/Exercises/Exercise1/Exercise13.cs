namespace Store.Exercises.Exercise1;

internal class Exercise13: Exercise11, IExercise
{   
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(3);
    
    public override string[] Code => ["private static string _tmp = string.Empty;",
                                      "public static IEnumerable<int> GetInts()",
                                      "{",
                                      "    yield return \"1\";",
                                      "    yield return \"2\";",
                                      "    _tmp += \"3\";",
                                      "}"];
    
    public override string[] TestCode => ["_tmp += GetInts().Last();",
                                          "Console.WriteLine(_tmp);"];
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
