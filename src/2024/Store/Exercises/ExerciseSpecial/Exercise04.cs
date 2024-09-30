namespace Store.Exercises.ExerciseSpecial;

internal class Exercise04 : ExerciseBase, IExercise
{
    public string[] Code => [];
    public string[] TestCode => ["Console.WriteLine(1 + 2 + \"A\");", "Console.WriteLine('A' + 1 + 2);"];
    public override string[] Variants => ["12A A12", "3A A3", "3A A12", "68 68", "68 A12", "3A 68", "68 68"];
    public override string Exercise()
    {
        return $"{1 + 2 + "A"} {'A' + 1 + 2}";
    }
}