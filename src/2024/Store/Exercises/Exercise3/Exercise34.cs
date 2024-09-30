using System.Text;

namespace Store.Exercises.Exercise3;

internal class Exercise34: Exercise31, IExercise
{
    public override string[] Code => ["public class HString",
                                      "{",
                                      "    private const int initSize = 0;",
                                      "    private StringBuilder sb;",
                                      "    private void Init(int iniSize) => sb = new StringBuilder(iniSize);",
                                      "    public HString() => Init(initSize);",
                                      "    public HString(int iniSize) => Init(initSize);",
                                      "    public StringBuilder SB => sb;",
                                      "}"];

    public class HString4
    {
        private const int initSize = 0;
        private StringBuilder sb;
        private void Init(int iniSize) => sb = new StringBuilder(iniSize);
        public HString4() => Init(initSize);
        public HString4(int iniSize) => Init(initSize);
        public StringBuilder StringBuilder => sb;
    }
    public override string Exercise()
    {        
        return new HString4(256).StringBuilder.Capacity.ToString();
    }
}
