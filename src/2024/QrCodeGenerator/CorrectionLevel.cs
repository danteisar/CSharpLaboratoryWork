namespace QrCodeGenerator;

/// <summary>
/// Error Correction Level
/// </summary>
public enum CorrectionLevel : byte
{
    /// <summary>
    /// 7 % Low
    /// </summary>
    L,
    /// <summary>
    /// 15 % Medium
    /// </summary>
    M,
    /// <summary>
    /// 25 % Quartile
    /// </summary>
    Q,
    /// <summary>
    /// 30 % High
    /// </summary>
    H
}
