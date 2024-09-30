namespace Store.Exercises.Exercise5;

internal class Exercise54: Exercise51, IExercise
{
    public override string[] TestCode => ["var array = [5, 3, 2, 1, 4];",
                                          "Do(array, array.Length - 1, 1)",                            
                                          "Console.WriteLine(string.Join(\" \", array.Select(x => x.ToString()).ToArray());"];

    public override string Exercise()
    {
        Ints[] i = [5, 3, 2, 1, 4];
        Do(i, i.Length - 1, 1);
        return Example(i);
    }
}
