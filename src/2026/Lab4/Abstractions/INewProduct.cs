using Lab2.Abstractions;
using Lab4.Events;

namespace Lab4.Abstractions;

/// <summary>
/// Новый продукт
/// </summary>
public interface INewProduct : IProduct
{
    public event EventHandler<ProductEventArgs> DataChanged;
}