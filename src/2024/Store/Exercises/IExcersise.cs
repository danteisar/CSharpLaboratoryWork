namespace Store.Exercises;

internal interface IExercise
{    
    int Number { get; set;}
    string[] Code { get; }
    string[] TestCode { get; }
    string[] Variants { get; }
    int Check(string text);
    string Exercise();   
    TimeSpan NeedTime {get; }
}