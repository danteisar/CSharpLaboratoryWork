namespace Store.Exercises.Exercise2;

internal class Exercise21 : ExerciseBase, IExercise
{
    public string[] Code => [];

    public virtual string[] TestCode => ["Console.WriteLine(~0 << 2);"];

    public override string[] Variants => ["-8", "-4", "-1", "0", "1", "4", "8"];

    public override string Exercise() => (~0 << 2).ToString();
}