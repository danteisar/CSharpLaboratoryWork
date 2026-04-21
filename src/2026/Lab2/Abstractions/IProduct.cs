using Lab1.Abstractions;

namespace Lab2.Abstractions;

/// <summary>
/// Продукт
/// </summary>
public interface IProduct
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    long Id { get; set; }   
    /// <summary>
    /// Стоимость
    /// </summary>
    decimal Price { get; set; }
    /// <summary>
    /// Наименование
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Код
    /// </summary>
    IProductCode Code { get; }
    /// <summary>
    /// Тип продукта
    /// </summary>
    string Type { get; }
    /// <summary>
    /// Полезная информация
    /// </summary>
    string Information { get; }
}