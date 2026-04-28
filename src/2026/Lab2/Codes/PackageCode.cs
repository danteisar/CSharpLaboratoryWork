using Lab1.Abstractions;
using Lab1.Codes;

namespace Lab2.Codes;

public sealed record PackageCode : IProductCode
{
    private readonly ProductCode _productCode = new();

    public static OutputMode OutputMode
    {
        get => ProductCode.OutputMode;
        set => ProductCode.OutputMode = value;
    }

    public string QrCode => _productCode.QrCode;

    public string DataMatrix => _productCode.DataMatrix;

    public string Text
    {
        get => _productCode.Text;
        set => _productCode.Text = value;
    }

    private string ProductText => $"\t* {_productCode.Text} *";

    public override string ToString()
    {
        return OutputMode switch
        {
            OutputMode.Text => ProductText,
            OutputMode.QrCode => _productCode.QrCode,
            OutputMode.DataMatrix => _productCode.DataMatrix,
            _ => QrCode + "\n" + DataMatrix + " ".PadRight(5) + ProductText
        };
    }
}