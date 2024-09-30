using Store.Exercises;

internal class Exercise35 : ExerciseBase, IExercise
{
    public string[] Code => ["int i = (int)+(char)-(int)+(long)-1;"];    
    public string[] TestCode => ["Console.WriteLine(i);"];
    public override string[] Variants => ["-9","-1","0","1","9"];
    public override string Exercise()
    {        
        int i = (int)+(char)-(int)+(long)-1;
        return i.ToString();
    }
}
