using System.Text;

namespace Store.Exercises.Exercise3;

internal class Exercise33: Exercise31, IExercise
{
    public override string[] Code => ["public class HString",
                             "{",
                             "    private const int initSize = 64;",
                             "    private StringBuilder sb;",
                             "    private void Init(int iniSize) => sb = new StringBuilder(iniSize);",
                             "    public HString() => Init(128);",
                             "    public HString(int iniSize = 256) => Init(initSize);",
                             "    public StringBuilder SB => sb;",
                             "}"];
    
    public override string[] TestCode => ["Console.WriteLine(new HString().SB.Capacity);"];

    public class HString3
    {
        private const int initSize = 64;
        private StringBuilder sb;
        private void Init(int iniSize) => sb = new StringBuilder(iniSize);
        public HString3() => Init(128);
        public HString3(int iniSize = 256) => Init(initSize);
        public StringBuilder StringBuilder => sb;
    }
    public override string Exercise()
    {        
        return new HString3().StringBuilder.Capacity.ToString();
    }
}
