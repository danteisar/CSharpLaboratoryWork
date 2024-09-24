namespace Store.Exercises.Exercise4;

internal class Exercise41 : ExerciseBase, IExercise
{
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
                         "}",
                         " ",
                         "var a = new Amplifier();",
                         "Console.WriteLine(a.Volume);"];

    public string[] Variants => ["69", "123", " ", "Model X"];

    public class Amplifier
    {
        public string ModelName = string.Empty;
        public int Volume = 123;
        public Amplifier() : this("Model X", 69) { }
        public Amplifier(string modelName, int volume) //: this()
        {
            ModelName = modelName;
            Volume = volume;
        }
    }

    public override string Exercise()
    {
        var a = new Amplifier();        
        return string.Empty;// Error
    }    
}
