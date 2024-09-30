namespace Store.Exercises.Exercise1;

internal class Exercise15 : ExerciseBase, IExercise
{
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(3);
    
    public virtual string[] Code => ["public class Sample",
                                     "{",
                                     "    static Sample() { Console.WriteLine(\"Static\");}",
                                     "    private Sample() { Console.WriteLine(\"Instance\"); }",
                                     "    public static Sample Instance { get; } = new();",
                                     "}"];

    public virtual string[] TestCode => ["var sample = Sample.Instance;"];

    public override string[] Variants => ["Static", "Instance", "Static Instance", "Instance Static"];    

    private static string _result = string.Empty;
    
    public class Sample
    {
        static Sample() { _result += " Static";}
        private Sample() { _result += "Instance"; }
        public static Sample Instance { get; } = new();
    }

    public override string Exercise()
    {
        var _ = Sample.Instance;
        return _result;
    }
}
