namespace QrCodeGenerator;

public record QrCodeData
{
    public QR Version {get; init;}
    public CorrectionLevel CorrectionLevel {get; init;}
    public string Data {get; init;}
}
