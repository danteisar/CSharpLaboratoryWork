using System.Text;

namespace Barcode.Lab1;

public static class BarcodeGenerator
{
    /// <summary>
    ///  КЕодирование текста в штрих-код
    /// </summary>
    /// <param name="code"></param>
    /// <param name="optimize"></param>
    /// <param name="paddings"></param>
    /// <returns></returns>
    public static string Encrypt(this string code, bool optimize, out int paddings)
    {
        var tmp = code.Parse(optimize);

        if (tmp.Length % 2 > 0)
            tmp += "0";

        var result = new StringBuilder();
        foreach (var s1 in tmp.SplitText(2)) 
            result.Append(GetBar(s1));

        paddings = result.Length;
        var empty = "\n".PadLeft(paddings + 1, Bars[0]);

        var s = new StringBuilder();
        for (var i = 0; i < Height; i++)
        {
            s.Append(result).AppendLine();
        }

        paddings = empty.Length / 2 + code.Length / 2;

        return s.Insert(0, empty).Append(empty).ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <param name="optimize"></param>
    /// <returns></returns>
    public static int Encrypt(this string code, bool optimize)
    {
        code.Encrypt(optimize, out var paddings);
        return paddings;
    }

    private static string Parse(this string text, bool optimize)
    {
        List<int> values = new();

        if (int.TryParse(text, out _) && text.Length % 2 == 0)
            return text.Parse(StartWithNumber, GetTypeNumbers, 2, values);

        return optimize
            ? CheckMode(ref text, values)
            : text.Parse(StartWithText, GetTypeText, 1, values);
    }

    private static int NumberPairs(string text)
    {
        var cnt = text
            .SplitText(2)
            .TakeWhile(tmp => int.TryParse(tmp, out _)).Count();
        return cnt;
    }


    private static string CheckMode(ref string text, IList<int> values, bool isStart = true)
    {
        if (string.IsNullOrEmpty(text)) return BuildChecksum(values) + Stop;

        string str;
        var cnt = NumberPairs(text);

        if (cnt > 1)
        {
            str = text[..(cnt * 2)];
            text = text.Remove(0, str.Length);
            str = isStart
                ? str.Parse(StartWithNumber, GetTypeNumbers, 2, values, false)
                : str.Parse(ChangeToNumbers, GetTypeNumbers, 2, values, false);
        }
        else
        {
            str = cnt == 1
                ? text[..2]
                : string.Empty;

            var q = cnt == 1
                ? text[2..]
                : text;

            foreach (var tmp in q.SplitText(2))
            {
                if (int.TryParse(tmp, out _)) break;
                str += tmp;
                cnt++;
            }

            var tmp2 = text[..str.Length];
            var rest = NumberPairs(tmp2);
            if (tmp2 != null && rest < 2)
            {
                str += text[(tmp2.Length + rest * 2)..];
            }
            text = text.Remove(0, str.Length);
            str = isStart
                ? str.Parse(StartWithText, GetTypeText, 1, values, false)
                : str.Parse(ChangeToText, GetTypeText, 1, values, false);
        }


        return str + CheckMode(ref text, values, false);
    }

    private static string Parse(this string text, Func<IList<int>, string> start, Func<string, IList<int>, string> getType, int chunkSize,
        IList<int> values, bool isFull = true)
    {
        var tmp = new StringBuilder(start(values));
        foreach (var s in text.SplitText(chunkSize)) tmp.Append(getType(s, values));

        if (!isFull) return tmp.ToString();

        tmp.Append(BuildChecksum(values)).Append(Stop);

        return tmp.ToString();
    }

    private static IEnumerable<string> SplitText(this string text, int chunkSize)
    {
        return Enumerable.Range(0, text.Length / chunkSize)
            .Select(i => text.Substring(i * chunkSize, chunkSize));
    }

    private static string BuildChecksum(IList<int> check)
    {
        var sum = check[0];

        for (var i = 1; i < check.Count; i++)
        {
            sum += i * check[i];
        }

        sum %= 103;

        return Patterns[sum];
    }

    private static string GetTypeText(string text, IList<int> values)
    {
        return GetPattern(TextSymbols, text, values);
    }

    private static string GetTypeNumbers(string text, IList<int> values)
    {
        return GetPattern(NumberSymbols, text, values);
    }

    private static string GetPattern(string[] array, string text, ICollection<int> values)
    {
        var index = Array.IndexOf(array, text);
        values.Add(index);
        return Patterns[index];
    }

    private static string StartWithText(IList<int> values)
    {
        values.Add(104);
        return StartText;
    }

    private static string StartWithNumber(IList<int> values)
    {
        values.Add(105);
        return StartNumbers;
    }

    private static string ChangeToText(ICollection<int> values)
    {
        values.Add(100);
        return SwitchToText;
    }

    private static string ChangeToNumbers(ICollection<int> values)
    {
        values.Add(99);
        return SwitchToNumbers;
    }


    #region Help

    public static char GetBar(string code) => Bars[Convert.ToInt32(code, 2)];

    /*

██████████████████████████████████████████████████████████████
███ ▌█▐█▌█▌▐▐█  █▐▌▌█▐ ██  ▌ ▌▌▌█  █ █▐▐█▌▌▐▌██▐ ▐  ▌▐█ ▐▐ ███
███ ▌█▐█▌█▌▐▐█  █▐▌▌█▐ ██  ▌ ▌▌▌█  █ █▐▐█▌▌▐▌██▐ ▐  ▌▐█ ▐▐ ███
███ ▌█▐█▌█▌▐▐█  █▐▌▌█▐ ██  ▌ ▌▌▌█  █ █▐▐█▌▌▐▌██▐ ▐  ▌▐█ ▐▐ ███
███ ▌█▐█▌█▌▐▐█  █▐▌▌█▐ ██  ▌ ▌▌▌█  █ █▐▐█▌▌▐▌██▐ ▐  ▌▐█ ▐▐ ███
███ ▌█▐█▌█▌▐▐█  █▐▌▌█▐ ██  ▌ ▌▌▌█  █ █▐▐█▌▌▐▌██▐ ▐  ▌▐█ ▐▐ ███
██████████████████████████████████████████████████████████████
                            Example
    */

    /// <summary>
    ///  Высота штрихкода (в строках)
    /// </summary>
    private const int Height = 4;

    /// <summary>
    ///    Допустимые варианты штрихов
    /// </summary>
    public static readonly char[] Bars = { '█', '▌', '▐', ' ' };

    /// <summary>
    ///     Начальный паттерн для текстовой строки
    /// </summary>
    private const string StartText = "00000011010010000";

    /// <summary>
    ///     Начальный паттерн для числовой строки
    /// </summary>
    private const string StartNumbers = "00000011010011100";

    /// <summary>
    ///     Переключить в текстовый режим кодирования
    /// </summary>
    private const string SwitchToText = "10111101110";

    /// <summary>
    ///     Переключить в числовой режим кодирования
    /// </summary>
    private const string SwitchToNumbers = "10111011110";

    /// <summary>
    ///     Паттерн завершения
    /// </summary>
    private const string Stop = "1100011101011000000";

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