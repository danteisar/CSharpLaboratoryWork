using static Barcode.BarcodeGenerator;

namespace Barcode;

/// <summary>
///     Штрих код
/// </summary>
public class Barcode
{
    public Barcode(string text, bool optimize = false)
    {
        _optimize = optimize;
        Text = text;
    }

    private string _text;
    private int _paddings;
    private readonly bool _optimize;

    public static BarcodeType Type { get; set; } = BarcodeType.Full;
    
    public string Code { get; private set; }

    public string Text
    {
        get => _text;
        set
        {
            if (_text == value) return;
            _text = value;  
            Code = _text.Encrypt(_optimize, out _paddings);
        }
    }
        
    public override string ToString()
    {
        return Type switch
        {
            BarcodeType.Barcode => Code,
            BarcodeType.Text => Text,
            BarcodeType.Full => Code + Text.PadLeft(_paddings, Bars[3]).PadRight(_paddings, Bars[3]),
            _ => string.Empty
        };
    }
}
