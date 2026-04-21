using Lab2.Abstractions;
using Lab2.Products;
using Lab4.Abstractions;
using Lab4.Events;

namespace Lab4.Products;

public sealed class NewPackage : Package, INewProduct, IDisposable
{
    public event EventHandler<ProductEventArgs> DataChanged;

    public NewPackage(INewProduct product) : base(product)
    {
        product.DataChanged += OnProductDataChanged;
    }

    private void OnProductDataChanged(object sender, ProductEventArgs e)
    {
        if (sender is not INewProduct product) return;

        Id = product.Id;
        Price = product.Price * 1.1m;

        DataChanged?.Invoke(this, e);
    }

    public void Dispose()
    {
        ((INewProduct)Product).DataChanged -= OnProductDataChanged;
    }
}
