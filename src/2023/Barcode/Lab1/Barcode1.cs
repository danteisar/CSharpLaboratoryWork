using static Barcode.Lab1.BarcodeGenerator;

namespace Barcode.Lab1;

/// <summary>
///     Штрих код
/// </summary>
public class Barcode1
{
    public Barcode1(string text)
    {      
        Text = text;
    }

    private string _text;
    private int _paddings;

    public static BarcodeType Type { get; set; } = BarcodeType.Full;

    public string Code { get; private set; }

    public string Text
    {
        get => _text;
        set
        {
            if (_text == value) return;
            _text = value;
            Code = _text.Encrypt(out _paddings);
        }
    }

    public static implicit operator Barcode1(string text) => new(text);


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
