namespace QrCodeGenerator;

/// <summary>
/// QR Code encoding Type of data
/// </summary>
public enum EncodedType : byte
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
