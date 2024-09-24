namespace Store.Exercises.Exercise2;

internal class Exercise24: ExerciseBase, IExercise
{
    public string[] Code => ["Console.WriteLine(~-1 >> 1);"];

    public string[] Variants => ["-8", "-4", "-1", "0", "1", "4", "8"];

    public override string Exercise() => (~-1 >> 1).ToString();
}
