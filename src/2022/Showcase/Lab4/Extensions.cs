using Barcode.Lab1;
using Product.Lab4;

namespace Showcase.Lab4;

public static class Extensions
{
    public static void SetBarcode<T>(this T thing, int id, int index) where T : class, IThing4
    {
        if (thing == null) return;
        thing.Barcode = (Barcode1)$"{id} {index + 1} {thing.Id}"; 
    }

    public static void SetBarcode<T>(this IThing4 thing, IAssortment4<T> assortment) where T : class, IThing4
    {
        if (thing == null) return;
        thing.Barcode = (Barcode1)$"{assortment.Id} {assortment.Find(thing.Id) + 1} {thing.Id}";
    }
}