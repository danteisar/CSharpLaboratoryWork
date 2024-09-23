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
                         "}",
                         " ",
                         "var a = new Amplifier();",
                         "Console.WriteLine(a.Volume);"];

    public string[] Variants => [];

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
