namespace Store.Exercises.Exercise2;

internal class Exercise25: ExerciseBase, IExercise
{
    public string[] Code => ["Console.WriteLine(1 << 0);"];

    public string[] Variants => ["-8", "-4", "-1", "0", "1", "4", "8"];

    public override string Exercise() => (1 << 0).ToString();
}
