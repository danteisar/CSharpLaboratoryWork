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
        Console.WindowHeight = HEIGHT + 1;       

        Console.CursorVisible = false;        
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;

        Console.Clear();

        Barcode1.Type = BarcodeType.Full;
        Barcode3.Type = BarcodeType.Full;

        _width = Console.WindowWidth - 1;
        _field = new Item[_width + 1, FIELD_HEIGHT + 1];
        _customer = new Customer();
        
        _rightSideInfoPos = _width * 3 / 4;
        _storeIdPos = (_rightSideInfoPos + 2, FIELD_HEIGHT + 4);
        _storeNamePos = (_rightSideInfoPos + 2, FIELD_HEIGHT + 7);
        _selectedIdPos = (5, HEIGHT - 1);
        _selectedNamePos = (7 + _rightSideInfoPos / 2, HEIGHT - 1);

        CanRun = true;

        Init();
    }

    private int _width;
    private bool _isFull = true;
    private bool _isFreeMoving = false;

    private readonly Item[,] _field;
    private readonly Customer _customer;   
    private readonly int _rightSideInfoPos;
    private readonly  (int x, int y) _storeIdPos;
    private readonly  (int x, int y) _selectedIdPos;
    private readonly  (int x, int y) _storeNamePos;
    private readonly  (int x, int y) _selectedNamePos;

    public bool CanRun { get; private set; }

    #endregion

    #region Run

    private bool _canMoveX(int step) => !(_customer.X + step < 1 || _customer.X + step >= _width - 1);
    
    private bool _canMoveY(int step) => !(_customer.Y + step < 1 || _customer.Y + step >= FIELD_HEIGHT);

    private void Search(bool byX, int step)
    {       
        if (_isFreeMoving)
        {
            if (byX && _canMoveX(step)) _customer.X += step;
            if (!byX && _canMoveY(step)) _customer.Y += step;
            return;
        }
        
        IThing4 p = Item?.Product;
        IAssortment4<IThing4> s = Item?.Store;        
        int index = GetPosition(s);

        if (byX)
        {
            if (s is null)
            {
                while (_canMoveX(step) && GetPosition(Item?.Store) == -1) 
                {                
                    _customer.X += step;
                }                 
            }

            if (s is {} && index>-1)
            {
                _customer.Y = GetStorePosition(s).Value.y + 1;
                while (_canMoveX(step) && GetPosition(Item?.Store) == index) 
                {                
                    _customer.X += step;                    
                }
                if (step < 0) _customer.X += step;   
            }  

            if (Item?.Product is null && GetPosition(Item?.Store) == -1 && _canMoveY(step))
            {
                _customer.Y += step;
                _customer.X = step > 0 ? 1 : _width - 1;
                Search(byX, step);
            }
        }    
        else
        {
            while (_canMoveY(step) && Item?.Store == s) 
            {                
                _customer.Y += step;
            }  

            if (Item?.Store is {} && GetPosition(Item?.Store) == -1)
            {
                Search(!byX, step); 
            } 

            if (GetPosition(Item?.Store) != -1) 
            {
                _customer.Y = GetStorePosition(Item?.Store).Value.y + 1;
            }
         
            if (Item?.Store is null && _canMoveX(step))
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
                Search(true, -1);
                break;
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                Search(false, -1);
                break;
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                Search(true, 1);
                break;
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                Search(false, 1);
                break;
            case ConsoleKey.Spacebar:
                Check();
                canRestore = false;
                break;
            case ConsoleKey.R:
                Clear();
                canRestore = false;
                break;
            case ConsoleKey.E:
                Sort();
                canRestore = false;
                break;
            case ConsoleKey.N:
                Add();
                canRestore = false;
                break;
            case ConsoleKey.C:
                ClearAtPos();
                canRestore = false;
                break;
            case ConsoleKey.D1:
                _isFreeMoving = !_isFreeMoving;
                canRestore = false;
                break;
            case ConsoleKey.D2:
                _isFull = !_isFull;                
                canRestore = false;
                break;
            case ConsoleKey.Z:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Clear();
                Init();
                return;
            case ConsoleKey.Q:
                CanRun = false;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();
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
            Console.SetCursorPosition(3, FIELD_HEIGHT + 3 + i++);
            Console.Write("AUTHOR: ");
            string author = Console.ReadLine();
            Console.SetCursorPosition(3, FIELD_HEIGHT + 3 + i++);
            Console.Write("YEAR: ");
            int year = Convert.ToInt32(Console.ReadLine());
            Console.SetCursorPosition(3, FIELD_HEIGHT + 3 + i++);
            Console.Write("PRICE: ");
            decimal price = Convert.ToDecimal(Console.ReadLine());

            var index = GetPosition(Item.Store);
            Operation(() => Item.Store[index] = new Book4(id, name, author, year, price));
            
        }
        catch
        {
            ClearProductInfo();
            Console.SetCursorPosition(3, FIELD_HEIGHT + 3);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR");
        }
        Console.CursorVisible = false;
    }

    private void ClearAtPos()
    {
        if (Item?.Product is null) return;
        var index = GetPosition(Item.Store);
        Operation(()=>Item.Store.Pop(index));
    }

    private void RewriteStore(Item e)
    {
        var pos = HideStore(e.Store);
        ShowStore(e.Store, pos.x, pos.y, e.Color);
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
                    3 + _width * 3 / 4 * pos,
                    FIELD_HEIGHT + 3 + i,
                    string.Empty.PadRight(_width * 3 / 4 - 4, ' ')
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
        Console.ForegroundColor = ConsoleColor.Cyan;             

        // Horizontal
        for (int i = 1; i < _width; i++)
        {
            WriteChar(i, 0, HORIZONTAL_BORDER);
            WriteChar(i, FIELD_HEIGHT, HORIZONTAL_BORDER);
            WriteChar(i, HEIGHT, HORIZONTAL_BORDER);
        }
        for (int i = 1; i < _width * 3 / 4; i++)
        {
            WriteChar(i, HEIGHT - 2, HORIZONTAL_BORDER);
        }

        for (int i = _rightSideInfoPos; i < _width; i++)
        {
            WriteChar(i, HEIGHT - 12, HORIZONTAL_BORDER);
        }        

        // Vertical
        for (int i = 1; i < HEIGHT; i++)
        {
            WriteChar(0, i, VERTICAL_BORDER);
            WriteChar(_width, i, VERTICAL_BORDER);
        }

        for (int i = FIELD_HEIGHT; i < HEIGHT; i++)
        {
            WriteChar(_width * 3 / 4, i, VERTICAL_BORDER);
        }
        // corners
        WriteChar(_width * 3 / 4, FIELD_HEIGHT, CROSS_TOP_BORDER);
        WriteChar(_width * 3 / 4, HEIGHT - 2, CROSS_RIGHT_BORDER);
        WriteChar(_width * 3 / 4, HEIGHT, CROSS_BOTTOM_BORDER);

        WriteChar(0, 0, TOP_LEFT_BORDER);
        WriteChar(_width, 0, TOP_RIGHT_BORDER);
        WriteChar(0, HEIGHT, BOTTOM_LEFT_BORDER);
        WriteChar(_width, HEIGHT, BOTTOM_RIGHT_BORDER);
        WriteChar(_width, FIELD_HEIGHT, CROSS_RIGHT_BORDER);        
        WriteChar(0, FIELD_HEIGHT, CROSS_LEFT_BORDER);
        WriteChar(0, HEIGHT - 2, CROSS_LEFT_BORDER);

        WriteChar(_rightSideInfoPos, HEIGHT - 12, CROSS_LEFT_BORDER);
        WriteChar(_width, HEIGHT - 12, CROSS_RIGHT_BORDER);

        Console.SetCursorPosition(2, HEIGHT - 1);
        Console.Write("ID:");
        Console.SetCursorPosition(2 + _rightSideInfoPos / 2, HEIGHT - 1);
        Console.Write("NAME:");

        WriteChar(_rightSideInfoPos / 2, HEIGHT - 2, TV);
        WriteChar(_rightSideInfoPos / 2, HEIGHT - 1, IV);
        WriteChar(_rightSideInfoPos / 2, HEIGHT - 0, LV);        

        Console.SetCursorPosition(2, FIELD_HEIGHT + 1);
        Console.Write("POSITION INFORMATION");
        Console.SetCursorPosition(2 + _rightSideInfoPos, FIELD_HEIGHT + 1);
        Console.Write("STORED INFORMATION");
        Console.SetCursorPosition(2 + _rightSideInfoPos, FIELD_HEIGHT + 3);
        Console.Write("ID:");
        Console.SetCursorPosition(2 + _rightSideInfoPos, FIELD_HEIGHT + 6);
        Console.Write("NAME:");

        int pos = 11;
        Console.ForegroundColor = ConsoleColor.Black;
        ShowHelp(pos--, "CONTROLS:"); pos--;
        ShowHelp(pos--, "WASD - move *"); 
        ShowHelp(pos--, "1/2 - move/show modes"); 
        ShowHelp(pos--, "N - new Book"); 
        ShowHelp(pos--, "Space - take/put"); 
        ShowHelp(pos--, "C - remove"); 
        ShowHelp(pos--, "R - clear"); 
        ShowHelp(pos--, "Z - restart"); 
        ShowHelp(pos--, "Q - quit");        
    }
    private void ShowHelp(int pos, string text)
    {
        Console.SetCursorPosition(2 + _rightSideInfoPos, HEIGHT - pos);
        Console.Write(text);
    }

    private IThing4 Check(IAssortment4<IThing4> store, int index)
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

    private void Init()
    {
        ShowTerminal();

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

        ShowStore(store1, 2, 1, ConsoleColor.Magenta);
        ShowStore(store2, 2, 4, ConsoleColor.Red);
        ShowStore(store3, 2, 7, ConsoleColor.White);

        _customer.X = 1;
        _customer.Y = 1;
        _customer.Write();
    }

    #endregion
}
