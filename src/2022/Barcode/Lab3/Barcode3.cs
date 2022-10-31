using Barcode.Lab1;
using static Barcode.Lab1.BarcodeGenerator;

namespace Barcode.Lab3;

public record Barcode3(string Text) : IBarcode3
{
    private readonly int _paddings = Text.Encrypt(false);
    public string Code { get; init; } = Text.Encrypt(false, out _);
    public static BarcodeType Type { get; set; } = BarcodeType.Full;

    public static implicit operator Barcode3(string text) => new(text);
    public override string ToString()
    {
        return Type switch
        {
            BarcodeType.Barcode => Code,
            BarcodeType.Text => Text,
            BarcodeType.Full => Code + $"* {Text} *".PadLeft(_paddings-2, Bars[3]).PadRight(_paddings-2, Bars[3]),
            _ => string.Empty
        };
    }
}