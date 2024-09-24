namespace Store;

internal class Constants
{
#if DEBUG
    public const ConsoleColor BACKGROUND_COLOR = ConsoleColor.Black;
    public const ConsoleColor FOREGROUND_COLOR = ConsoleColor.White;

    public const ConsoleColor COLOR = ConsoleColor.Green;
    public const ConsoleColor ACTIVE_COLOR = ConsoleColor.Green;
    public const ConsoleColor HELP_COLOR = ConsoleColor.Red;
    public const ConsoleColor BORDER_COLOR = ConsoleColor.White;
    
    public const ConsoleColor STORE1 = ConsoleColor.Magenta;
    public const ConsoleColor STORE2 = ConsoleColor.Red;
    public const ConsoleColor STORE3 = ConsoleColor.White;
#else
    public const ConsoleColor BACKGROUND_COLOR = ConsoleColor.White;
    public const ConsoleColor FOREGROUND_COLOR = ConsoleColor.Black;

    public const ConsoleColor COLOR = ConsoleColor.DarkGreen;
    public const ConsoleColor ACTIVE_COLOR = ConsoleColor.DarkGreen;
    public const ConsoleColor HELP_COLOR = ConsoleColor.DarkRed;
    public const ConsoleColor BORDER_COLOR = ConsoleColor.Black;
    
    public const ConsoleColor STORE1 = ConsoleColor.Magenta;    
    public const ConsoleColor STORE2 = ConsoleColor.Red;
    public const ConsoleColor STORE3 = ConsoleColor.Black;
#endif

    public const char OPERATOR = '*';
    public const int HEIGHT = 30;
    public const int FIELD_HEIGHT = 10;

    public const char EMPTY = ' ';

    public const char TOP_LEFT_BORDER = '╔';
    public const char CROSS_LEFT_BORDER = '╠';
    public const char BOTTOM_LEFT_BORDER = '╚';
    public const char TOP_RIGHT_BORDER = '╗';
    public const char CROSS_RIGHT_BORDER = '╣';
    public const char BOTTOM_RIGHT_BORDER = '╝';
    public const char HORIZONTAL_BORDER = '═';
    public const char VERTICAL_BORDER = '║';
    public const char CROSS_TOP_BORDER = '╦';
    public const char CROSS_BORDER = '╬';
    public const char CROSS_BOTTOM_BORDER = '╩';

    public const char TV = '╤';
    public const char IV = '│';
    public const char LV = '╧';

    public const char A2 = '┤';
    public const char A3 = '┐';
    public const char A4 = '└';
    public const char A5 = '┴';
    public const char A6 = '┬';
    public const char A7 = '├';
    public const char A8 = '┼';
    public const char A9 = '─';
    public const char A10 = '┘';
    public const char A11 = '┌';
}
