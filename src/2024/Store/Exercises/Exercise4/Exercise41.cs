﻿namespace Store.Exercises.Exercise4;

internal class Exercise41 : ExerciseBase, IExercise
{
    public virtual string[] Code => ["public class Amplifier",
                                     "{",
                                     "    public string ModelName => 123;",
                                     "    public int Volume => string.Empty;",
                                     "    public Amplifier() : this(\"Model X\", 69) { }",
                                     "    public Amplifier(string modelName, int volume) : this()",
                                     "    {",
                                     "        ModelName = modelName;",
                                     "        Volume = volume;",
                                     "    }",
                                     "}"];
   
    public virtual string[] TestCode => ["Console.WriteLine(new Amplifier().Volume);"];
    public override string[] Variants => ["0", "69", "123", " ", "Model X"];
    
    /*
    public class Amplifier
    {
        public string ModelName => 123; //Error
        public int Volume => string.Empty; //Error
        public Amplifier() : this("Model X", 69) { }
        public Amplifier(string modelName, int volume) : this() //Error
        {
            ModelName = modelName;
            Volume = volume;
        }
    }
    */  
}
