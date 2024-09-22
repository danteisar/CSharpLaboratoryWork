﻿using System.Text;

namespace Store.Exercises;

internal class Exercise3 : ExerciseBase, IExercise
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
    public override string Exercise()
    {        
        return new HString(256).StringBuilder.Capacity.ToString();
    }
}