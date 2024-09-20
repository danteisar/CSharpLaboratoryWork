using System.Text;

namespace Store.Excersises;

public class Excersise3 : ExcersiseBase, IExcersise
{
    public int Number => 3;

    public string[] Code => ["public class HString",
                             "{",
                             "    private const int initSize = 64;",
                             "    private StringBuilder sb;",
                             "    private void Init(int iniSize) => sb = new StringBuilder(iniSize);",
                             "    public HString() => Init(initSize);",
                             "    public HString(int iniSize) => Init(initSize);",
                             "    public StringBuilder StringBuilder => sb;",
                             "}",
                             " ",
                             "Console.WriteLine(new HString(256).StringBuilder.Capacity);"
                             ];

    public string[] Variants => ["0","64","128", "256"];

    public class HString
    {
        private const int initSize = 64;
        private StringBuilder sb;
        private void Init(int iniSize) => sb = new StringBuilder(iniSize);
        public HString() => Init(initSize);
        public HString(int iniSize) => Init(initSize);
        public StringBuilder StringBuilder => sb;
    }
    public override string Excersise()
    {        
        return new HString(256).StringBuilder.Capacity.ToString();
    }
}
