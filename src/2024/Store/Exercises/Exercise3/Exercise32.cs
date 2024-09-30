using System.Text;

namespace Store.Exercises.Exercise3;

internal class Exercise32: Exercise31, IExercise
{
    public override string[] Code => ["public class HString",
                                      "{",
                                      "    private const int initSize = 64;",
                                      "    private StringBuilder sb;",
                                      "    private void Init(int iniSize) => sb = new StringBuilder(iniSize);",
                                      "    public HString() => Init(initSize);",
                                      "    public HString(int iniSize) => Init(iniSize);",
                                      "    public StringBuilder SB => sb;",
                                      "}"];

    public class HString2
    {
        private const int initSize = 64;
        private StringBuilder sb;
        private void Init(int iniSize) => sb = new StringBuilder(iniSize);
        public HString2() => Init(initSize);
        public HString2(int iniSize) => Init(iniSize);
        public StringBuilder StringBuilder => sb;
    }
    public override string Exercise()
    {        
        return new HString2(256).StringBuilder.Capacity.ToString();
    }
}
