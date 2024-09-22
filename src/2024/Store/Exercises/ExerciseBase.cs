namespace Store.Exercises;

internal abstract class ExerciseBase
{
    public abstract string Exercise();

    public virtual bool Check(string text) => string.Equals(Exercise(), text);
}

