﻿using System.Text;

namespace Store.Exercises.Exercise3;

internal class Exercise31 : ExerciseBase, IExercise
{
    public virtual string[] Code => ["public class HString",
                                     "{",
                                     "    private const int initSize = 64;",
                                     "    private StringBuilder sb;",
                                     "    private void Init(int iniSize) => sb = new StringBuilder(iniSize);",
                                     "    public HString() => Init(initSize);",
                                     "    public HString(int iniSize) => Init(initSize);",
                                     "    public StringBuilder SB => sb;",
                                     "}"];
    
    public virtual string[] TestCode => ["Console.WriteLine(new HString(256).SB.Capacity);"];
    public override string[] Variants => ["16","64","128", "256"];

    public class HString
    {
        private const int initSize = 64;
        private StringBuilder sb;
        private void Init(int iniSize) => sb = new StringBuilder(iniSize);
        public HString() => Init(initSize);
        public HString(int iniSize) => Init(initSize);
        public StringBuilder StringBuilder => sb;
    }
    public override string Exercise()
    {        
        return new HString(256).StringBuilder.Capacity.ToString();
    }
}
