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
public class QrCode(string text, CodeType codeType = CodeType.Binary, QR qrCodeVersion = QR.V1, CorrectionLevel? correctionLevel = null, int? maskNum = null, bool invert = false)
{
    public string Text { get; } = text;
    public string Code { get; } = QrCodeBuilder2.GetQrCode(text, qrCodeVersion, codeType, correctionLevel, maskNum, invert);
    public override string ToString() => Code;
}