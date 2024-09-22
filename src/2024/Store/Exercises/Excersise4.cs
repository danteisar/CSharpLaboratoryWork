namespace Store.Exercises;

internal class Exercise4 : ExerciseBase, IExercise
{
    public int Number => 4;

    public string[] Code => ["public class Amplifier",
                         "{",
                         "    public string ModelName = 123;",
                         "    public int Volume = string.Empty;",
                         "    public Amplifier() : this(\"Model X\", 69) { }",
                         "    public Amplifier(string modelName, int volume) : this()",
                         "    {",
                         "        ModelName = modelName;",
                         "        Volume = volume;",
                         "    }",
                         " ",
                         "var a = new Amplifier();",
                         "Console.WriteLine(a.Volume);"];

    public string[] Variants => [];

    public override string Exercise()
    {
        return "";// ,"Add";
    }

    public class BaseStorage
    {
        private List<double> _list = new();
        public virtual void Add(double value)=>_list.Add(value);
        public virtual void AddAll(IEnumerable<double> values)
        {
            foreach (var val in values)
            {
                Add(val);
            }
        }
    }
}
