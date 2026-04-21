using Lab2.Abstractions;
using Lab2.Codes;

namespace Lab2.Products;

public class Package(IProduct product)
    : Product(product.Id, product.Price * 1.1m, product.Name, new PackageCode())
{
    public IProduct Product { get; } = product;

    public override string Type => $"Упаковка ({Product.Type})";

    public override string Information => Product.Information;
}
