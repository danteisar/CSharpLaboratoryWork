using System.Text;
using Barcode.Lab1;
using Barcode.Lab3;
using Store.Exercises;
using static Store.Constants;

namespace Store.ConsoleExtensions;

internal static class ConsoleWriter
{
    public static bool IsDemo { get; set; } = true;
    public static bool IncludeLoading { get; set; } = true;

    public static void WriteChar(int posX, int posY, char c)
    {
        Console.SetCursorPosition(posX, posY);
        Console.Write(c);
        Console.CursorVisible = false;
    }

    public static void WriteString(int posX, int posY, string str)
    {
        Console.SetCursorPosition(posX, posY);
        Console.Write(str);
        Console.CursorVisible = false;
    }

    public static void RestoreChar(Item e)
    {
        if (e is null) return;
        Console.ForegroundColor = e.Color;
        WriteChar(e.X, e.Y, e.Char);
    }

    public static void RestoreChar((int x, int y) pos)
    {
        WriteChar(pos.x, pos.y, EMPTY);
    }

    public static void ClearConsole()
    {
        Console.BackgroundColor = BACKGROUND_COLOR;
        Console.ForegroundColor = FOREGROUND_COLOR;
        Console.Clear();
        Console.CursorVisible = true;
    }

    public static void ClearTerminal()
    {
        Console.WindowHeight = HEIGHT + 1;
        if (Console.WindowWidth < 121)
        {
            Console.WindowWidth = 121;
        }
        Console.CursorVisible = false;
        Console.BackgroundColor = BACKGROUND_COLOR;
        Console.ForegroundColor = FOREGROUND_COLOR;

        Console.Clear();

        Barcode1.Type = BarcodeType.Full;
        Barcode3.Type = BarcodeType.Full;
    }

    public static void ShowLoading()
    {
        if (!IncludeLoading) return;

        ClearConsole();
        var rnd = new Random();
        Console.CursorVisible = false;
        var y = 0;
        foreach (var item in MsDos())
        {
            AnimateText(0, y++, [item], 0);
            Thread.Sleep(rnd.Next(10, 100));
        }
        y--;
        var x = MsDos().Last().Length;
        for (int i = 0; i < 2; i++)
        {
            AnimateText(4, y, [" "], 500);
            AnimateText(4, y, ["_"], 500);
        }
        AnimateText(4, y, ["TERMINAL.EXE"], 50);
        Thread.Sleep(500);
        ClearTerminal();
        Barcode1 logo = "TERMINAL v.1.0";
        var text = logo.ToString().Split('\n');
        var posX = (Console.WindowWidth - text[0].Length + 2) / 2;
        var posY = (Console.WindowHeight - 8) / 2;

        ShowRectangle(posX - 1, posY - 1, text[0].Length + 4, 11);
        AnimateText(posX + 1, posY + 1, text, 5);
        Thread.Sleep(1000);
        IncludeLoading = false;
    }

    public static void AnimateText(int x, int y, string[] text, int delay)
    {
        for (int i = 0; i < text[0].Length; i++)
        {
            for (int j = 0; j < text.Length; j++)
            {
                if (i < text[j].Length)
                {
                    Console.SetCursorPosition(x + i, y + j);
                    Console.Write(text[j][i]);
                }
            }
            if (delay > 0) Thread.Sleep(delay);
        }
    }

    private static string[] MsDos()
    {
        return
        [
            "Welcome to FreeDOS",
            "",
            "CuteMouse v1.9.1 alpha 1 [FreeDOS]",
            "Installed at PS/2 port",
            @"c:\>ver",
            "",
            "FreeCom version 0.82 pl 3 XMS_Swap [Dec 19 2024 18:00:00]",
            "",
            @"C:\>dir",
            " Volume in drive C is FREEDOS_C95",
            " Volume Serial Number is 0E4F-19EB",
            @" Directory of C:\",
            "",
            "FDOS                <DIR> 08-26-04 6:23p",
            "AUTOEXEC BAT          435 08-26-04 6:24p",
            "BOOTSECT BIN          512 08-26-04 6:23p",
            "COMMAND  COM       93,963 08-26-04 6:24p",
            "CONFIG   SYS          801 08-26-04 6:24p",
            "FDOSBOOT BIN          512 08-26-04 6:24p",
            "KERNEL   SYS       45,815 04-17-04 9:19p",
            "TERMINAL EXE      224,455 09-19-24 6:00p",
            "         7 file(s)     366,493 bytes",
            "         1 dir(s) 1,064,517,632 bytes free",
            "",
            @"C:\>",
        ];
    }

    public static void ShowRectangle(int offsetX, int offsetY, int width, int height)
    {
        width--;
        height--;

        for (int i = 1; i < width; i++)
        {
            WriteChar(offsetX + i, offsetY, A9);
            WriteChar(offsetX + i, offsetY + height, A9);
        }
        for (int i = 1; i < height; i++)
        {
            WriteChar(offsetX, offsetY + i, IV);
            WriteChar(offsetX + width, offsetY + i, IV);
        }
        WriteChar(offsetX, offsetY, A11);
        WriteChar(offsetX, offsetY + height, A4);
        WriteChar(offsetX + width, offsetY, A3);
        WriteChar(offsetX + width, offsetY + height, A10);
    }

    public static void ShowLinesOfCode(int offsetX, int offsetY, int count)
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

    private static void Write(string text, int delay)
    {
        if (string.IsNullOrEmpty(text)) return;

        var count = 1;
        var cc = FOREGROUND_COLOR;

        var res = FindKeyWord(ref count, ref cc, text)
            || FindClassName(ref count, ref cc, text)
            || FindFunctionName(ref count, ref cc, text)
            || FindSpecial(ref count, ref cc, text);

        Console.ForegroundColor = cc;

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
        Write(text[count..], delay);
    }

    public static void AnimateTextLine(int x, int y, string[] text, int delay)
    {
        var posY = y;
        foreach (var line in text)
        {
            var posX = x;
            Console.SetCursorPosition(posX, posY++);
            Write(line, delay);
        }
    }

    public static ConsoleKey AskMessage(string text, bool isReadKey = true, int delay = 2000)
    {
        ClearConsole();
        var posX = (Console.WindowWidth - text.Length + 2) / 2;
        var posY = (Console.WindowHeight - 3) / 2;
        ShowRectangle(posX, posY, text.Length + 2, 3);
        AnimateText(posX + 1, posY + 1, [text], 5);
        if (!isReadKey) 
        {
            Thread.Sleep(delay);
            return ConsoleKey.None;
        }
        return Console.ReadKey(true).Key;
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
        => FindWord(FOREGROUND_COLOR_KEYWORD, keyWords, text, ref count, ref consoleColor);
    private static bool FindClassName(ref int count, ref ConsoleColor consoleColor, string text)
        => FindWord(FOREGROUND_COLOR_CLASSES, classes, text, ref count, ref consoleColor);
    private static bool FindFunctionName(ref int count, ref ConsoleColor consoleColor, string text)
        => FindWord(FOREGROUND_COLOR_FUNCTION, functions, text, ref count, ref consoleColor);

    private static bool FindSpecial(ref int count, ref ConsoleColor consoleColor, string text)
        => FindWord(FOREGROUND_COLOR_SPECIAL, special, text, ref count, ref consoleColor);

    public static string[] keyWords = ["var", "public", "private", "protected", "int", "string", "double", 
         "this", "while", "new", "readonly",  "base", "virtual", "override", "get", "set", "void", "class", "{", "}"];
    public static string[] functions = ["[", "]", "WriteLine", "IComparable", "CompareTo", "Do", "AddAll", "Add", "ToString"];
    public static string[] classes = ["Console", "Amplifier", "BaseStorage", "ItemsStorage", "IEnumerable", "List", "Ints", "HString", "StringBuilder"];
    public static string[] special = ["(", ")",  "return", "yield", "=", "foreach", ">", "<", " in "];

    private static int WriteCode(int posX, int posY, string[] code)
    {
        ShowRectangle(posX, posY++, code.Max(x=>x.Length) + 5 + code.Length.ToString().Length, code.Length + 2);
        ShowLinesOfCode(posX + 1, posY, code.Length);
        AnimateTextLine(posX + 3 + code.Length.ToString().Length, posY++, code, 1);
        return posY;
    }

    public static int Write(this IExercise exercise)
    {
        var posy = 0;
        ClearConsole();
        AnimateText(1, posy++, [$"Задание #{exercise.Number}"], 50);
        posy++;
        AnimateText(1, posy++, [$"Что будет выведено на консоль?"], 50);
        posy = WriteCode(1, posy, exercise.Code);                        
        if (exercise.Variants.Length > 0)
        {            
            AnimateTextLine(1, posy++ + exercise.Code.Length + 2, ["Варианты ответов:"], 50);
            AnimateTextLine(1, posy++ + exercise.Code.Length + 2, exercise.Variants, 5);
        }
        posy += exercise.Code.Length + exercise.Variants.Length + 2;
        
        AnimateTextLine(1, posy++, [], 50); 
        var answer = InputWait(1, posy, exercise.NeedTime);  
        return exercise.Check(answer) ? 1 : 0;
    }

    public static string InputWait(int posX, int posY, TimeSpan period)
    {
        ConsoleKeyInfo cki;
        var dt = DateTime.Now;
        var sb = new StringBuilder(10);
        var sb2 = new StringBuilder(10);
        WriteString(posX, posY, "Осталось времени:");
        WriteString(posX, posY + 1, period.ToString(@"mm\:ss"));
        WriteString(posX, posY + 2, "Ваш ответ: ");        
        do 
        {                        
            while (Console.KeyAvailable == false)
            {
                var rest = period - (DateTime.Now - dt);
                WriteString(posX, posY + 1, rest.ToString(@"mm\:ss"));    
                
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
                WriteString(posX, posY + 3, sb2.ToString());
                sb2.Clear();                
            }           
            
            WriteString(posX, posY + 3, sb.ToString());
        } 
        while(cki.Key != ConsoleKey.Enter );
        return sb.ToString();
    }
}
