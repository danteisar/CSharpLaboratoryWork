namespace Store.Exercises.Exercise1;

internal class Exercise16 : ExerciseBase, IExercise
{
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(3);
    
    public virtual string[] Code => ["List<Action> ints = [];", 
                                     "for (int i = 0; i < 3; i++)", 
                                     "{", 
                                     "    ints.Add(delegate {Console.WriteLine(i);});",
                                     "}"];

    public virtual string[] TestCode => ["foreach (Action write in ints)", 
                                         "{", 
                                         "    write();", 
                                         "}"];

    public override string[] Variants => ["0 0 0", "0 1 2", "2 2 2", "3 3 3"];    

    public static void Do()
    {
        List<Action> ints = []; 
        for (int i = 0; i < 3; i++)
        {
            ints.Add(delegate {Console.WriteLine(i);});
        }
        foreach (Action write in ints)
        {
            write();
        }
    }

    public override string Exercise()
    {
        List<Func<int>> ints = []; 
        for (int i = 0; i < 3; i++)
        {
            ints.Add(()=>i);
        }
        return string.Join(" ", ints.Select(x => x()));
    }
}
