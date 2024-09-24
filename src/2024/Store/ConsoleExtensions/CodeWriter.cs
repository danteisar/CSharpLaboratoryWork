using System.Text;
using static Store.Constants;
using static Store.ConsoleExtensions.ConsoleWriter;
using Store.Exercises;
using Store.Exercises.Exercise1;
using Store.Exercises.Exercise2;
using Store.Exercises.Exercise3;
using Store.Exercises.Exercise4;
using Store.Exercises.Exercise5;
using Store.Exercises.Exercise6;

namespace Store.ConsoleExtensions;

internal static class CodeWriter
{
    public const ConsoleColor FOREGROUND_COLOR_VAR = ConsoleColor.Blue;   
    public const ConsoleColor FOREGROUND_COLOR_KEYWORD = ConsoleColor.DarkBlue;
    public const ConsoleColor FOREGROUND_COLOR_FUNCTION = ConsoleColor.DarkYellow;
    public const ConsoleColor FOREGROUND_COLOR_CLASSES = ConsoleColor.DarkGreen;
    public const ConsoleColor FOREGROUND_COLOR_SPECIAL = ConsoleColor.Magenta;
    public const ConsoleColor CODE_BORDER = ConsoleColor.DarkGray;
   
    private static readonly string[] _keyWords = ["var", 
                                                  "static",
                                                  "public", 
                                                  "protected", 
                                                  "private", 
                                                  "int", 
                                                  "string", 
                                                  "double", 
                                                  "this", 
                                                  "while", 
                                                  "new", 
                                                  "readonly",  
                                                  "base", 
                                                  "virtual", 
                                                  "override", 
                                                  "get", 
                                                  "set", 
                                                  "void", 
                                                  "class", 
                                                  "{", "}", 
                                                  "const",
                                                  "if"];
    private static readonly string[] _functions = ["WriteLine", 
                                                   "CompareTo", 
                                                   "Do", 
                                                   "AddAll", 
                                                   "Add", 
                                                   "ToString", 
                                                   "GetInts", 
                                                   "Last",
                                                   "Init",
                                                   "Count"];
    private static readonly string[] _classes = ["Console", 
                                                 "Amplifier", 
                                                 "BaseStorage", 
                                                 "IComparable", 
                                                 "ItemsStorage", 
                                                 "IEnumerable", 
                                                 "List", 
                                                 "HString", 
                                                 "StringBuilder"];
    private static readonly string[] _special = ["return", 
                                      "yield",                                       
                                      "foreach",                                      
                                      " in ",
                                      "<", ">",
                                      "[", "]",
                                      "\"Model X\""];
    private static readonly string[] _rest = [";", 
                                              "~",
                                              "<<",
                                              "=>", 
                                              "<=", 
                                              "++",
                                              "+=",
                                              "--",
                                              " > ", 
                                              " < ", 
                                              " = ",
                                              " : ",
                                              " + ",
                                              "/",
                                              "-",
                                              ",",
                                              "."
                                              ];
    
    public static bool ShowExercises(bool _showAnswers)
    {
        AskMessage("Тогда необходимо решить 6 задачек на время...", false);
        var rnd = new Random();
        List<List<IExercise>> exercises = [[new Exercise11(), new Exercise12(), new Exercise13(), new Exercise14()], 
                                           [new Exercise21(), new Exercise22(), new Exercise23(), new Exercise24()], 
                                           [new Exercise31(), new Exercise32(), new Exercise33(), new Exercise34()], 
                                           [new Exercise41(), new Exercise42(), new Exercise43(), new Exercise44()], 
                                           [new Exercise51(), new Exercise52(), new Exercise53(), new Exercise54()], 
                                           [new Exercise61(), new Exercise62(), new Exercise63(), new Exercise64()]]; 
                                            
        List<IExercise> selected = [];
        int num = 1;
        for (int i = 0; i < exercises.Count; i++){
            var j = rnd.Next(0, exercises[i].Count);
            exercises[i][j].Number = num++;
            selected.Add(exercises[i][j]);
        }                    
        var score = 0;
        foreach (var exercise in selected) 
        {
                score += Write(exercise);
                if (_showAnswers) 
                    AskMessage("Ответ:" + exercise.Exercise(), false);
        } 

        var res = score < 6;
        if (res)
        {
            AskMessage("Вы не прошли тест.", false);
            AskMessage($"Количество верных ответов: {score} из 6", false);
            AskMessage("Вы можете ознакомится с демонстрацией 5й лабораторной работы.", false);
        }
        else
        {
            AskMessage("Поздравляем! Вы прошли тест. У вас всего 4e лабораторных.", false);
            AskMessage("Далее можно посмотреть как работает терминал.", false);
        }
        return res;
    }

    private static void ShowLinesOfCode(int offsetX, int offsetY, int count)
    {
        if (count == 0) return;        
        var width = count.ToString().Length;
        WriteChar(offsetX + width, offsetY - 1, A6);
        for (int i = 0; i < count; i++)
        {
            WriteString(offsetX, offsetY + i, (i+1).ToString().PadRight(width, ' ') + IV);
        }
        WriteChar(offsetX + width, offsetY + count, A5);
    }

    private static void WriteCode(string text, int delay, ref int scope)
    {
        if (string.IsNullOrEmpty(text)) return;

        var count = 1;
        var cc = FOREGROUND_COLOR_VAR;        

        var res = FindKeyWord(ref count, ref cc, text)
            || FindClassName(ref count, ref cc, text)
            || FindFunctionName(ref count, ref cc, text)            
            || FindRest(ref count, ref cc, text)
            || FindSpecial(ref count, ref cc, text);

        Console.ForegroundColor = cc;

        if (text[0] == '(') scope++;        
        if ((text[0] == '(' || text[0] == ')') && count == 1)
            switch (scope % 3)
            {
                case 0:
                    Console.ForegroundColor = FOREGROUND_COLOR_KEYWORD;
                break;
                case 1:
                    Console.ForegroundColor = FOREGROUND_COLOR_FUNCTION;
                break;
                case 2:
                    Console.ForegroundColor = FOREGROUND_COLOR_SPECIAL;
                break;
            }        
        if (text[0] == ')') scope--;

        if (res)
        {
            Console.Write(text[..count]);
        }
        else
        if (count == 1 && text.Length > 0)
        {
            Console.Write(text[0]);
        }
        Thread.Sleep(delay);
        WriteCode(text[count..], delay, ref scope);
    }

    public static void AnimateCodeLine(int x, int y, string[] text, int delay)
    {
        var posY = y;
        var scope = -1;
        foreach (var line in text)
        {
            var posX = x;
            Console.SetCursorPosition(posX, posY++);
            WriteCode(line, delay, ref scope);
        }
    }

    private static bool FindWord(ConsoleColor targetColor, string[] source, string text, ref int count, ref ConsoleColor consoleColor)
    {
        foreach (var word in source.Where(text.StartsWith))
        {
            count = word.Length;
            consoleColor = targetColor;
            return true;
        }
        return false;
    }

    private static bool FindKeyWord(ref int count, ref ConsoleColor consoleColor, string text)
        => FindWord(FOREGROUND_COLOR_KEYWORD, _keyWords, text, ref count, ref consoleColor);
    private static bool FindClassName(ref int count, ref ConsoleColor consoleColor, string text)
        => FindWord(FOREGROUND_COLOR_CLASSES, _classes, text, ref count, ref consoleColor);
    private static bool FindFunctionName(ref int count, ref ConsoleColor consoleColor, string text)
        => FindWord(FOREGROUND_COLOR_FUNCTION, _functions, text, ref count, ref consoleColor);
    private static bool FindSpecial(ref int count, ref ConsoleColor consoleColor, string text)
        => FindWord(FOREGROUND_COLOR_SPECIAL, _special, text, ref count, ref consoleColor);
    private static bool FindRest(ref int count, ref ConsoleColor consoleColor, string text)
        => FindWord(FOREGROUND_COLOR, _rest, text, ref count, ref consoleColor);
    
    private static int WriteCode(int posX, int posY, string[] code, int delay = 1)
    {
        Console.ForegroundColor = CODE_BORDER;
        ShowRectangle(posX, posY++, code.Max(x=>x.Length) + 5 + code.Length.ToString().Length, code.Length + 2);
        ShowLinesOfCode(posX + 1, posY, code.Length);
        AnimateCodeLine(posX + 3 + code.Length.ToString().Length, posY++, code, delay);
        return posY;
    }

    private static int Write(IExercise exercise)
    {
        int speed = 50;
        var posy = 1;
        ClearConsole();
        var height = exercise.Code.Length + exercise.Variants.Length + 10;
        if (exercise.Variants.Length > 0) height += 2;
        if (Console.WindowHeight < height) Console.WindowHeight = height;
        ShowRectangle(0, 0, Console.WindowWidth, height);
        ShowHorizontalBorder(0, height - 3, Console.WindowWidth);
        ShowHorizontalBorder(0, 2, Console.WindowWidth);
        ShowVerticalBorder(25, height - 3, 3);
        WriteString(1, height - 2, "Осталось времени: " + exercise.NeedTime.ToString(@"mm\:ss"));
        WriteString(27, height - 2, "Ваш ответ:");
        AnimateText(1, posy++, [$"Задание #{exercise.Number}"], speed);
        posy++;        
        AnimateText(1, posy++, [$"Что будет выведено на консоль?"], speed);
        posy = WriteCode(1, posy, exercise.Code);                        
        if (exercise.Variants.Length > 0)
        {   
            Console.ForegroundColor = FOREGROUND_COLOR;         
            AnimateTextLine(1, posy++ + exercise.Code.Length + 1, ["Варианты ответов:"], speed);
            AnimateTextLine(1, posy++ + exercise.Code.Length + 1, exercise.Variants, speed);
        }
        posy += exercise.Code.Length + exercise.Variants.Length + 2;        
        AnimateTextLine(1, posy++, [], speed); 
        Console.ForegroundColor = FOREGROUND_COLOR_VAR;
        var answer = InputWait(19, height - 2, exercise.NeedTime, 38, height - 2);          
        return exercise.Check(answer) ? 1 : 0;
    }

    private static string InputWait(int posX, int posY, TimeSpan period, int x, int y)
    {
        ConsoleKeyInfo cki;
        var dt = DateTime.Now;
        var sb = new StringBuilder(10);
        var sb2 = new StringBuilder(10);           
        do 
        {                        
            while (Console.KeyAvailable == false)
            {
                var rest = period - (DateTime.Now - dt);
                WriteString(posX, posY, rest.ToString(@"mm\:ss"));    
                
                if (rest.TotalMilliseconds <= 0) 
                    return sb.ToString(); 

                Thread.Sleep(100);
            }           

            cki = Console.ReadKey(true);
            if (cki.Key != ConsoleKey.Enter)
            {
                sb.Append(cki.KeyChar);    
                sb2.Append(' ');           
            } 
            if (cki.Key == ConsoleKey.Backspace)
            {
                sb.Clear();
                WriteString(x, y, sb2.ToString());
                sb2.Clear();                
            }           
            
            WriteString(x, y, sb.ToString());
        } 
        while(cki.Key != ConsoleKey.Enter );
        return sb.ToString();
    }
}
