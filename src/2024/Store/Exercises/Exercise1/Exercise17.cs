namespace Store.Exercises.Exercise1;

internal class Exercise17 : ExerciseBase, IExercise
{
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(3);
    
    public virtual string[] Code => ["public static bool? IsOddNumber(int i)",                                    
                                     "{", 
                                     "    return i % 2 == 1;",
                                     "}"];

    public virtual string[] TestCode => ["Console.WriteLine(IsOddNumber(-5));"];

    public override string[] Variants => ["True", " ", "False", "null"];    

    public static bool? IsOddNumber(int i)
    {
        return i % 2 == 1;
    }

    public override string Exercise()
    {
        return IsOddNumber(-5).ToString();
    }
}
