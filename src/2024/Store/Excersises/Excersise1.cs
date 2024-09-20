namespace Store.Excersises;

public class Excersise1 : ExcersiseBase, IExcersise
{
    public int Number => 1;

    public string[] Code => ["Console.WriteLine(~0 << 2);"];

    public string[] Variants => ["-100", "-4", "-1", "0", "1", "4", "100"];

    public override string Excersise() => (~0 << 2).ToString();
}