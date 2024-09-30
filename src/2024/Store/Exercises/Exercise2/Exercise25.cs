namespace Store.Exercises.Exercise2;

internal class Exercise25: Exercise21, IExercise
{
    public override string[] TestCode => ["Console.WriteLine(1 << 0);"];
    public override string Exercise() => (1 << 0).ToString();
}
