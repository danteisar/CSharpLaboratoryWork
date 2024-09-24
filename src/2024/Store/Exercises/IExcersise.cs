namespace Store.Exercises;

internal interface IExercise
{    
    int Number { get; set;}
    string[] Code { get; }
    string[] Variants { get; }
    bool Check(string text);
    string Exercise();   
    TimeSpan NeedTime {get; }
}