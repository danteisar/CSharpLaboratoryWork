namespace Store.Exercises.Exercise4;

internal class Exercise45: Exercise41, IExercise
{
    public override string[] Code => ["public class Amplifier(string modelName = \"\", int volume = 0)",
                                      "{",
                                      "    public string ModelName => modelName = \"0\";",
                                      "    public int Volume => volume = 123;",
                                      "    public Amplifier() : this(\"Model X\", 69) { }",                          
                                      "}"];

    public override string[] TestCode => ["Console.WriteLine(new Amplifier().Volume);"];

    protected class Amplifier(string modelName = "", int volume = 0)
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