using Barcode.Lab1;
using Lab1.Abstractions;
using QrCodeGenerator;

namespace Lab1.Codes;

/// <summary>
/// Проверка:
/// <list type="string">
/// <item>1. Инкапсуляция</item>
/// <item>2. Аттрибуты</item>
/// <item>3. Свойства</item>
/// <item>4. Связь между текстом и кодом</item>
/// <item>5. Отсутсвие открытых методов</item>
/// <item>6. Перегрузка к строке</item>
/// </list>
/// </summary>
public class ProductCode : IProductCode
{
    private string _text;
    private string _qrcode;
    private string _bardoe;
    private int _paddings;

    private QR _qr = QR.V1;
    private EncodingMode? _encodingMode = EncodingMode.Binary;
    private EccLevel? _eccLevel = EccLevel.Q;
    private Mask? _mask = null;

    public string QrCode => _qrcode;
    public string Barcode => _bardoe;

    public static OutputMode OutputMode { get; set; } = OutputMode.Text;

    public string Text
    {
        get => _text;
        set
        {
            if (string.Equals(_text, value)) return;
            _text = value;
            _bardoe = value.ToBarcode(out _paddings);
            _qrcode = value.ToQrCode(ref _qr, ref _encodingMode, ref _eccLevel, ref _mask);
        }
    }

    public override string ToString()
    {
        return OutputMode switch
        {
            OutputMode.Text => $"\t{_text}",
            OutputMode.QrCode => _qrcode,
            OutputMode.Barcode => _bardoe,
            _ => _qrcode + "\n" + Barcode + " ".PadRight(_paddings) + _text
        };
    }
}
