namespace QrCodeGenerator;

/// <summary>
/// Error Correction Level
/// </summary>
public enum EccLevel : byte
{
    /// <summary>
    /// 7 % Low черный-черный
    /// </summary>
    L  = 0, 
    /// <summary>
    /// 15 % Medium черный-белый
    /// </summary>
    M = 1,
    /// <summary>
    /// 25 % Quartile белый-черный
    /// </summary>
    Q = 2,
    /// <summary>
    /// 30 % High белый-белый
    /// </summary>
    H = 3
}
