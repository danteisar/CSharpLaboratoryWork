namespace Store.Exercises.ExerciseSpecial;

internal class Exercise02 : ExerciseBase, IExercise
{
    public virtual string[] Code => ["var s = ((string)null + null);",
                                     "var isNull = (s == null);"];
    public virtual string[] TestCode => ["Console.WriteLine(isNull);"];

    public override string[] Variants => ["True", " ", "False"];

    public override string Exercise()
    {
        var s = ((string)null + null);
        var isNull = (s == null);
        return isNull.ToString();
    }
}