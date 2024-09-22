namespace Store.Exercises;

internal class Exercise2 : ExerciseBase, IExercise
{
    public int Number => 2;
    
    public string[] Code => ["Console.WriteLine(~0 << 2);"];

    public string[] Variants => ["-100", "-4", "-1", "0", "1", "4", "100"];

    public override string Exercise() => (~0 << 2).ToString();
}