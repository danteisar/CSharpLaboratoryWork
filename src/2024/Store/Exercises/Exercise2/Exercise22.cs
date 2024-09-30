namespace Store.Exercises.Exercise2;

internal class Exercise22: Exercise21, IExercise
{
    public override string[] TestCode => ["Console.WriteLine(~1 << 2);"];
    public override string Exercise() => (~1 << 2).ToString();
}
