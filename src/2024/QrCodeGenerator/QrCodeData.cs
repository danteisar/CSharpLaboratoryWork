namespace QrCodeGenerator;

public record QrCodeData
{
    public byte Version {get; init;}
    public CorrectionLevel CorrectionLevel {get; init;}
    public string Data {get; init;}
}
