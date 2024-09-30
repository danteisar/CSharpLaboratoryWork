namespace Store.Exercises.ExerciseSpecial;

internal class Exercise01 : ExerciseBase, IExercise
{
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(3);

    public virtual string[] Code => ["public int Do()",
                                     "{",
                                     "    try",
                                     "    {",
                                     "        return 1;",
                                     "    }",
                                     "    finally",
                                     "    {",
                                     "        return 2;",
                                     "    }",
                                     "}"];

    public virtual string[] TestCode => ["Console.WriteLine(Do());"];

    public override string[] Variants => ["1", "2", " ", "1 2"];

    public int Do()
    {
        try
        {
            return 1;
        }
        finally
        {
            //return 2;
        }
    }
}