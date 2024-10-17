namespace QrCodeGenerator;

public enum Mask : byte
{
    /// <summary>
    /// ■■■ 000
    /// </summary>
    M0,
    /// <summary>
    /// ■■_ 001
    /// </summary>
    M1,
    /// <summary>
    /// ■_■ 010
    /// </summary>
    M2,
    /// <summary>
    /// ■__ 011
    /// </summary>
    M3,
    /// <summary>
    /// _■■ 100
    /// </summary>
    M4,
    /// <summary>
    /// _■_ 101
    /// </summary>
    M5,
    /// <summary>
    /// __■ 110
    /// </summary>
    M6,
    /// <summary>
    /// ___ 111
    /// </summary>
    M7    
}
