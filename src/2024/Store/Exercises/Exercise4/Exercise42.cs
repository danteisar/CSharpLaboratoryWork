namespace Store.Exercises.Exercise4;

internal class Exercise42: Exercise41, IExercise
{
    public override string[] Code => ["public class Amplifier",
                                      "{",
                                      "    public string ModelName => string.Empty;",
                                      "    public int Volume => 123;",
                                      "    public Amplifier() : this(\"Model X\", 69) { }",
                                      "    public Amplifier(string modelName, int volume)",
                                      "    {",
                                      "        ModelName = modelName;",
                                      "        Volume = volume;",
                                      "    }",
                                      "}"];
    
    public override string[] TestCode => ["Console.WriteLine(new Amplifier().ModelName);"];
    
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
}