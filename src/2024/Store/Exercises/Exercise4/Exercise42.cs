namespace Store.Exercises.Exercise4;

internal class Exercise42: ExerciseBase, IExercise
{
    public string[] Code => ["public class Amplifier",
                             "{",
                             "    public string ModelName => string.Empty;",
                             "    public int Volume => 123;",
                             "    public Amplifier() : this(\"Model X\", 69) { }",
                             "    public Amplifier(string modelName, int volume)",
                             "    {",
                             "        ModelName = modelName;",
                             "        Volume = volume;",
                             "    }",
                             "}",
                             " ",
                             "Console.WriteLine(new Amplifier().ModelName);"];

    public string[] Variants => ["0", "69", "123", " ", "Model X"];

    /*
    public class Amplifier
    {
        public string ModelName => string.Empty;
        public int Volume => 123;
        public Amplifier() : this("Model X", 69) { }
        public Amplifier(string modelName, int volume)
        {
            ModelName = modelName; //Error
            Volume = volume; //Error
        }
    }*/

    public override string Exercise()
    {      
        return string.Empty;// Error
    }    
}