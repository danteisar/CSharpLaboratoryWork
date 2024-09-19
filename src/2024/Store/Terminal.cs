using Barcode.Lab1;
using Barcode.Lab3;
using Product.Lab4;
using Showcase.Lab4;
using System.Text;
using static Store.Constants;

namespace Store;

public class Terminal
{
    #region ctor

    public Terminal()
    {
        _width = Console.WindowWidth - 1;
        _field = new Item[_width + 1, FIELD_HEIGHT + 1];
        _customer = new Customer();

        _rightSideInfoPos = _width * 3 / 4 - 6;
        _storeIdPos = (_rightSideInfoPos + 2 + 4, FIELD_HEIGHT + 3);
        _storeNamePos = (_rightSideInfoPos + 2 + 6, FIELD_HEIGHT + 4);
        _storeCodePos = (_rightSideInfoPos + 2 + 6, FIELD_HEIGHT + 5);
        _selectedIdPos = (6, HEIGHT - 1);
        _selectedNamePos = (8 + _rightSideInfoPos / 2, HEIGHT - 1);

        _opeartions = new();

        CanRun = true;

        Init();
    }

    private readonly int _width;
    private bool _isFull = true;
    private bool _isFreeMoving = false;

    private readonly Item[,] _field;
    private readonly List<Item> _opeartions;
    private readonly Customer _customer;
    private readonly int _rightSideInfoPos;
    private readonly (int x, int y) _storeIdPos;
    private readonly (int x, int y) _selectedIdPos;
    private readonly (int x, int y) _storeNamePos;
    private readonly (int x, int y) _storeCodePos;
    private readonly (int x, int y) _selectedNamePos;

    public bool CanRun { get; private set; }

    #endregion

    #region Run

    private bool CanMoveX(int step) => !(_customer.X + step < 1 || _customer.X + step >= _width - 1);

    private bool CanMoveY(int step) => !(_customer.Y + step < 1 || _customer.Y + step >= FIELD_HEIGHT);

    private void Search(bool byX, int step)
    {
        if (_isFreeMoving)
        {
            if (byX && CanMoveX(step)) _customer.X += step;
            if (!byX && CanMoveY(step)) _customer.Y += step;
            return;
        }

        IThing4 p = Item?.Product;
        IAssortment4<IThing4> s = Item?.Store;
        int index = GetPosition(s);

        if (byX)
        {
            if (s is null)
                while (CanMoveX(step) && GetPosition(Item?.Store) == -1)
                {
                    _customer.X += step;
                }

            if (Item?.Store is { })
                _customer.Y = GetStorePosition(Item?.Store).Value.y + 1;

            if (s is { } && index > -1)
            {
                while (CanMoveX(step) && GetPosition(Item?.Store) == index)
                {
                    _customer.X += step;
                }
                if (step < 0) _customer.X += step;
            }

            if (Item is { } && Item.Char == IV)
                _customer.X--;

            if (Item?.Product is null && GetPosition(Item?.Store) == -1 && CanMoveY(step))
            {
                _customer.Y += step;
                _customer.X = step > 0 ? 1 : _width - 1;
                Search(byX, step);
            }
        }
        else
        {
            while (CanMoveY(step) && Item?.Store == s)
                _customer.Y += step;

            if (Item?.Store is { } && GetPosition(Item?.Store) == -1)
                Search(!byX, step);

            if (GetPosition(Item?.Store) != -1)
                _customer.Y = GetStorePosition(Item?.Store).Value.y + 1;

            if (Item is { } && Item.Char == IV)
                _customer.X--;

            if (Item?.Store is null && CanMoveX(step))
            {
                _customer.X += step;
                _customer.Y = step > 0 ? FIELD_HEIGHT - 1 : 1;
                Search(byX, step);
            }
        }

    }

    public void Run()
    {
        var key = Console.ReadKey(true);
        var e = _field[_customer.X, _customer.Y];
        (int x, int y) pos = (_customer.X, _customer.Y);
        bool canRestore = true;

        switch (key.Key)
        {
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
                VisualizeOpeartion(Operations.MoveA, () => Search(true, -1));
                break;
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                VisualizeOpeartion(Operations.MoveW, () => Search(false, -1));
                break;
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                VisualizeOpeartion(Operations.MoveD, () => Search(true, 1));
                break;
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                VisualizeOpeartion(Operations.MoveS, ()=> Search(false, 1));
                break;
            case ConsoleKey.Spacebar:
                VisualizeOpeartion(Operations.Space, Check);
                canRestore = false;
                break;
            case ConsoleKey.R:
                VisualizeOpeartion(Operations.Remove, Remove);
                canRestore = false;
                break;
            case ConsoleKey.E:
                VisualizeOpeartion(Operations.Sort, Sort);
                canRestore = false;
                break;
            case ConsoleKey.N:
                VisualizeOpeartion(Operations.New, Add);
                canRestore = false;
                break;
            case ConsoleKey.C:
                VisualizeOpeartion(Operations.Clear, Clear);
                canRestore = false;
                break;
            case ConsoleKey.D1:
                _isFreeMoving = !_isFreeMoving;
                VisualizeOpeartion(Operations.MoveMode1);
                canRestore = false;
                break;
            case ConsoleKey.D2:
                _isFull = !_isFull;
                VisualizeOpeartion(Operations.ShowMode2);
                canRestore = false;
                break;
            case ConsoleKey.Z:
                if (key.Modifiers == ConsoleModifiers.Shift)
                    _includeLoading = true;
                Init();
                return;
            case ConsoleKey.Q:
                CanRun = false;                
                return;
        }

        if (!CanRun) return;

        RestoreChar(pos);

        if (canRestore)
            RestoreChar(e);

        WriteCustomer();

        WriteProductInfo(_isFull);
    }

    #endregion

    #region TAKE & PUT

    private Item Item => _field[_customer.X, _customer.Y];

    private void Check()
    {
        if (_customer.Item.Product is { })
        {
            TryPut();
        }
        else
        {
            TryTake();
        }
    }

    private void Operation(Action method)
    {
        if (Item.Store is not null)
        {
            method();
        }
        RewriteStore(Item);
    }

    private void Clear()
    {
        if (Item is null) return;

        Operation(() =>
        {
            for (int i = 0; i < Item.Store.Size; i++)
            {
                Item.Store.Pop();
            }
        });
    }

    private void Sort()
    {
        if (Item is null) return;
        Operation(Item.Store.OrderById);
    }

    private void Add()
    {
        if (Item is null) return;
        if (Item.Product is { }) return;
        try
        {
            ClearProductInfo();
            Console.CursorVisible = true;
            Console.SetCursorPosition(_selectedIdPos.x, _selectedIdPos.y);
            var tmp = Console.ReadLine();
            tmp = tmp.Length > 6 ? tmp.Substring(0, 6) : tmp;
            int id = Convert.ToInt32(tmp);
            Console.SetCursorPosition(_selectedNamePos.x, _selectedNamePos.y);
            string name = Console.ReadLine();

            int i = 0;
            AnimateText(3, FIELD_HEIGHT + 3 + i++, ["AUTHOR: "], 50);
            string author = Console.ReadLine();

            AnimateText(3, FIELD_HEIGHT + 3 + i++, ["YEAR: "], 50);            
            int year = Convert.ToInt32(Console.ReadLine());

            AnimateText(3, FIELD_HEIGHT + 3 + i++, ["PRICE: "], 50);
            decimal price = Convert.ToDecimal(Console.ReadLine());

            var index = GetPosition(Item.Store);
            Operation(() => Item.Store[index] = new Book4(id, name, author, year, price));

        }
        catch
        {
            ClearProductInfo();
            Console.ForegroundColor = ConsoleColor.Red;
            AnimateText(3, FIELD_HEIGHT + 3, ["ERROR"], 50);
            Thread.Sleep(333);
        }
        Console.CursorVisible = false;
    }

    private void Remove()
    {
        if (Item?.Product is null) return;
        Operation(() => Item.Store.Pop(GetPosition(Item.Store)));
    }

    private void RewriteStore(Item e)
    {
        var (x, y) = HideStore(e.Store);
        ShowStore(e.Store, x, y, e.Color);
    }

    private void TryTake()
    {
        var e = Item;
        if (e?.Product is null) return;

        _customer.StoreProduct(e);
        int index = GetPosition(e.Store);
        e.Store.Pop(index);

        RewriteStore(e);
    }

    private void TryPut()
    {
        var e = Item;
        if (e?.Store is null) return;

        var tmp = _customer.Item?.Product;
        if (tmp is null) return;

        var index = GetPosition(e.Store);

        if (e?.Product is not null)
        {
            _customer.StoreProduct(e);
            e.Store.Replace(tmp, index);
        }
        else
        {
            e.Store[index] = tmp;
            _customer.EmptyProduct();
        }

        RewriteStore(e);
    }

    private int GetPosition(IAssortment4<IThing4> store)
    {
        var pos = GetStorePosition(store);
        if (!pos.HasValue) return -1;

        var customer = Item;

        var i = pos.Value.x + 1;
        var index = -1;
        while (i < customer.X)
        {
            if (_field[i, pos.Value.y + 1].Char == IV)
                index++;
            i++;
        }

        return index;
    }

    #endregion

    #region Write & Hide

    private void WriteCustomer()
    {
        var el = _field[_customer.X, _customer.Y];

        if (el is { })
        {
            _customer.Write(el.Char);
        }
        else
        {
            _customer.Write();
        }
    }

    private void WriteProductInfo(bool isFull)
    {
        WriteProductInfo(_field[_customer.X, _customer.Y], isFull);
        WriteProductInfo(_customer.Item, false, 1);
    }

    private void ClearProductInfo(int pos = 0)
    {
        if (pos == 0)
        {
            for (int i = 0; i < HEIGHT - FIELD_HEIGHT - 6; i++)
            {
                WriteString(
                    3 + _rightSideInfoPos * pos,
                    FIELD_HEIGHT + 3 + i,
                    string.Empty.PadRight(_rightSideInfoPos - 4, ' ')
                    );
            }
            var empty = string.Empty.PadRight(_rightSideInfoPos / 2 - 2 - _selectedIdPos.x, ' ');
            WriteString(_selectedIdPos.x, _selectedIdPos.y, empty);
            empty = string.Empty.PadRight(_rightSideInfoPos - 2 - _selectedNamePos.x, ' ');
            WriteString(_selectedNamePos.x, _selectedNamePos.y, empty);
        }
        else
        {
            WriteString(_storeIdPos.x, _storeIdPos.y, string.Empty.PadRight(_width - _storeIdPos.x - 2, ' '));
            WriteString(_storeNamePos.x, _storeNamePos.y, string.Empty.PadRight(_width - _storeNamePos.x - 2, ' '));
            WriteString(_storeCodePos.x, _storeCodePos.y, string.Empty.PadRight(_width - _storeCodePos.x - 2, ' '));
        }
    }

    private void WriteProductInfo(Item p, bool isFull, int pos = 0)
    {
        if (p is { }) Console.ForegroundColor = p.Color;

        // clear
        ClearProductInfo(pos);

        if (pos == 0)
        {
            // Product ID
            var empty = string.Empty.PadRight(_rightSideInfoPos / 2 - 2 - _selectedIdPos.x, ' ');
            WriteString(_selectedIdPos.x, _selectedIdPos.y, p?.Product?.Id.ToString() ?? empty);

            // Product Name
            empty = string.Empty.PadRight(_rightSideInfoPos - 2 - _selectedNamePos.x, ' ');
            WriteString(_selectedNamePos.x, _selectedNamePos.y, p?.Product?.Name.ToString() ?? empty);

            if (p?.Product is null) return;

            var t = !isFull && pos == 0
                ? p.Product.Barcode.ToString().Split('\n')
                : p.Product.ToString().Split('\n');
            // INFO
            for (int i = 0; i < t.Length; i++)
            {
                WriteString(3 + _width / 2 * pos, FIELD_HEIGHT + 3 + i, t[i]);
            }
        }
        else
        {
            var empty = string.Empty.PadRight(_width - _storeIdPos.x - 2, ' ');
            WriteString(_storeIdPos.x, _storeIdPos.y, p?.Product?.Id.ToString() ?? empty);

            empty = string.Empty.PadRight(_width - _storeNamePos.x - 2, ' ');
            WriteString(_storeNamePos.x, _storeNamePos.y, p?.Product?.Name.ToString() ?? empty);

            empty = string.Empty.PadRight(_width - _storeCodePos.x - 2, ' ');
            WriteString(_storeCodePos.x, _storeCodePos.y, p?.Product?.Barcode.Text ?? empty);
        }
    }

    private (int x, int y)? GetStorePosition(IAssortment4<IThing4> store)
    {
        (int x, int y)? tmp = null;

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < FIELD_HEIGHT; j++)
            {
                if (_field[i, j] is { } && _field[i, j].Store == store)
                {
                    if (!tmp.HasValue)
                    {
                        tmp = (i, j);
                        break;
                    }
                }
            }
        }

        return tmp;
    }

    private (int x, int y) HideStore(IAssortment4<IThing4> store)
    {
        (int x, int y)? tmp = null;

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < FIELD_HEIGHT; j++)
            {
                if (_field[i, j] is { } && _field[i, j].Store == store)
                {
                    if (!tmp.HasValue)
                        tmp = (i, j);

                    _field[i, j] = null;
                    WriteChar(i, j, EMPTY);
                }
            }
        }

        return tmp.Value;
    }

    #endregion

    #region Show

    private void ShowTerminal()
    {
        Console.ForegroundColor = BORDER_COLOR;

        // Horizontal
        for (int i = 1; i < _width; i++)
        {
            WriteChar(i, 0, HORIZONTAL_BORDER);
            WriteChar(i, FIELD_HEIGHT, HORIZONTAL_BORDER);
            WriteChar(i, HEIGHT, HORIZONTAL_BORDER);
        }
        for (int i = 1; i < _rightSideInfoPos; i++)
        {
            WriteChar(i, HEIGHT - 2, HORIZONTAL_BORDER);
        }

        for (int i = _rightSideInfoPos; i < _width; i++)
        {
            WriteChar(i, HEIGHT - 13, HORIZONTAL_BORDER);
        }

        // Vertical
        for (int i = 1; i < HEIGHT; i++)
        {
            WriteChar(0, i, VERTICAL_BORDER);
            WriteChar(_width, i, VERTICAL_BORDER);
        }

        for (int i = FIELD_HEIGHT; i < HEIGHT; i++)
        {
            WriteChar(_rightSideInfoPos, i, VERTICAL_BORDER);
        }
        // corners
        WriteChar(_rightSideInfoPos, FIELD_HEIGHT, CROSS_TOP_BORDER);
        WriteChar(_rightSideInfoPos, HEIGHT - 2, CROSS_RIGHT_BORDER);
        WriteChar(_rightSideInfoPos, HEIGHT, CROSS_BOTTOM_BORDER);

        WriteChar(0, 0, TOP_LEFT_BORDER);
        WriteChar(_width, 0, TOP_RIGHT_BORDER);
        WriteChar(0, HEIGHT, BOTTOM_LEFT_BORDER);
        WriteChar(_width, HEIGHT, BOTTOM_RIGHT_BORDER);
        WriteChar(_width, FIELD_HEIGHT, CROSS_RIGHT_BORDER);
        WriteChar(0, FIELD_HEIGHT, CROSS_LEFT_BORDER);
        WriteChar(0, HEIGHT - 2, CROSS_LEFT_BORDER);

        WriteChar(_rightSideInfoPos, HEIGHT - 13, CROSS_LEFT_BORDER);
        WriteChar(_width, HEIGHT - 13, CROSS_RIGHT_BORDER);

        Console.SetCursorPosition(2, HEIGHT - 1);
        Console.Write("ID:");
        Console.SetCursorPosition(2 + _rightSideInfoPos / 2, HEIGHT - 1);
        Console.Write("NAME:");

        WriteChar(_rightSideInfoPos / 2, HEIGHT - 2, TV);
        WriteChar(_rightSideInfoPos / 2, HEIGHT - 1, IV);
        WriteChar(_rightSideInfoPos / 2, HEIGHT - 0, LV);

        Console.SetCursorPosition(2, FIELD_HEIGHT + 1);
        Console.Write("─ POSITION INFORMATION ─");
        Console.SetCursorPosition(2 + _rightSideInfoPos, FIELD_HEIGHT + 1);
        Console.Write("─ STORED INFORMATION ─");
        Console.SetCursorPosition(2 + _rightSideInfoPos, FIELD_HEIGHT + 3);
        Console.Write("ID:");
        Console.SetCursorPosition(2 + _rightSideInfoPos, FIELD_HEIGHT + 4);
        Console.Write("NAME:");
        Console.SetCursorPosition(2 + _rightSideInfoPos, FIELD_HEIGHT + 5);
        Console.Write("CODE:");

        int pos = 12;
        Console.ForegroundColor = HELP_COLOR;
        //                         11111111112222222222333
        //               012345678901234567890123456789012
        ShowHelp(pos--, " ─ CONTROLS ─          Z - restart");
        ShowHelp(pos--, "                         Q - quit");
        ShowHelp(pos--, "                    1         2  ");//-1
        ShowHelp(pos--, "     ╔───╗        ╔─┬─╗     ╔─┬─╗");//0
        ShowHelp(pos--, "     │ W │    MOVE│1│2│ SHOW│1│2│");//1
        ShowHelp(pos--, "     ╚───╝        ╚─┴─╝     ╚─┴─╝");//2
        ShowHelp(pos--, "╔───╗╔───╗╔───╗     ╔─╗       ╔─╗");//3
        ShowHelp(pos--, "│ A ││SPC││ D │ SORT│E│  CLEAR│C│");//4
        ShowHelp(pos--, "╚───╝╚───╝╚───╝     ╚─╝       ╚─╝");//5
        ShowHelp(pos--, "     ╔───╗          ╔─╗       ╔─╗");//6
        ShowHelp(pos--, "     │ S │    REMOVE│R│    NEW│N│");//7
        ShowHelp(pos--, "     ╚───╝          ╚─╝       ╚─╝");//8

        SaveText(" W ", 5, 0, Operations.MoveW);
        SaveText(" A ", 0, 3, Operations.MoveA);
        SaveText(" S ", 5, 6, Operations.MoveS);
        SaveText(" D ", 10, 3, Operations.MoveD);
        SaveText("SPC", 5, 3, Operations.Space);
        SaveText("E", 20, 3, Operations.Sort);
        SaveText("R", 20, 6, Operations.Remove);
        SaveText("C", 30, 3, Operations.Clear);
        SaveText("N", 30, 6, Operations.New);

        Save('1', Operations.MoveMode1, 2 + _rightSideInfoPos + 19, HEIGHT - 9 + 1);
        Save('2', Operations.MoveMode2, 2 + _rightSideInfoPos + 21, HEIGHT - 9 + 1);
        Save('1', Operations.ShowMode1, 2 + _rightSideInfoPos + 29, HEIGHT - 9 + 1);
        Save('2', Operations.ShowMode2, 2 + _rightSideInfoPos + 31, HEIGHT - 9 + 1);
    }

    private void SaveText(string t, int offsetX, int offsetY, Operations op)
    {
        Save('╔', op, 2 + _rightSideInfoPos + offsetX, HEIGHT - 9 + offsetY);
        Save('│', op, 2 + _rightSideInfoPos + offsetX, HEIGHT - 8 + offsetY);
        Save('╚', op, 2 + _rightSideInfoPos + offsetX, HEIGHT - 7 + offsetY);

        Save('╗', op, 2 + _rightSideInfoPos + offsetX + t.Length + 1, HEIGHT - 9 + offsetY);
        Save('│', op, 2 + _rightSideInfoPos + offsetX + t.Length + 1, HEIGHT - 8 + offsetY);
        Save('╝', op, 2 + _rightSideInfoPos + offsetX + t.Length + 1, HEIGHT - 7 + offsetY);

        for (int i = 0; i < t.Length; i++)
        {
            Save('─', op, 2 + _rightSideInfoPos + offsetX + i + 1, HEIGHT - 9 + offsetY);
            Save(t[i], op, 2 + _rightSideInfoPos + offsetX + i + 1, HEIGHT - 8 + offsetY);
            Save('─', op, 2 + _rightSideInfoPos + offsetX + i + 1, HEIGHT - 7 + offsetY);
        }
    }

    private void VisualizeOpeartion(Operations o, Action action = null)
    {
        if (o == Operations.MoveMode1)
        {
            if (_isFreeMoving)
            {
                ShowOperation(Operations.MoveMode2, ACTIVE_COLOR);
                ShowOperation(Operations.MoveMode1, HELP_COLOR);
            }
            else
            {
                ShowOperation(Operations.MoveMode1, ACTIVE_COLOR);
                ShowOperation(Operations.MoveMode2, HELP_COLOR);
            }
            return;
        }

        if (o == Operations.ShowMode2)
        {
            if (_isFull)
            {
                ShowOperation(Operations.ShowMode1, ACTIVE_COLOR);
                ShowOperation(Operations.ShowMode2, HELP_COLOR);
            }
            else
            {
                ShowOperation(Operations.ShowMode2, ACTIVE_COLOR);
                ShowOperation(Operations.ShowMode1, HELP_COLOR);
            }
            return;
        }

        ShowOperation(o, ACTIVE_COLOR);

        action?.Invoke();

        Thread.Sleep(100);

        ShowOperation(o, HELP_COLOR);
    }

    private void ShowOperation(Operations o, ConsoleColor c)
    {
        Console.ForegroundColor = c;
        foreach (var item in _opeartions.Where(x => x.Operation == o))
        {
            Console.SetCursorPosition(item.X, item.Y);
            Console.Write(item.Char);
        }
    }

    private void Save(char c, Operations o, int x, int y)
    {
        _opeartions.Add((c, o, x, y));
    }

    private void ShowHelp(int pos, string text)
    {
        Console.SetCursorPosition(2 + _rightSideInfoPos, HEIGHT - pos);
        Console.Write(text);
    }

    private static IThing4 Check(IAssortment4<IThing4> store, int index)
    {
        var tmp = store[index];
        store[index] = tmp;
        return tmp;
    }

    private void ShowStore(IAssortment4<IThing4> store, int x, int y, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        var sb = new StringBuilder();
        sb.Append(IV);
        sb.Append("ID:");
        sb.Append(store.Id);
        sb.Append(IV);
        for (int i = 0; i < store.Size; i++)
        {
            var item = Check(store, i);
            sb.Append(item?.Id.ToString() ?? " ");
            sb.Append(IV);
        }
        var tmp = sb.ToString();

        var topTmp = new StringBuilder(tmp);
        for (int i = 0; i < topTmp.Length; i++)
        {
            if (i == 0)
                topTmp[i] = A11;
            else if (i == topTmp.Length - 1)
                topTmp[i] = A3;
            else if (topTmp[i] == IV)
                topTmp[i] = A6;
            else
                topTmp[i] = A9;
        }

        var bottomTmp = new StringBuilder(tmp);
        for (int i = 0; i < topTmp.Length; i++)
        {
            if (i == 0)
                bottomTmp[i] = A4;
            else if (i == bottomTmp.Length - 1)
                bottomTmp[i] = A10;
            else if (bottomTmp[i] == IV)
                bottomTmp[i] = A5;
            else
                bottomTmp[i] = A9;
        }

        for (int i = 0; i < tmp.Length; i++)
        {
            _field[x + i, y] = new Item
            {
                Char = topTmp[i],
                Color = color,
                Store = store,
                X = x + i,
                Y = y
            };
            _field[x + i, y + 1] = new Item
            {
                Char = tmp[i],
                Color = color,
                Store = store,
                X = x + i,
                Y = y + 1
            };
            _field[x + i, y + 2] = new Item
            {
                Char = bottomTmp[i],
                Color = color,
                Store = store,
                X = x + i,
                Y = y + 2
            };
        }

        WriteString(x, y, topTmp.ToString());
        WriteString(x, y + 1, tmp);
        WriteString(x, y + 2, bottomTmp.ToString());

        int pos = store.Id.ToString().Length + 5;
        int prodPos = 0;

        while (pos < tmp.Length && prodPos < store.Size)
        {
            var item = Check(store, prodPos);
            ShowProduct(item, x + pos, y + 1);
            prodPos++;
            while (tmp[pos] != IV)
                pos++;
            pos++;
        }

    }

    private void ShowProduct(IThing4 product, int x, int y)
    {
        if (product is null) return;
        var tmp = product.Id.ToString();
        for (int i = 0; i < tmp.Length; i++)
        {
            _field[x + i, y].Product = product;
        }
    }

    #endregion

    #region Help

    private static void WriteChar(int posX, int posY, char c)
    {
        Console.SetCursorPosition(posX, posY);
        Console.Write(c);
        Console.CursorVisible = false;
    }

    private static void WriteString(int posX, int posY, string str)
    {
        Console.SetCursorPosition(posX, posY);
        Console.Write(str);
        Console.CursorVisible = false;
    }

    private static void RestoreChar(Item e)
    {
        if (e is null) return;
        Console.ForegroundColor = e.Color;
        WriteChar(e.X, e.Y, e.Char);
    }

    private static void RestoreChar((int x, int y) pos)
    {
        WriteChar(pos.x, pos.y, EMPTY);
    }

    private static void ClearConsole()
    {
        Console.WindowHeight = HEIGHT + 1;

        Console.CursorVisible = false;
        Console.BackgroundColor = BACKGROUND_COLOR;
        Console.ForegroundColor = FOREGROUND_COLOR;

        Console.Clear();

        Barcode1.Type = BarcodeType.Full;
        Barcode3.Type = BarcodeType.Full;       
    }

    private static void Test4<T>(IAssortment4<T> a, IEnumerable<T> list, T test) where T : class, IThing4
    {
        Console.WriteLine("".PadLeft(80, '='));
        foreach (var product in list)
        {
            a.Push(product);
        }
        a.OrderByName();

        a.Id++;
        test.Id++;

        Console.WriteLine(a);
    }

    private static void ShowLoading()
    {
        NormalClear();
        var rnd = new Random();
        Console.CursorVisible = false;
        var y = 0;
        foreach (var item in MsDos())
        {
            AnimateText(0, y++, [item], 0);
            Thread.Sleep(rnd.Next(10, 100));
        }
        y--;
        var x = MsDos().Last().Length;
        for (int i = 0; i < 3; i++)
        {
            AnimateText(4, y, [" "], 500);
            AnimateText(4, y, ["_"], 500);
        }
        AnimateText(4, y, ["TERMINAL.EXE"], 50);
        Thread.Sleep(500);
        ClearConsole();
        Barcode1 logo = "TERMINAL v.1.0";
        var text = logo.ToString().Split('\n');
        var posX = (Console.WindowWidth - text[0].Length + 2) / 2;
        var posY = (Console.WindowHeight - 8) / 2;
        
        ShowRectangle(posX-1, posY-1, text[0].Length + 4, 11);
        AnimateText(posX + 1, posY + 1, text, 5);
        Thread.Sleep(1000);        
        _includeLoading = false;
    }

    private static void ShowRectangle(int offsetX, int offsetY, int width, int height)
    {
        width--;
        height--;

        for (int i = 1; i < width; i++)
        {
            WriteChar(offsetX + i, offsetY, A9);
            WriteChar(offsetX + i, offsetY + height, A9);
        }
        for (int i = 1; i < height; i++)
        {
            WriteChar(offsetX, offsetY + i, IV);
            WriteChar(offsetX + width, offsetY + i, IV);
        }
        WriteChar(offsetX, offsetY, A11);
        WriteChar(offsetX, offsetY + height, A4);
        WriteChar(offsetX + width, offsetY, A3);
        WriteChar(offsetX + width, offsetY + height, A10);
    }

    private static void AnimateText(int x, int y, string[] text, int delay)
    {        
        for (int i = 0; i < text[0].Length; i++)
        {
            for (int j = 0; j < text.Length; j++)
            {
                if (i < text[j].Length)
                {
                    Console.SetCursorPosition(x + i, y + j);
                    Console.Write(text[j][i]);
                }
            }
            if (delay > 0) Thread.Sleep(delay);
        }
    }

    private static void NormalClear()
    {
        Console.ResetColor();
        Console.Clear();
        Console.CursorVisible = true;
    }

    private static bool _isDemo = true;
    private static bool _includeLoading = true;
    private static bool _include5th = false;

    private static string[] MsDos()
    {
        return
        [
            "Welcome to FreeDOS",
            "",
            "CuteMouse v1.9.1 alpha 1 [FreeDOS]",
            "Installed at PS/2 port",
            @"c:\>ver",
            "",
            "FreeCom version 0.82 pl 3 XMS_Swap [Dec 19 2024 18:00:00]",
            "",
            @"C:\>dir",
            " Volume in drive C is FREEDOS_C95",
            " Volume Serial Number is 0E4F-19EB",
            @" Directory of C:\",
            "",
            "FDOS                <DIR> 08-26-04 6:23p",
            "AUTOEXEC BAT          435 08-26-04 6:24p",
            "BOOTSECT BIN          512 08-26-04 6:23p",
            "COMMAND  COM       93,963 08-26-04 6:24p",
            "CONFIG   SYS          801 08-26-04 6:24p",
            "FDOSBOOT BIN          512 08-26-04 6:24p",
            "KERNEL   SYS       45,815 04-17-04 9:19p",
            "TERMINAL EXE      224,455 09-19-24 6:00p",
            "         7 file(s)     366,493 bytes",
            "         1 dir(s) 1,064,517,632 bytes free",
            "",
            @"C:\>",
        ];
    }

    private void Init()
    {        
        var rnd = new Random();
        var lab4Data = new List<IThing4>
        {
            new Book4(3000, "ВОЙНА И МИРЪ III", "Л.Н. Толстой", 1867, 300000),
            new Book4(1000, "ВОЙНА И МИРЪ I", "Л.Н. Толстой", 1863, 1000000),
            new Book4(2000, "ВОЙНА И МИРЪ II", "Л.Н. Толстой", 1865, 200000),
            new Book4(4000, "ВОЙНА И МИРЪ IV", "Л.Н. Толстой", 1869, 400000)
        };

        for (int i = 4; i < 99; i++)
        {
            lab4Data.Add(new Book4(rnd.Next(1, 1000), rnd.Next(1, 100).ToString(), rnd.Next(1, 100).ToString(), rnd.Next(1860, 2024), (decimal)rnd.NextDouble() * 1000));
        }
        var lab4Data2 = new List<Comic4>
        {
            new (5555, "Хранители", "С. Маккоауд", 2008, 2071),
            new (6666, "Понимание комикса", "А. Шпигельман", 1990, 860),
            new (7777, "Ходячие мертвецы", "Р. Кирман", 2003, 2257)
        };

        IAssortment4<IThing4> store1 = (Assortment4<IThing4>)20;
        store1[0] = lab4Data[0];
        store1[2] = lab4Data[1];
        store1[3] = lab4Data[3];
        store1[10] = lab4Data[2];

        IAssortment4<IThing4> store2 = (Assortment4<IThing4>)20;
        store2[1] = lab4Data2[0];
        store2[3] = lab4Data2[1];
        store2[5] = lab4Data2[2];

        IAssortment4<IThing4> store3 = (Assortment4<IThing4>)10;

        for (int i = 0; i < 10; i++)
        {
            store3[i] = lab4Data[rnd.Next(0, 99)];
        }        

        if (_isDemo)
        {               
            ConsoleKeyInfo key;
            do
            {
                NormalClear();
                var text = "Вы были 19.09.2024 на лекции? (y/n)";
                var posX = (Console.WindowWidth - text.Length + 2) / 2;
                var posY = (Console.WindowHeight - 3) / 2;
                ShowRectangle(posX, posY, text.Length + 2, 3);
                AnimateText(posX+1, posY+1, [text], 5);
                key = Console.ReadKey(true);
            }
            while (key.Key != ConsoleKey.Y && key.Key != ConsoleKey.N && key.Key != ConsoleKey.Spacebar);

            _include5th = key.Key != ConsoleKey.Y;

            if (key.Key == ConsoleKey.Spacebar)
                _includeLoading = false;

            _isDemo = false;
        }

        if (!_include5th)
        {
            var data = lab4Data.Concat(lab4Data2.Select(x => x as IThing4));
            
            NormalClear();
            Test4(store1, data, lab4Data[0]);
            Test4(store2, lab4Data2, lab4Data2[0]);
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            CanRun = false;
        }
        else
        {
            if (_includeLoading) ShowLoading();

            _opeartions.Clear();
            ClearConsole();
            ShowTerminal();
            VisualizeOpeartion(Operations.MoveMode1);
            VisualizeOpeartion(Operations.ShowMode2);

            ShowStore(store1, 2, 1, STORE1);
            ShowStore(store2, 2, 4, STORE2);
            ShowStore(store3, 2, 7, STORE3);

            _customer.X = 1;
            _customer.Y = 1;
            _customer.Write();
        }        
    }

    #endregion
}
