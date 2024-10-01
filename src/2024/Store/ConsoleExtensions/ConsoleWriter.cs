using Barcode.Lab1;
using Barcode.Lab3;
using Store.ConsoleWrappers;
using static Store.Constants;

namespace Store.ConsoleExtensions;

internal static class ConsoleWriter
{
    #region Colors

    public static ConsoleColor BACKGROUND_COLOR = ConsoleColor.White;
    public static ConsoleColor FOREGROUND_COLOR = ConsoleColor.Black;

    public static ConsoleColor COLOR = ConsoleColor.DarkGreen;
    public static ConsoleColor ACTIVE_COLOR = ConsoleColor.DarkGreen;
    public static ConsoleColor HELP_COLOR = ConsoleColor.DarkRed;
    public static ConsoleColor ERROR_COLOR = ConsoleColor.Red;
    public static ConsoleColor ERROR_FOREGROUND_COLOR = ConsoleColor.White;
    public static ConsoleColor BORDER_COLOR = ConsoleColor.Black;
    
    public static ConsoleColor STORE1 = ConsoleColor.Magenta;    
    public static ConsoleColor STORE2 = ConsoleColor.Red;
    public static ConsoleColor STORE3 = ConsoleColor.Black;
    
    #endregion
   
    public static void SetColors(bool isDark)
    {
        if (isDark)
        {
            BACKGROUND_COLOR = ConsoleColor.Black;
            FOREGROUND_COLOR = ConsoleColor.White;
            COLOR = ConsoleColor.Green;
            ACTIVE_COLOR = ConsoleColor.Green;
            HELP_COLOR = ConsoleColor.Red;
            ERROR_COLOR = ConsoleColor.DarkRed;
            BORDER_COLOR = ConsoleColor.White;
            STORE1 = ConsoleColor.Magenta;
            STORE2 = ConsoleColor.Red;
            STORE3 = ConsoleColor.White;
        }
        else
        {
            BACKGROUND_COLOR = ConsoleColor.White;
            FOREGROUND_COLOR = ConsoleColor.Black;
            COLOR = ConsoleColor.DarkGreen;
            ACTIVE_COLOR = ConsoleColor.DarkGreen;
            HELP_COLOR = ConsoleColor.DarkRed;
            ERROR_COLOR = ConsoleColor.Red;
            BORDER_COLOR = ConsoleColor.Black;            
            STORE1 = ConsoleColor.Magenta;    
            STORE2 = ConsoleColor.Red;
            STORE3 = ConsoleColor.Black;
        }
    }
    
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

    public static void ClearConsole(ConsoleColor? consoleColor = null, ConsoleColor? foreground = null)
    {
        Console.BackgroundColor = consoleColor ?? BACKGROUND_COLOR;
        Console.ForegroundColor = foreground ?? FOREGROUND_COLOR;
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
        Barcode1 logo = "TERMINAL v.1.1";
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
        for (int i = 0; i < text.Max(x=>x.Length); i++)
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

    public static void ShowHorizontalBorder(int offsetX, int offsetY, int width)
    {
        width--; 
        for (int i = 1; i < width; i++)
        {
            WriteChar(offsetX + i, offsetY, A9);
        }        
        WriteChar(offsetX, offsetY, A7);
        WriteChar(offsetX + width, offsetY, A2);
    }

    public static void ShowVerticalBorder(int offsetX, int offsetY, int height)
    {
        height--; 
        for (int i = 1; i < height; i++)
        {
            WriteChar(offsetX, offsetY + i, IV);
        }        
        WriteChar(offsetX, offsetY, A6);
        WriteChar(offsetX, offsetY + height, A5);
    }
    
    private static void WriteText(string text, int delay){
        Console.Write(text);
        Thread.Sleep(delay);
    }
    
    public static void AnimateTextLine(int x, int y, string[] text, int delay)
    {
        var posY = y;
        foreach (var line in text)
        {
            var posX = x;
            Console.SetCursorPosition(posX, posY++);
            WriteText(line, delay);
        }
    }
    
    public static ConsoleKeyInfo ErrorMessage(string[] text, params ConsoleKey[] keys)
    {        
        ConsoleKeyInfo key;
        do   
        {
            key = ShowMessage(text, true, ERROR_COLOR, ERROR_FOREGROUND_COLOR);
        }
        while (keys.Length > 0 && !keys.Contains(key.Key));
        return key;
    }

    public static ConsoleKeyInfo AskMessage(string[] text, params ConsoleKey[] keys)
    {
        ConsoleKeyInfo key;
        do   
        {
            key = ShowMessage(text, true, BACKGROUND_COLOR, FOREGROUND_COLOR);
        }
        while (keys.Length > 0 && !keys.Contains(key.Key));
        return key;
    }

    public static bool AskMessage(string[] text)
    {
        text[^1] += " (y/n)";
        return AskMessage(text, ConsoleKey.Y, ConsoleKey.N).Key == ConsoleKey.Y;
    }

    public static ConsoleKeyInfo ShowMessage(string[] text, bool isReadKey, ConsoleColor consoleColor, ConsoleColor foreground, int delay = 2000)
    {
        ClearConsole(consoleColor, foreground);
        var posX = (Console.WindowWidth - text.Max(x=>x.Length) + 2) / 2;
        var posY = (Console.WindowHeight - text.Length + 2) / 2;
        ShowRectangle(posX, posY,  text.Max(x=>x.Length) + 2, text.Length + 2);
        AnimateText(posX + 1, posY + 1, text, 5);
        if (!isReadKey) 
        {
            Thread.Sleep(delay);
            return default;
        }
        return Console.ReadKey(true);
    }
}
