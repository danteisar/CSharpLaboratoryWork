namespace QrCodeGenerator;

public class QrCode(string text, EncodingMode? encodingMode = null, EccLevel? correctionLevel = null, QR qrCodeVersion = QR.V1, Mask? maskNum = null)
{   
    public string Text { get; } = text;
    public EncodingMode EncodingMode => encodingMode ?? EncodingMode.Binary;
    public QR Version => qrCodeVersion;
    public EccLevel CorrectionLevel => correctionLevel ?? EccLevel.L;
    public Mask Mask => maskNum ?? Mask.M111;
    public string Code { get; } = QrCodeBuilder.GetQrCode(text, ref qrCodeVersion, ref encodingMode, ref correctionLevel, ref maskNum);
    public override string ToString() => Code;

    public static bool IsDemo {get => QrCodeBuilder.IsDemo; set=> QrCodeBuilder.IsDemo = value;}
    public static bool IsDataDemo {get => QrCodeBuilder.IsDataDemo; set=> QrCodeBuilder.IsDataDemo = value;}
    public static bool IsUtf8 {get => QrCodeBuilder.IsUtf8; set=> QrCodeBuilder.IsUtf8 = value;}
    public static bool IsInvert {get => QrCodeBuilder.IsInvert; set=> QrCodeBuilder.IsInvert = value;}
    public static bool ShowMaskOnDataOnly {get => QrCodeBuilder.ShowMaskOnDataOnly; set=> QrCodeBuilder.ShowMaskOnDataOnly = value;}
    public static bool ShowMask {get => QrCodeBuilder.ShowMask; set=> QrCodeBuilder.ShowMask = value;}
    public static bool ShowMagicData {get => QrCodeBuilder.ShowMagicData; set=> QrCodeBuilder.ShowMagicData = value;}
    public static bool PutCorrectionBlock {get => QrCodeBuilder.PutCorrectionBlock; set=> QrCodeBuilder.PutCorrectionBlock = value;}
    
    /// <inheritdoc cref="QrCodeBuilder.CharTemplate"/>
    public static string CharTemplate {get => QrCodeBuilder.CharTemplate; set=> QrCodeBuilder.CharTemplate = value;}
}