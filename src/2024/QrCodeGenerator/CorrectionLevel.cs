namespace QrCodeGenerator;

/// <summary>
/// Error Correction Level
/// </summary>
public enum EccLevel : byte
{
    /// <summary>
    /// 7 % Low 00
    /// </summary>
    L  = 0, 
    /// <summary>
    /// 15 % Medium 01
    /// </summary>
    M = 1,
    /// <summary>
    /// 25 % Quartile 10
    /// </summary>
    Q = 2,
    /// <summary>
    /// 30 % High 11
    /// </summary>
    H = 3
}
