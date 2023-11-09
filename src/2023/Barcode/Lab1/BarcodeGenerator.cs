using System;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Barcode.Lab1;

public static class BarcodeGenerator
{
    /// <summary>
    ///  Кодирование текста в штрих-код
    /// </summary>
    public static string Encrypt(this string code, out int paddings)
    {      
        var tmp = code.Parse();           
                
        var result = new StringBuilder();
        foreach (var s1 in tmp.ToString().SplitText(2)) 
            result.Append(GetBar(s1));

        paddings = result.Length;

        var empty = "\n".PadLeft(paddings + 1, Bars[0]);

        var s = new StringBuilder((result.Length+1)*Height + 2 * (paddings + 2));
        for (var i = 0; i < Height; i++)
        {
            s.Append(result).AppendLine();
        }

        paddings = empty.Length / 2 + code.Length / 2;

        return s.Insert(0, empty).Append(empty).ToString();
    }

    /// <summary>
    /// Узнать ширину отступа
    /// </summary>
    public static int Encrypt(this string code)
    {
        code.Encrypt(out var paddings);
        return paddings;
    }

    private static StringBuilder Parse(this string text)
    {
        var index = 0;
        var tmp = new StringBuilder(Frame);
        var check = new List<int>();
        var isDigitMode = text.Length > 1 && Checknum(text, index, 2);

        #region Patterns  
        
        AddPattern(tmp, check, isDigitMode ? StartNumbers : StartText);
        while (index < text.Length)
        {
            AddPattern(text, tmp, check, ref isDigitMode, ref index);
        }
        AddPattern(tmp, check, Checksum(check));
        AddPattern(tmp, check, Stop);

        #endregion

        tmp.Append(Frame);
        if (tmp.Length % 2 > 0)
            tmp.Append('0');

        return tmp;
    }

    #region Help

    private static void AddPattern(string text, StringBuilder tmp, IList<int> check, ref bool isDigitMode, ref int index)
    {
        if ((isDigitMode && !Checknum(text, index, 2)) ||
               (!isDigitMode && Checknum(text, index, 4)))
        {
            isDigitMode = !isDigitMode;
            AddPattern(tmp, check, isDigitMode ? SwitchToNumbers : SwitchToText);
        }
        AddPattern(ref index, check, isDigitMode, text, tmp);
    }

    private static void AddPattern(ref int index, IList<int> check, bool isDigitMode, string text, StringBuilder tmp)
    {
        if (isDigitMode)
        {
            AddPattern(tmp, check, Array.IndexOf(NumberSymbols, text.Substring(index, 2)));
            index+=2;
        }
        else
        {
            AddPattern(tmp, check, Array.IndexOf(TextSymbols, text.Substring(index, 1)));
            index++;
        }
    }

    private static void AddPattern(StringBuilder tmp, IList<int> check, int number)
    {
        tmp.Append(Patterns[number]);
        check.Add(number);
    }

    private static bool Checknum(string text, int skip, int take)
    {
        var chars = text.Skip(skip).Take(take);
        return chars.Count() == take && chars.All(x=>char.IsDigit(x));
    }    

    private static int Checksum(IList<int> check)
    {
        var sum = check[0];

        for (var i = 1; i < check.Count; i++)
        {
            sum += i * check[i];
        }

        sum %= 103;

        return sum;
    }    

    private static char GetBar(string code) => Bars[Convert.ToInt32(code, 2)];

    private static IEnumerable<string> SplitText(this string text, int chunkSize)
    {
        return Enumerable.Range(0, text.Length / chunkSize)
            .Select(i => text.Substring(i * chunkSize, chunkSize));
    }

    /*

    ████████████████████████████████████████████████████████████
    ██ ▌█▐█▌█▌▐▐█  █▐▌▌█▐ ██  ▌ ▌▌▌█  █ █▐▐█▌▌▐▌██▐ ▐  ▌▐█ ▐▐ ██
    ██ ▌█▐█▌█▌▐▐█  █▐▌▌█▐ ██  ▌ ▌▌▌█  █ █▐▐█▌▌▐▌██▐ ▐  ▌▐█ ▐▐ ██
    ██ ▌█▐█▌█▌▐▐█  █▐▌▌█▐ ██  ▌ ▌▌▌█  █ █▐▐█▌▌▐▌██▐ ▐  ▌▐█ ▐▐ ██
    ██ ▌█▐█▌█▌▐▐█  █▐▌▌█▐ ██  ▌ ▌▌▌█  █ █▐▐█▌▌▐▌██▐ ▐  ▌▐█ ▐▐ ██
    ████████████████████████████████████████████████████████████
                          Example
    */

    /// <summary>   
    ///  Высота штрихкода (в строках)
    /// </summary>
    private const int Height = 4;

    /// <summary>
    /// Для получения рамки штрихкода по краям
    /// </summary>
    private const string Frame = "0000";

    /// <summary>
    ///    Допустимые варианты штрихов
    /// </summary>
    public static readonly char[] Bars = { '█', '▌', '▐', ' ' };

    /// <summary>
    ///     Начальный номер паттерна для текстовой строки
    /// </summary>
    private const int StartText = 104;

    /// <summary>
    ///     Начальный номер паттерна для числовой строки
    /// </summary>
    private const int StartNumbers = 105;    

    /// <summary>
    ///     Переключить в числовой режим кодирования
    /// </summary>
    private const int SwitchToNumbers = 99;

    /// <summary>
    ///     Переключить в текстовый режим кодирования
    /// </summary>
    private const int SwitchToText = 100;

    /// <summary>
    ///     Номер паттерна завершения
    /// </summary>
    private const int Stop = 108;

    /// <summary>
    ///     Доступные паттерны
    /// </summary>
    private static readonly string[] Patterns = {
        "11011001100", "11001101100", "11001100110", "10010011000", "10010001100",
        "10001001100", "10011001000", "10011000100", "10001100100", "11001001000",
        "11001000100", "11000100100", "10110011100", "10011011100", "10011001110",
        "10111001100", "10011101100", "10011100110", "11001110010", "11001011100",
        "11001001110", "11011100100", "11001110100", "11101101110", "11101001100",
        "11100101100", "11100100110", "11101100100", "11100110100", "11100110010",
        "11011011000", "11011000110", "11000110110", "10100011000", "10001011000",
        "10001000110", "10110001000", "10001101000", "10001100010", "11010001000",
        "11000101000", "11000100010", "10110111000", "10110001110", "10001101110",
        "10111011000", "10111000110", "10001110110", "11101110110", "11010001110",
        "11000101110", "11011101000", "11011100010", "11011101110", "11101011000",
        "11101000110", "11100010110", "11101101000", "11101100010", "11100011010",
        "11101111010", "11001000010", "11110001010", "10100110000", "10100001100",
        "10010110000", "10010000110", "10000101100", "10000100110", "10110010000",
        "10110000100", "10011010000", "10011000010", "10000110100", "10000110010",
        "11000010010", "11001010000", "11110111010", "11000010100", "10001111010",
        "10100111100", "10010111100", "10010011110", "10111100100", "10011110100",
        "10011110010", "11110100100", "11110010100", "11110010010", "11011011110",
        "11011110110", "11110110110", "10101111000", "10100011110", "10001011110",
        "10111101000", "10111100010", "11110101000", "11110100010", "10111011110",
        // 100+
        "10111101110", "11101011110", "11110101110", "11010000100", "11010010000",
        "11010011100", "11000111010", "11010111000", "1100011101011"};
    /// <summary>
    ///     Разрешенные символы
    /// </summary>
    private static readonly string[] TextSymbols = {
        " ","!","\"","#","$","%","&","'","(",")",
        "*","+",",","-",".","/","0","1","2","3",
        "4","5","6","7","8","9",":",";","<","=",
        ">","?","@","A","B","C","D","E","F","G",
        "H","I","J","K","L","M","N","O","P","Q",
        "R","S","T","U","V","W","X","Y","Z","[",
        "\\","]","^","_","`","a","b","c","d","e",
        "f","g","h","i","j","k","l","m","n","o",
        "p","q","r","s","t","u","v","w","x","y",
        "z","{","|","|","~"
    };
    /// <summary>
    ///     Разрешенные пары числовых строк
    /// </summary>
    private static readonly string[] NumberSymbols = {
        "00","01","02","03","04","05","06","07","08","09",
        "10","11","12","13","14","15","16","17","18","19",
        "20","21","22","23","24","25","26","27","28","29",
        "30","31","32","33","34","35","36","37","38","39",
        "40","41","42","43","44","45","46","47","48","49",
        "50","51","52","53","54","55","56","57","58","59",
        "60","61","62","63","64","65","66","67","68","69",
        "70","71","72","73","74","75","76","77","78","79",
        "80","81","82","83","84","85","86","87","88","89",
        "90","91","92","93","94","95","96","97","98","99"
    };

    //  █▀▀▀▀▀█ ▀█▄█▄ █▀▀▀▀▀█
    //  █ ███ █ ▄▀ ▄  █ ███ █
    //  █ ▀▀▀ █ █▀▄▄▀ █ ▀▀▀ █
    //  ▀▀▀▀▀▀▀ █▄▀▄█ ▀▀▀▀▀▀▀
    //  █ ▄ ▀▄▀▀▀ ▄█▄▀▀▀▀█▄▄▀
    //  █▄█▀█ ▀▄▀█ ▀ ▄█▀█  ▀▄
    //  ▀  ▀  ▀▀█▀▀ ███▀▄ ▄▄█
    //  █▀▀▀▀▀█ ▀▄▄▄█▀ ▄▀  █▄
    //  █ ███ █ ▀  █▄ ▀▄▄█▄▄█
    //  █ ▀▀▀ █  █▄▀ ▄█ █▀   
    //  ▀▀▀▀▀▀▀ ▀ ▀ ▀▀▀▀ ▀  ▀

    #endregion
}