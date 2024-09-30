using Store.Exercises;

internal class Exercise37 : Exercise36, IExercise
{
    public override string[] Code => ["int[] dataArray = [0, 1, 2];",
                                      "int result = 0;",
                                      "var selectedData = dataArray.Select(",
                                      "    x =>",
                                      "    {",
                                      "        result += x;",
                                      "        return x;",
                                      "    }).ToArray();"];    
    public override string Exercise()
    {      
        int result = 0;
        int[] dataArray = [0, 1, 2];
        var selectedData = dataArray.Select(x =>
                                            {
                                                result += x;
                                                return x;
                                            }).ToArray();
        return result.ToString();
    }
}
