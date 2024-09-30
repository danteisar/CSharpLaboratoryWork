namespace Store.Exercises.ExerciseSpecial;

internal class Exercise05 : ExerciseBase, IExercise
{
    public string[] Code => ["public static IEnumerable<int> Do()",
                             "{",
                             "    yield return 1;",
                             "    throw new Exception();",
                             "    yield return 2;",
                             "}"];
    public string[] TestCode => ["var raw = Do();", 
                                 "var tmp = raw.Take(2).Select(x => x * 2);", 
                                 "Console.WriteLine(tmp.FirstOrDefault());"];
    public override string[] Variants => ["1", " ", "2", "4"];
    public override string Exercise()
    {
        var raw = Do();
        var tmp = raw.Take(2).Select(x => x * 1);
        return tmp.FirstOrDefault().ToString();
    }

    public static IEnumerable<int> Do()
    {
        yield return 1;
        throw new Exception();
        //yield return 2;
    }
}