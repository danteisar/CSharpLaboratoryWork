namespace Store.Exercises.Exercise5;

internal class Exercise52 : Exercise51, IExercise
{
    public override string[] TestCode => ["var array = [5, 3, 2, 1, 4];",
                                          "Do(array, 0, array.Length - 2)",                            
                                          "Console.WriteLine(string.Join(\" \", array.Select(x => x.ToString()).ToArray());"];

    public override string Exercise()
    {
        Ints[] i = [5, 3, 2, 1, 4];
        Do(i, 0, i.Length - 2);
        return Example(i);
    }
}
