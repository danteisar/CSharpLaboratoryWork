﻿using Product.Lab4;
using Showcase.Lab4;

namespace Store.ConsoleWrappers;

internal class Item
{
    public IAssortment4<IThing4> Store { get; set; }

    public IThing4 Product { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public ConsoleColor Color { get; set; }

    public char Char { get; set; }

    public Operations Operation { get; set; }

    public static implicit operator Item((char c, Operations o, int x, int y) o) => new()
    {
        X = o.x,
        Y = o.y,
        Char = o.c,
        Operation = o.o
    };
}
