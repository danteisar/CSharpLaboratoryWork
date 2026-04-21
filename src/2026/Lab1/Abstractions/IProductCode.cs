using Lab1.Codes;

namespace Lab1.Abstractions;

/// <summary>
/// Базовый код
/// </summary>
public interface IProductCode
{
    /// <summary>
    /// Способ формирования кода, можно реализовать по умолчанию
    /// </summary>
    static abstract OutputMode OutputMode { get; set; }

    /// <summary>
    /// QR-код
    /// </summary>
    string QrCode { get; }
    
    /// <summary>
    /// Штрихкод
    /// </summary>
    string Barcode { get; }

    /// <summary>
    /// Исходный текст
    /// </summary>
    string Text { get; set; }
}