namespace Store.Exercises.Exercise2;

internal class Exercise24: Exercise21, IExercise
{
    public override string[] TestCode => ["Console.WriteLine(~-1 >> 1);"];
    public override string Exercise() => (~-1 >> 1).ToString();
}
