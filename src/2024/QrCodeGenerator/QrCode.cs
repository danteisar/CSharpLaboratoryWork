namespace QrCodeGenerator;

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
public class QrCode(string text, EncodedType codeType = EncodedType.Binary, QR qrCodeVersion = QR.V1, CorrectionLevel? correctionLevel = null, int? maskNum = null, bool invert = false)
{
   
    public string Text { get; } = text;
    public EncodedType CodeType => codeType;
    public QR Version => qrCodeVersion;
    public CorrectionLevel CorrectionLevel => correctionLevel ?? CorrectionLevel.L;
    public string Code { get; } = QrCodeBuilder2.GetQrCode(text, ref qrCodeVersion, codeType, ref correctionLevel, maskNum, invert);
    public override string ToString() => Code;
}