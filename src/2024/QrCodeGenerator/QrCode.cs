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
public class QrCode(string text, EncodingMode encodingMode = EncodingMode.Binary, EccLevel? correctionLevel = null, QR qrCodeVersion = QR.V1, Mask? maskNum = null)
{
   
    public string Text { get; } = text;
    public EncodingMode EncodingMode => encodingMode;
    public QR Version => qrCodeVersion;
    public EccLevel CorrectionLevel => correctionLevel ?? EccLevel.L;
    public Mask Mask => maskNum ?? Mask.M111;
    public string Code { get; } = QrCodeBuilder.GetQrCode(text, ref qrCodeVersion, encodingMode, ref correctionLevel, ref maskNum);
    public override string ToString() => Code;

    public static bool IsDemo {get => QrCodeBuilder.IsDemo; set=> QrCodeBuilder.IsDemo = value;}
    public static bool IsUtf8 {get => QrCodeBuilder.IsUtf8; set=> QrCodeBuilder.IsUtf8 = value;}
    public static bool IsInvert {get => QrCodeBuilder.IsInvert; set=> QrCodeBuilder.IsInvert = value;}
}