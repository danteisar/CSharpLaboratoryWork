namespace Store.Exercises.ExerciseSpecial;

internal class Exercise06 : ExerciseBase, IExercise
{
    public string[] Code => ["public enum Sample",
                             "{",
                             "    V1, V2, V3, V4",
                             "}"];
    public string[] TestCode => ["var str = \"V1, V2, V3\";",
                                 "if (Enum.TryParse(str, out Sample result))", 
                                 "    Console.WriteLine(result);", 
                                 "else", 
                                 "    Console.WriteLine(\"Error\");"];
    public override string[] Variants => ["V1", "V2", "V3", "V4", "Error"];
    public override string Exercise()
    {
        var str = "V1, V2, V3";
        if (Enum.TryParse(str, out Sample result))
            return result.ToString();            
        return "Error";
    }

    public enum Sample
    {
        V1, V2, V3, V4
    }
}