namespace Store.Exercises.Exercise1;

internal class Exercise12: Exercise11, IExercise
{
    public override TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(3);
    
    public override string[] Code => ["public static IEnumerable<int> GetInts()",
                                      "{",
                                      "    yield return 1;",
                                      "    Console.WriteLine(2);",
                                      "    yield return 3;",                            
                                      "}"];
                                      
    public override string Exercise() => "2 3";
}