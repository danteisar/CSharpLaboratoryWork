namespace Store.Exercises.Exercise4;

internal class Exercise44: Exercise41, IExercise
{
    public override string[] Code => ["public class Amplifier",
                                      "{",
                                      "    public string ModelName { get; } = string.Empty;",
                                      "    public int Volume { get; } = 123;",
                                      "    public Amplifier() : this(\"Model X\", 69) { }",
                                      "    public Amplifier(string modelName, int volume)",
                                      "    {",
                                      "        ModelName = modelName;",
                                      "        Volume = volume;",
                                      "    }",
                                      "}"];
    public override string[] TestCode => ["Console.WriteLine(new Amplifier().ModelName);"];

    private class Amplifier
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