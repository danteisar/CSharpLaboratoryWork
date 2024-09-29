namespace Store.Exercises.Exercise4;

internal class Exercise45: ExerciseBase, IExercise
{
    public virtual string[] Code => ["public class Amplifier(string modelName = \"\", int volume = 0)",
                                     "{",
                                     "    public string ModelName => modelName = \"0\";",
                                     "    public int Volume => volume = 123;",
                                     "    public Amplifier() : this(\"Model X\", 69) { }",                          
                                     "}",
                                     " ",
                                     "Console.WriteLine(new Amplifier().Volume);"];

    public string[] Variants => ["0", "69", "123", " ", "Model X"];

    public class Amplifier(string modelName = "", int volume = 0)
    {
        public string ModelName => modelName = "0";
        public int Volume => volume = 123;
        public Amplifier() : this("Model X", 69) { }
    }

    public override string Exercise()
    {      
        return new Amplifier().Volume.ToString();
    }    
}