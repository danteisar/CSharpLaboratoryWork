namespace Store.Exercises;

internal abstract class ExerciseBase
{
    public int Number { get; set; }
    
    public abstract string[] Variants { get; }

    public virtual string Exercise() => string.Empty;

    private static bool Compare(string text1, string text2) => string.Equals(text1.Replace(" ",""), text2?.Replace(" ",""));
    
    public virtual int Check(string text)
    {
        if (Variants.Any(x=> Compare(x, text)))
            return Compare(Exercise(), text) ? 1 : 0;

        return -1;
    }

    public virtual TimeSpan NeedTime { get; } = TimeSpan.FromMinutes(5);
}
