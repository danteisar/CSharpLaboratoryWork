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

public class QrCode(string text, CodeType codeType = CodeType.Binary, byte qrCodeVersion = 1)
{
    private string Text { get; } = text;
    private string Code { get; } = QrCodeBuilder2.GetQrCode(text, qrCodeVersion, codeType);
    public override string ToString() => Code;
}