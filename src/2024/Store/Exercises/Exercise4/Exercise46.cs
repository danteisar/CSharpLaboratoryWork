namespace Store.Exercises.Exercise4;

internal class Exercise46: Exercise45, IExercise
{
    public override string[] Code => ["public class Amplifier(string modelName = \"\", int volume = 0)",
                                      "{",
                                      "    public string ModelName => modelName = \"0\";",
                                      "    public int Volume => volume = 123;",
                                      "    public Amplifier() : this(\"Model X\", 69) { }",                          
                                      "}",
                                      " ",
                                      "Console.WriteLine(new Amplifier().ModelName);"];

    public override string Exercise()
    {    
        return new Amplifier().ModelName;
    }    
}