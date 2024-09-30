namespace Store.Exercises.Exercise2;

internal class Exercise23: Exercise21, IExercise
{
    public override string[] TestCode => ["Console.WriteLine(~0 << 0);"];

    public override string Exercise() => (~0 << 0).ToString();
}
