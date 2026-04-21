using Lab2.Abstractions;
using Lab3.Abstractions;

namespace Lab3.Extensions;

public static class Extensions
{
    public static void SetCode<T>(this T thing, long id, int index) where T : class, IProduct
    {
        if (thing is null) return;

        thing.Code.Text = $"{id}[{index + 1}]:({thing.Id}:{thing.Price})";
    }

    public static void SetCode<T>(this T thing, IAssortment<T> assortment) where T : class, IProduct
    {
        if (thing == null) return;

        var index = assortment.Find(thing.Id);

        thing.SetCode(assortment.Id, index);
    }
}