namespace Store.Exercises.Exercise1;

internal class Exercise14: Exercise13
{   
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(3);
    
    public override string[] Code => ["private static string _tmp = string.Empty;",
                                      "public static IEnumerable<int> GetInts()",
                                      "{",
                                      "    yield return \"1\";",
                                      "    _tmp += \"2\";",
                                      "    yield return \"3\";",                             
                                      "}"];
    
    private static string _tmp = string.Empty;

    public static IEnumerable<string> GetInts2()
    {
        yield return "1";
         _tmp = "2";
        yield return "3";       
    }

    public override string Exercise()
    {
        _tmp = string.Empty;
        _tmp += GetInts2().Last();
        return _tmp;
    }
}
