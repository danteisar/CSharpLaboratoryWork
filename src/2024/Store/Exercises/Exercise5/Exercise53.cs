namespace Store.Exercises.Exercise5;

internal class Exercise53: Exercise51, IExercise
{
    public override string[] TestCode => ["var array = [5, 3, 2, 1, 4];",
                                          "Do(array, 1, array.Length - 1)",                            
                                          "Console.WriteLine(string.Join(\" \", array.Select(x => x.ToString()).ToArray());"];

    public override string Exercise()
    {
        Ints[] i = [5, 3, 2, 1, 4];
        Do(i, 1, i.Length - 1);
        return Example(i);
    }
}
