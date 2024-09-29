namespace Store.Exercises.Exercise4;

internal class Exercise44: ExerciseBase, IExercise
{
    public string[] Code => ["public class Amplifier",
                             "{",
                             "    public string ModelName { get; } = string.Empty;",
                             "    public int Volume { get; } = 123;",
                             "    public Amplifier() : this(\"Model X\", 69) { }",
                             "    public Amplifier(string modelName, int volume)",
                             "    {",
                             "        ModelName = modelName;",
                             "        Volume = volume;",
                             "    }",
                             "}",
                             " ",
                             "var a = new Amplifier();",
                             "Console.WriteLine(a.ModelName);"];

    public string[] Variants => ["0", "69", "123", " ", "Model X"];

    public class Amplifier
    {
        public string ModelName { get; } = string.Empty;
        public int Volume { get; } = 123;
        public Amplifier() : this("Model X", 69) { }
        public Amplifier(string modelName, int volume)
        {
            ModelName = modelName;
            Volume = volume;
        }
    }

    public override string Exercise()
    {       
        return new Amplifier().ModelName;
    }    
}