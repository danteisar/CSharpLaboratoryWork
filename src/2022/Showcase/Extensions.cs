using Product;

namespace Showcase;

public static class Extensions
{
    public static void SetBarcode<T>(this T product, int id, int index) where T : class, IProduct
    {
        if (product == null) return;
        product.Barcode.Text = $"{id} {index + 1} {product.Id}";
    }

    public static void SetBarcode<T>(this IProduct product, IAssortment<T> assortment) where T : class, IProduct
    {
        if (product == null) return;
        product.Barcode.Text = $"{assortment.Id} {assortment.Find(product.Id) + 1} {product.Id}";
    }
}