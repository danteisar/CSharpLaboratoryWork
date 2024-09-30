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
using Store.Exercises.ExerciseSpecial;

namespace Store.ConsoleExtensions;

internal static class CodeWriter
{
    private const int SPEED = 15;
    private const int CODE_SPEED = 1;
    private const ConsoleColor FOREGROUND_COLOR_VAR = ConsoleColor.Blue;   
    private const ConsoleColor FOREGROUND_COLOR_KEYWORD = ConsoleColor.DarkBlue;
    private const ConsoleColor FOREGROUND_COLOR_FUNCTION = ConsoleColor.DarkYellow;
    private const ConsoleColor FOREGROUND_COLOR_CLASSES = ConsoleColor.DarkGreen;
    private const ConsoleColor FOREGROUND_COLOR_SPECIAL = ConsoleColor.Magenta;
    private const ConsoleColor CODE_BORDER = ConsoleColor.DarkGray;

    private static readonly string[] _keyWords = ["var", 
                                                  "static",
                                                  "public", 
                                                  "protected", 
                                                  "private", 
                                                  "int", 
                                                  "char", 
                                                  "long", 
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
                                                  "enum", 
                                                  "delegate", 
                                                  "{", "}", 
                                                  "const",                                                  
                                                  "if"];
    private static readonly string[] _functions = ["WriteLine", 
                                                   "CompareTo", 
                                                   "Do", 
                                                   "AddAll", 
                                                   "Add", 
                                                   "Where", 
                                                   "ToString", 
                                                   "GetInts", 
                                                   "Last",
                                                   "Init",
                                                   "Select",
                                                   "TryParse",
                                                   "Remove",
                                                   "Where",
                                                   "Take",
                                                   "StartsWith",
                                                   "Count"];
    private static readonly string[] _classes = ["Console", 
                                                 "Amplifier", 
                                                 "BaseStorage", 
                                                 "IComparable", 
                                                 "ItemsStorage", 
                                                 "IEnumerable",
                                                 "Sample",                                                
                                                 "List", 
                                                 "ToArray", 
                                                 "Action", 
                                                 "IsOddNumber", 
                                                 "HString", 
                                                 "StringBuilder"];
    private static readonly string[] _special = ["return", 
                                                 "yield",                                       
                                                 "foreach", 
                                                 "finally",
                                                 "try",                                     
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
    
    public static void Test()
    {

        AskMessage(["ЗАДАНИЕ 1", new Exercise11().Exercise(), new Exercise12().Exercise(), new Exercise13().Exercise(), new Exercise14().Exercise(),
            new Exercise15().Exercise(), new Exercise16().Exercise(), new Exercise17().Exercise()]);

        AskMessage(["ЗАДАНИЕ 2", new Exercise21().Exercise(), new Exercise22().Exercise(), new Exercise23().Exercise(), new Exercise24().Exercise(), 
            new Exercise25().Exercise(), new Exercise26().Exercise(), new Exercise27().Exercise()]);
        
        AskMessage(["ЗАДАНИЕ 3", new Exercise31().Exercise(), new Exercise32().Exercise(), new Exercise33().Exercise(), new Exercise34().Exercise(),
            new Exercise35().Exercise(), new Exercise36().Exercise(), new Exercise37().Exercise()]);

        AskMessage(["ЗАДАНИЕ 4", new Exercise41().Exercise(), new Exercise42().Exercise(), new Exercise43().Exercise(), new Exercise44().Exercise(),
            new Exercise45().Exercise(), new Exercise46().Exercise(), new Exercise01().Exercise()]);

        AskMessage(["ЗАДАНИЕ 5", new Exercise51().Exercise(), new Exercise52().Exercise(), new Exercise53().Exercise(), new Exercise54().Exercise(),
            new Exercise55().Exercise(), new Exercise02().Exercise(), new Exercise03().Exercise()]);

        AskMessage(["ЗАДАНИЕ 6", new Exercise61().Exercise(), new Exercise62().Exercise(), new Exercise63().Exercise(), new Exercise64().Exercise(),
            new Exercise04().Exercise(), new Exercise05().Exercise(), new Exercise06().Exercise()]);
    }

    public static bool ShowExercises(bool _showAnswers)
    {
        ShowMessage(["Тогда необходимо решить 6 задачек на время..."], false, BACKGROUND_COLOR, FOREGROUND_COLOR);
        var rnd = new Random();
        List<List<IExercise>> exercises = [[new Exercise11(), new Exercise12(), new Exercise13(), new Exercise14(), new Exercise15(), new Exercise16(), new Exercise17()], 
                                           [new Exercise21(), new Exercise22(), new Exercise23(), new Exercise24(), new Exercise25(), new Exercise26(), new Exercise27()], 
                                           [new Exercise31(), new Exercise32(), new Exercise33(), new Exercise34(), new Exercise35(), new Exercise36(), new Exercise37()], 
                                           [new Exercise41(), new Exercise42(), new Exercise43(), new Exercise44(), new Exercise45(), new Exercise46(), new Exercise01()], 
                                           [new Exercise51(), new Exercise52(), new Exercise53(), new Exercise54(), new Exercise55(), new Exercise02(), new Exercise03()], 
                                           [new Exercise61(), new Exercise62(), new Exercise63(), new Exercise64(), new Exercise04(), new Exercise05(), new Exercise06()]]; 
        var score = 0;
        int count = 0;  
        int tryCount = 4;
        bool giveUp = false;
        while (count++ < tryCount)                                    
        {
            ShowMessage([$"Попытка №{count} из {tryCount}"], false, BACKGROUND_COLOR, FOREGROUND_COLOR, 1000);
            score = 0;
            List<IExercise> selected = [];
            HashSet<int> sequence  = [];
            int num = 1;
            for (int i = 0; i < exercises.Count; i++)
            {
                var j = rnd.Next(0, exercises[i].Count);
                while(sequence.Any(x=>x == j))
                {
                    j = rnd.Next(0, exercises[i].Count);
                }
                sequence.Add(j);
                exercises[i][j].Number = num++;
                selected.Add(exercises[i][j]);
            }          

            if (_showAnswers) 
                ShowMessage(["Сгенерированная последовательность:",string.Join(" ", sequence)], true, BACKGROUND_COLOR, FOREGROUND_COLOR, 1500);

            foreach (var exercise in selected) 
            {
                (var points, giveUp) = Write(exercise);
                
                score += points;

                if (_showAnswers) 
                    ShowMessage([points == 1 ? "Верно" : "Правильный ответ: " + exercise.Exercise()], 
                        false, points == 1 ? COLOR : ERROR_COLOR, points == 1 ? FOREGROUND_COLOR : ERROR_FOREGROUND_COLOR, 1000);
                
                if (giveUp)
                    break;
            }

            if (score < exercises.Count)
            {
                
                List<string> message = ["       Вы не прошли тест :(", "",
                                       $"Количество верных ответов: {score} из {exercises.Count}"];
                
                if (score == exercises.Count - 1)
                {
                    message.Add(string.Empty);
                    message.Add("     Принять спасательный круг? (y/n)");
                    if (ErrorMessage([.. message], ConsoleKey.Y, ConsoleKey.N).Key != ConsoleKey.Y)
                        continue;
                    else
                    {
                        int ii, jj;
                        do
                        {
                            ii = rnd.Next(0, exercises.Count);
                            jj = rnd.Next(0, exercises.Count);
                        }
                        while(selected.Any(x=>x == exercises[ii][jj]));   
                        
                        (var points, giveUp) = Write(exercises[ii][jj]);                
                        score += points;
                        if (_showAnswers) 
                            ShowMessage([points == 1 ? "Верно" : "Правильный ответ: " + exercises[ii][jj].Exercise()], 
                                false, points == 1 ? COLOR : ERROR_COLOR, points == 1 ? FOREGROUND_COLOR : ERROR_FOREGROUND_COLOR, 1000);
                
                        if (giveUp)
                            break;
                    }
                }
                else if (count < tryCount)
                {                    
                    message.Add(string.Empty);
                    message.Add("     Попытаться еще раз? (y/n)");
                    if (ErrorMessage([.. message], ConsoleKey.Y, ConsoleKey.N).Key != ConsoleKey.Y)
                        break;
                }
                else
                    ShowMessage([.. message], false, ERROR_COLOR, ERROR_FOREGROUND_COLOR,  2000);               
            }

            if (score == exercises.Count) 
                break; 
        }

        var res = score < exercises.Count;
        if (res)
        {
            ShowMessage(["Вы можете ознакомится с демонстрацией 5й лабораторной работы."], false, BACKGROUND_COLOR, FOREGROUND_COLOR);
        }
        else
        {
            ShowMessage(["Поздравляем! Вы прошли тест. У вас всего 4e лабораторных.", 
                         "      Далее можно посмотреть как работает терминал.      "], false, COLOR, FOREGROUND_COLOR);
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
            Thread.Sleep(delay);
        }
        else
        if (count == 1 && text.Length > 0)
        {
            Console.Write(text[0]);
        }        
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
        if (code.Length == 0) return posY;
        Console.ForegroundColor = CODE_BORDER;
        ShowRectangle(posX, posY++, code.Max(x=>x.Length) + 5 + code.Length.ToString().Length, code.Length + 2);
        ShowLinesOfCode(posX + 1, posY, code.Length);
        AnimateCodeLine(posX + 3 + code.Length.ToString().Length, posY++, code, delay);
        return posY + code.Length;
    }

    private static int DrawExercise(IExercise exercise, int speed = 20, int codeSpeed = 1)
    {        
        var posy = 1;
        ClearConsole(BACKGROUND_COLOR);
        var height = exercise.Code.Length + exercise.TestCode.Length + exercise.Variants.Length + 11;
        if (exercise.Variants.Length > 0) height += 2;
        if (Console.WindowHeight < height) Console.WindowHeight = height;
        ShowRectangle(0, 0, Console.WindowWidth, height);
        ShowHorizontalBorder(0, height - 3, Console.WindowWidth);
        ShowHorizontalBorder(0, 2, Console.WindowWidth);
        ShowVerticalBorder(25, height - 3, 3);
        WriteString(1, height - 2, "Осталось времени: " + exercise.NeedTime.ToString(@"mm\:ss"));
        WriteString(27, height - 2, "Ваш ответ:");
        var hint = "Arrow Up, Down - выбор варианта";
        ShowVerticalBorder(Console.WindowWidth - 2 - hint.Length, height - 3, 3);
        WriteString(Console.WindowWidth - 1 - hint.Length, height - 2, hint);
        AnimateText(1, posy++, [$"Задание #{exercise.Number}"], speed);
        posy++;        
        AnimateText(1, posy++, [$"Что будет выведено на консоль?"], speed);
        posy = WriteCode(1, posy, exercise.Code, codeSpeed);  
        posy = WriteCode(1, posy, exercise.TestCode, codeSpeed);                       
        if (exercise.Variants.Length > 0)
        {   
            Console.ForegroundColor = FOREGROUND_COLOR;         
            AnimateTextLine(1, posy++ + 1, ["Варианты ответов:"], speed);
            AnimateTextLine(1, posy++ + 1, exercise.Variants.Select(x=>$"{Array.IndexOf(exercise.Variants, x) + 1}) {x}").ToArray(), speed);
        }
        posy += exercise.Variants.Length + 2;        
        AnimateTextLine(1, posy++, [], speed); 
        Console.ForegroundColor = FOREGROUND_COLOR_VAR;
        return height;
    }
    
    private static (int, bool) Write(IExercise exercise)
    {                
        string answer;
        var period = exercise.NeedTime;
        var speed = SPEED;
        var codeSpeed = CODE_SPEED;
        while (true)
        {
            var height = DrawExercise(exercise, speed, codeSpeed);           
            (answer, period, var esc) = InputWait(19, height - 2, period, 38, height - 2, exercise.Variants);
            var tmp = exercise.Check(answer);

            if (esc && AskMessage(["Вы хотите сдаться? (y/n)"], ConsoleKey.Y, ConsoleKey.N).Key == ConsoleKey.Y)
            {
                return (tmp, true);
            }
            else if (tmp < 0)
            {
                ShowMessage(["Варианта такого нет :("], false, ERROR_COLOR, ERROR_FOREGROUND_COLOR, 1000);
            }
            else if (!esc && !string.IsNullOrEmpty(answer) ||
                period == TimeSpan.Zero ||
                (!esc && period != TimeSpan.Zero && AskMessage(["Вы уверены, что хотите оставить пустой ответ? (y/n)"], ConsoleKey.Y, ConsoleKey.N).Key == ConsoleKey.Y))
            {
                return (tmp, false);
            }
            speed = 0;
            codeSpeed = 0;
        }
    }

    private static (string answer, TimeSpan rest, bool giveUp)  InputWait(int posX, int posY, TimeSpan period, int x, int y, string[] variants = null)
    {
        ConsoleKeyInfo cki;
        var dt = DateTime.Now;
        var sb = new StringBuilder(10);
        int index = -1;  
        var empty = variants is {} ? string.Empty.PadRight(variants.Max(x=>x.Length), ' ') : " ";        
        do 
        {                        
            while (Console.KeyAvailable == false)
            {
                var rest = period - (DateTime.Now - dt);
                WriteString(posX, posY, rest.ToString(@"mm\:ss"));    
                
                if (rest.TotalMilliseconds <= 0) 
                    return (sb.ToString(), TimeSpan.Zero, false); 

                Thread.Sleep(100);
            }           

            cki = Console.ReadKey(true);
            if (cki.Key == ConsoleKey.Backspace || variants is {} && (cki.Key == ConsoleKey.DownArrow || cki.Key == ConsoleKey.UpArrow))
            {
                if (cki.Key == ConsoleKey.DownArrow)
                    index++;
                if (cki.Key == ConsoleKey.UpArrow)
                    index--;
                if (index < 0) index = 0;
                if (index >= variants.Length) index = variants.Length - 1;

                WriteString(x, y, string.Empty.PadRight(sb.ToString().Length, ' '));
                sb.Clear();
                
                if (cki.Key != ConsoleKey.Backspace)
                    sb.Append(variants[index]);
                        
            }
            else if (cki.Key == ConsoleKey.Escape){
                return (sb.ToString(), period - (DateTime.Now - dt), true);
            }
            else if (cki.Key != ConsoleKey.Enter)
            {
                sb.Append(cki.KeyChar);          
            }            

            WriteString(x, y, sb.ToString());
        } 
        while(cki.Key != ConsoleKey.Enter);

        return (sb.ToString(), period - (DateTime.Now - dt), false);
    }
}
