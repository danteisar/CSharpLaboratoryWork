using static Store.Constants;
using static Store.ConsoleExtensions.ConsoleWriter;

namespace Store.ConsoleWrappers;

internal class Customer
{
    public int X { get; set; }

    public int Y { get; set; }

    public Item Item { get; } = new Item();

    public void StoreProduct(Item item)
    {
        Item.X = X;
        Item.Y = Y;
        Item.Char = item.Char;
        Item.Color = item.Color;
        Item.Product = item.Product;
        Item.Store = item.Store;
    }

    public void EmptyProduct()
    {
        Item.X = X;
        Item.Y = Y;
        Item.Char = OPERATOR;
        Item.Color = COLOR;
        Item.Product = null;
        Item.Store = null;
    }

    public void Write(char c = OPERATOR)
    {
        Write(c != EMPTY ? c : OPERATOR, COLOR);
    }

    public void Write(char c, ConsoleColor color)
    {
        Console.ForegroundColor = FOREGROUND_COLOR;
        Console.BackgroundColor = color;
        Console.SetCursorPosition(X, Y);
        Console.CursorVisible = false;
        Console.Write(c);
        Console.BackgroundColor = BACKGROUND_COLOR;
    }
}
