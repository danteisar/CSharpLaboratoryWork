namespace Store.Exercises.Exercise4;

internal class Exercise46: Exercise45, IExercise
{
    public override string[] TestCode => ["Console.WriteLine(new Amplifier().ModelName);"];

    public override string Exercise()
    {    
        return new Amplifier().ModelName;
    }    
}