namespace Store.Excersises;

internal interface IExcersise
{    
    int Number { get; }
    string[] Code { get; }
    string[] Variants { get; }
    bool Check(string text);
    public string Excersise();   
}
    
public abstract class ExcersiseBase
{
    public abstract string Excersise();

    public virtual bool Check(string text) => string.Equals(Excersise(), text);
}
