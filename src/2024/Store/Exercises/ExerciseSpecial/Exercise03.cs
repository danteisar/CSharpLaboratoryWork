namespace Store.Exercises.ExerciseSpecial;

internal class Exercise03 : ExerciseBase, IExercise
{
    public virtual string[] Code => ["var list = new List<string> { \"A1\", \"B1\", \"B2\"};",
                                     "var filtered = list.Where(x => x.StartsWith('B'));",
                                     "list.Remove(\"B1\");"];
    public virtual string[] TestCode => ["Console.WriteLine(filtered.Count());"];

    public override string[] Variants => ["0", "1", " ", "2"];

    public override string Exercise()
    {
        var list = new List<string> { "A1", "B1", "B2"};
        var filtered = list.Where(x => x.StartsWith('B'));
        list.Remove("B1");
        return filtered.Count().ToString();
    }
}