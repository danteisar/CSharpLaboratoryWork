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
public class QrCode(string text, EncodingMode encodingMode = EncodingMode.Binary, EccLevel? correctionLevel = null, QR qrCodeVersion = QR.V1, Mask? maskNum = null, bool invert = false)
{
   
    public string Text { get; } = text;
    public EncodingMode EncodingMode => encodingMode;
    public QR Version => qrCodeVersion;
    public EccLevel CorrectionLevel => correctionLevel ?? EccLevel.L;
    public Mask Mask => maskNum ?? Mask.M2;
    public string Code { get; } = QrCodeBuilder.GetQrCode(text, ref qrCodeVersion, encodingMode, ref correctionLevel, ref maskNum, invert);
    public override string ToString() => Code;

    public static bool IsDEmo {get => QrCodeBuilder.IsDemo; set=> QrCodeBuilder.IsDemo = value;}
}