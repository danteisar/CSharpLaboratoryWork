namespace Store.Exercises.Exercise4;

internal class Exercise43: ExerciseBase, IExercise
{
    public string[] Code => ["public class Amplifier",
                             "{",
                             "    public string ModelName = string.Empty;",
                             "    public int Volume int Volume = 123",
                             "    public Amplifier() : this(\"Model X\", 69) { }",
                             "    public Amplifier(string modelName, int volume)",
                             "    {",
                             "        ModelName = modelName;",
                             "        Volume = volume;",
                             "    }",
                             "}",
                             " ",
                             "var a = new Amplifier();",
                             "Console.WriteLine(a.Volume);"];

    public string[] Variants => ["69", "123", "\"\"", "Model X"];

    public class Amplifier(string modelName, int volume)
    {
        public string ModelName = modelName;
        public int Volume = volume;
        public Amplifier() : this("Model X", 69) { }
    }

    public override string Exercise()
    {
        var a = new Amplifier();        
        return a.Volume.ToString();
    }    
}