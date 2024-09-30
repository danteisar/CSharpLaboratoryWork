namespace Store.Exercises.Exercise2;

internal class Exercise27: Exercise21, IExercise
{
    public override string[] TestCode => ["Console.WriteLine(1 << 3);"];
    public override string Exercise() => (1 << 3).ToString();
}
