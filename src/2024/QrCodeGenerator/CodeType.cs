namespace QrCodeGenerator;

/// <summary>
/// QR Code Type
/// </summary>
public enum CodeType : byte
{
    /// <summary>
    /// Numbers only
    /// </summary>
    Numeric,
    /// <summary>
    /// Caps english + numbers
    /// </summary>
    AlphaNumeric,    
    /// <summary>
    /// Everything
    /// </summary>
    Binary,
    /// <summary>
    /// だってばよ
    /// </summary>
    Kanji
}
