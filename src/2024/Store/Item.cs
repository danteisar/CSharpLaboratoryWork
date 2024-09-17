using Product.Lab4;
using Showcase.Lab4;

namespace Store;

internal class Item
{
    public IAssortment4<IThing4> Store { get; set; }

    public IThing4 Product { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public ConsoleColor Color { get; set; }

    public char Char { get; set; }

}
