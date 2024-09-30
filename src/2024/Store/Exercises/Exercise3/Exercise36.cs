using Store.Exercises;

internal class Exercise36 : ExerciseBase, IExercise
{
    public virtual string[] Code => ["int[] dataArray = [0, 1, 2];",
                                     "int result = 0;",
                                     "var selectedData = dataArray.Select(",
                                     "    x =>",
                                     "    {",
                                     "        result += x;",
                                     "        return x;",
                                     "    });"];    
    public string[] TestCode => ["Console.WriteLine(result);"];
    public override string[] Variants => ["0","1","2","3"];
   
    public override string Exercise()
    {      
        int result = 0;
        int[] dataArray = [0, 1, 2];
        var selectedData = dataArray.Select(x =>
                                            {
                                                result += x;
                                                return x;
                                            });
        return result.ToString();
    }
}
