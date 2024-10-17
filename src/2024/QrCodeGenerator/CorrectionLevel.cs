namespace QrCodeGenerator;

/// <summary>
/// Error Correction Level
/// </summary>
public enum EccLevel : byte
{
    /// <summary>
    /// 7 % Low 00
    /// </summary>
    L, 
    /// <summary>
    /// 15 % Medium 01
    /// </summary>
    M,
    /// <summary>
    /// 25 % Quartile 10
    /// </summary>
    Q,
    /// <summary>
    /// 30 % High 11
    /// </summary>
    H
}
